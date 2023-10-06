using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mvvmTest.Model
{
    public class MqttServices
    {
        private IMqttClient _mqttClient;
        private MqttClientOptions _mqttOptions;
        public MqttServices()
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }
        public async Task ConnectAsync()
        {
            // 연결 옵션 설정
            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("200.100.10.11", 1883)
                .Build();

            // 브로커 연결
            await _mqttClient.ConnectAsync(_mqttOptions);
        }
    }
}
