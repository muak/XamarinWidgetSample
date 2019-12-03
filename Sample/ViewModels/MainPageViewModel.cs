using System;
using Prism.Mvvm;
using Reactive.Bindings;
using Sample.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Prism.Navigation;

namespace Sample.ViewModels
{
    public class MainPageViewModel:BindableBase,INavigatedAware
    {
        public ObservableCollection<WebBook> Books { get; set; }


        IWebApi _webApi;
        public MainPageViewModel(IWebApi webApi)
        {
            _webApi = webApi;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            Books = new ObservableCollection<WebBook>(await _webApi.GetByKeyword("Xamarin", 30, 0));

            RaisePropertyChanged(nameof(Books));
        }
    }
}
