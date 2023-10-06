using mvvmTest.Utilities;
using mvvmTest.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mvvmTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentPage;

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

        public MainViewModel()
        {
            CurrentPage = new PublishPage();
            ShowPublishPageCommand = new RelayCommand(() => CurrentPage = new PublishPage());
            ShowSubscribePageCommand = new RelayCommand(() => CurrentPage = new SubscribePage());
        }
    }
}
