using MQTTnet.Client;
using mvvmTest.Model;
using mvvmTest.Utilities;
using mvvmTest.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace mvvmTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private MqttServices _mqttService;
        private object _currentPage;
        private readonly object _publishPage = new PublishPage();
        private readonly object _SubscribePage = new SubscribePage();
        public string PublishTopic { get; set; }
        public string PublishMessage { get; set; }
        public string SubscribeTopic { get; set; }
        public ObservableCollection<string> TopicList { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> ReceivedMessages { get; } = new ObservableCollection<string>();
        public object CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        public ICommand ShowPublishPageCommand { get; }
        public ICommand ShowSubscribePageCommand { get; }
        public ICommand PublishCommand { get; }
        public ICommand SubscribeCommand { get; }

        public MainViewModel()
        {
            _mqttService = new MqttServices();
            MqttConnect();
            CurrentPage = _publishPage;
            ShowPublishPageCommand = new RelayCommand(() => CurrentPage = _publishPage);
            ShowSubscribePageCommand = new RelayCommand(() => CurrentPage = _SubscribePage);
            PublishCommand = new RelayCommand(DoPublish);
            SubscribeCommand = new RelayCommand(DoSubscribe);
        }
        public async void MqttConnect()
        {
            try
            {
                await _mqttService.ConnectAsync("broker.hivemq.com");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MQTT Connection Error: {ex.Message}");
            }
        }
        private async void DoPublish()
        {
            if (!string.IsNullOrEmpty(PublishTopic) && !string.IsNullOrEmpty(PublishMessage))
            {
                await _mqttService.PublishAsync(PublishTopic, PublishMessage);
            }
        }
        private async void DoSubscribe()
        {
            if (!string.IsNullOrEmpty(SubscribeTopic))
            {
                await _mqttService.SubscribeAsync(SubscribeTopic);
                TopicList.Add(SubscribeTopic);
            }
        }
        public Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("Message received");
            var receivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var topic = e.ApplicationMessage.Topic;
            Console.WriteLine($"Topic: {topic}, Message: {receivedMessage}");

            return Task.CompletedTask;
        }
    }
}
