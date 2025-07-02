using System.Threading.Tasks;
using UnityEngine;
using MQTTnet;
using MQTTnet.Server;
using System.Text;

public class MqttBrokerBehaviour : MonoBehaviour
{
    public static MqttBrokerBehaviour instance;

    public int HandState { get; private set; } = 0; // Inicia como 0 (mão aberta)

    private IMqttServer _mqttServer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    async void Start()
    {
        if (_mqttServer == null)
        {
            await StartMqttBroker();
        }
    }

    async void OnApplicationQuit()
    {
        await StopMqttBroker();
    }


    private async Task StartMqttBroker()
    {
        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Any)
            .WithDefaultEndpointPort(1883)
            .WithConnectionValidator(context =>
            {
                Debug.Log($"Client '{context.ClientId}' está tentando se conectar...");
            })
            .WithApplicationMessageInterceptor(context =>
            {
                var payload = context.ApplicationMessage.Payload != null ? Encoding.UTF8.GetString(context.ApplicationMessage.Payload) : "vazio";
                Debug.Log($"<color=green>Mensagem recebida do cliente '{context.ClientId}': {payload}</color>");
                if (int.TryParse(payload, out int state))
                {
                    HandState = state;
                }
                
                context.AcceptPublish = true;
            })
            .Build();

        var mqttFactory = new MqttFactory();
        _mqttServer = mqttFactory.CreateMqttServer();
        await _mqttServer.StartAsync(options);
        Debug.Log("✅ MQTT Broker started.");
    }

    private async Task StopMqttBroker()
    {
        if (_mqttServer != null)
        {
            await _mqttServer.StopAsync();
            Debug.Log("🛑 MQTT Broker stopped.");
        }
    }
}