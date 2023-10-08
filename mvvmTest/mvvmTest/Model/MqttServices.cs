using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Server;

namespace mvvmTest.Model
{
    public class MqttServices
    {
        private MqttFactory mqttFactory;
        private IMqttClient _mqttClient;
        private MqttClientOptions _mqttOptions;
        public MqttServices()
        {
            mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
        }
        public async Task ConnectAsync(string host, int port)
        {
            // 연결 옵션 설정
            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .Build();

            // 브로커 연결
            await _mqttClient.ConnectAsync(_mqttOptions);
            Console.WriteLine("MQTT Connected");
        }
        public async Task ConnectAsync(string host)
        {
            // 연결 옵션 설정
            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(host)
                .Build();

            // 브로커 연결
            await _mqttClient.ConnectAsync(_mqttOptions);
            Console.WriteLine("MQTT Connected");
        }

        public async Task PublishAsync(string topic, string payload)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
            .Build();

            await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
            Console.WriteLine("payload published");
        }
        public async Task SubscribeAsync(string topic)
        {
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f =>
                {
                    f.WithTopic(topic);
                })
            .Build();

            await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topic.");
        }
       
    }
}
