import paho.mqtt.client as mqtt

# ---------- MQTT Setup ----------

MQTT_BROKER = "localhost"
MQTT_PORT = 1883
MQTT_TOPIC = "myo/predicao"

# Função que será chamada quando o cliente se conectar ao broker
def on_connect(client, userdata, flags, rc):
    print("Conectado com sucesso ao broker. Código de resposta:", rc)
    client.subscribe(MQTT_TOPIC)  # Se inscreve no tópico 'myo/predicao'

# Função que será chamada quando uma mensagem for recebida
def on_message(client, userdata, msg):
    print(f"Mensagem recebida no tópico {msg.topic}: {msg.payload.decode()}")

# Função principal
def start_mqtt_client():
    # Cria uma instância do cliente MQTT
    client = mqtt.Client()

    # Define as funções de callback
    client.on_connect = on_connect
    client.on_message = on_message

    # Conecta ao broker MQTT
    client.connect(MQTT_BROKER, MQTT_PORT, 60)

    # Inicia o loop para processar mensagens recebidas
    client.loop_forever()

# ---------- Execução ----------
if __name__ == '__main__':
    start_mqtt_client()
