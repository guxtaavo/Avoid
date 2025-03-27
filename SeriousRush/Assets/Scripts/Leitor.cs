using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using MicronOptics.Hyperion.Communication;

public class Leitor : MonoBehaviour
{
    public static Leitor Instance { get; private set; }

    private TcpClient tcpClient;
    private NetworkStream tcpNetworkStream;
    private SpectrumData spectrumData;

    public int channelLeft = 2;
    public int channelRight = 3;

    public float wavelengthLeft = 0.0f;
    public float wavelengthRight = 0.0f;
    public float mean_openLeft = 0.0f;
    public float mean_closeLeft = 0.0f;

    public float mean_openRight = 0.0f;
    public float mean_closeRight = 0.0f;

    public bool? move = null;

    private List<float> wavelengthHistoryLeft = new List<float>();
    private List<float> wavelengthHistoryRight = new List<float>();

    void Awake()
    {
        // Implementação do Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre cenas
        }
        else
        {
            Destroy(gameObject); // evita duplicatas
        }
    }

    void Start()
    {
        string instrumentIpAddress = "10.0.0.55";

        try
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(instrumentIpAddress, Command.TcpPort);
            tcpNetworkStream = tcpClient.GetStream();

            CommandResponse response = Command.Execute(tcpNetworkStream, CommandOptions.None, CommandName.GetSpectrum);
            spectrumData = response.AsSpectrumData();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro na conexão TCP: {ex.Message}");
        }

        mean_openLeft = mean_closeLeft = CalcWl(channelLeft, wavelengthHistoryLeft);
        mean_openRight = mean_closeRight = CalcWl(channelRight, wavelengthHistoryRight);
    }

    void Update()
    {
        wavelengthLeft = CalcWl(channelLeft, wavelengthHistoryLeft);
        wavelengthRight = CalcWl(channelRight, wavelengthHistoryRight);

        UpdateMeanValues(ref mean_openLeft, ref mean_closeLeft, wavelengthLeft);
        UpdateMeanValues(ref mean_openRight, ref mean_closeRight, wavelengthRight);

        move = CompareWlValues();
    }

    public bool? GetDirection()
    {
        return move;
    }

    float CalcWl(int channel, List<float> history)
    {
        CommandResponse response = Command.Execute(tcpNetworkStream, CommandOptions.None, CommandName.GetSpectrum);
        spectrumData = response.AsSpectrumData();

        int index = 0;
        double max_sdpdata = 0.0;
        int max_ind = 0;

        foreach (double intensity in spectrumData.ToArray(channel))
        {
            if (intensity >= max_sdpdata)
            {
                max_ind = index;
                max_sdpdata = intensity;
            }
            index++;
        }

        float newWavelength = (float)(spectrumData.WavelengthStart + max_ind * spectrumData.WavelengthStep);

        if (history.Count >= 10)
        {
            history.RemoveAt(0);
        }
        history.Add(newWavelength);

        float averageWavelength = 0.0f;
        foreach (float wl in history)
        {
            averageWavelength += wl;
        }
        return averageWavelength / history.Count;
    }

    void UpdateMeanValues(ref float mean_open, ref float mean_close, float wavelength)
    {
        if (wavelength > mean_open)
        {
            mean_open = wavelength;
        }

        if (wavelength < mean_close || mean_close == 0.0f)
        {
            mean_close = wavelength;
        }
    }

    bool? CompareWlValues()
    {
        float leftOpen = Map(wavelengthLeft, mean_openLeft, mean_closeLeft, 0, 1);
        float rightOpen = Map(wavelengthRight, mean_openRight, mean_closeRight, 0, 1);

        if (leftOpen >= 0.3f)
        {
            //Debug.Log("Movendo para a esquerda");
            return false;
        }
        else if (rightOpen >= 0.3f)
        {
            //Debug.Log("Movendo para a direita");
            return true;
        }
        else
        {
            //Debug.Log("Neutro");
            return null;
        }
    }

    float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    void OnApplicationQuit()
    {
        if (tcpNetworkStream != null)
            tcpNetworkStream.Close();
        if (tcpClient != null)
            tcpClient.Close();
    }
}