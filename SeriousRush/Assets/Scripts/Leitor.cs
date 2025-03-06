using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using MicronOptics.Hyperion.Communication;
using UnityEngine.UI;
using System.IO;

public class Leitor : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream tcpNetworkStream;
    private SpectrumData spectrumData;

    public int channelLeft = 1;
    public int channelRight = 2;

    public float wavelengthLeft = 0.0f;
    public float wavelengthRight = 0.0f;

    public float mean_openLeft = 0.0f;
    public float mean_closeLeft = 0.0f;

    public float mean_openRight = 0.0f;
    public float mean_closeRight = 0.0f;

    public bool? moveRight = null; // true = direita, false = esquerda, null = neutro

    //public float neutralThreshold = 0.05f; // Ajuste para definir quando eh neutro

    private List<float> wavelengthHistoryLeft = new List<float>();
    private List<float> wavelengthHistoryRight = new List<float>();

    void Start()
    {
        string instrumentIpAddress = "10.0.0.55";

        try
        {
            // Inicia a conex�o tcp
            tcpClient = new TcpClient();
            tcpClient.Connect(instrumentIpAddress, Command.TcpPort);
            tcpNetworkStream = tcpClient.GetStream();

            CommandResponse response = Command.Execute(tcpNetworkStream, CommandOptions.None, CommandName.GetSpectrum);
            spectrumData = response.AsSpectrumData();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro na conex�o TCP: {ex.Message}");
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

        moveRight = CompareWlValues();
    }

    public bool? GetDirection()
    {
        return moveRight;
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
        float diffLeft = Mathf.Abs(wavelengthLeft - mean_openLeft) - Mathf.Abs(wavelengthLeft - mean_closeLeft);
        float diffRight = Mathf.Abs(wavelengthRight - mean_openRight) - Mathf.Abs(wavelengthRight - mean_closeRight);

        //float diffTotal = Mathf.Abs(diffRight - diffLeft);

        /*if (diffTotal < neutralThreshold)
        {
            Debug.Log("Neutro");
            return null; // Estado neutro
        }
        */
        if (diffLeft > diffRight){
            Debug.Log("mov esquerda");
            return false; // esquerda
        }
        else if (diffRight > diffLeft){
            Debug.Log("mov direita");
            return true; // direita
        }
        else
        {
            Debug.Log("Neutro");
            return null; // Estado neutro
        }
    }

    void OnApplicationQuit()
    {
        if (tcpNetworkStream != null)
            tcpNetworkStream.Close();
        if (tcpClient != null)
            tcpClient.Close();
    }
}
