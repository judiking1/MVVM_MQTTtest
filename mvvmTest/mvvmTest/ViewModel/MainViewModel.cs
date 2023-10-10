using MQTTnet.Client;
using MQTTnet.Server;
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
        private readonly MqttServices _mqttService;
        private readonly object _publishPage;
        private readonly object _subscribePage;
        private object _currentPage;
        private string _selectedTopic;
        private Dictionary<string, ObservableCollection<string>> messagesByTopic = new Dictionary<string, ObservableCollection<string>>();
        private ObservableCollection<string> _selectedTopicMessages;

        public MainViewModel()
        {
            _mqttService = new MqttServices();
            _publishPage = new PublishPage();
            _subscribePage = new SubscribePage();
            CurrentPage = _publishPage;
            SetupCommands();
            SetupMqttService();
            MqttConnect();
        }

        public string PublishTopic { get; set; }
        public string PublishMessage { get; set; }
        public string SubscribeTopic { get; set; }
        public ObservableCollection<string> TopicList { get; } = new ObservableCollection<string>();
        public string SelectedTopic
        {
            get { return _selectedTopic;}
            set 
            { 
                _selectedTopic = value;
                CurrentTopicMessages = messagesByTopic[_selectedTopic];
                OnPropertyChanged(nameof(SelectedTopic));
            }
        }
        public ObservableCollection<string> CurrentTopicMessages
        {
            get => _selectedTopicMessages;
            set
            {
                _selectedTopicMessages = value;
                OnPropertyChanged(nameof(CurrentTopicMessages));
            }
        }
        public object CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        public ICommand ShowPublishPageCommand { get; private set; }
        public ICommand ShowSubscribePageCommand { get; private set; }
        public ICommand PublishCommand { get; private set; }
        public ICommand SubscribeCommand { get; private set; }
        private void SetupCommands()
        {
            ShowPublishPageCommand = new RelayCommand(() => CurrentPage = _publishPage);
            ShowSubscribePageCommand = new RelayCommand(() => CurrentPage = _subscribePage);
            PublishCommand = new RelayCommand(DoPublish);
            SubscribeCommand = new RelayCommand(DoSubscribe);
        }
        private void SetupMqttService()
        {
            _mqttService.MessageReceived += (topic, message) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (!messagesByTopic.ContainsKey(topic))
                    {
                        messagesByTopic[topic] = new ObservableCollection<string>();
                        TopicList.Add(topic);
                    }
                    messagesByTopic[topic].Add(message);
                    Console.WriteLine($"Topic: {topic}, Message: {message}");
                });
            };
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
                if (!messagesByTopic.ContainsKey(SubscribeTopic))
                {
                    await _mqttService.SubscribeAsync(SubscribeTopic);
                    messagesByTopic[SubscribeTopic] = new ObservableCollection<string>();
                    TopicList.Add(SubscribeTopic);
                }
            }
        }
    }
}
