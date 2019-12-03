using Prism.Ioc;
using Xamarin.Forms;
using Prism.Unity;
using Prism;
using Sample.Views;
using Sample.ViewModels;
using Sample.Models;

namespace Sample
{
    public partial class App:PrismApplication
    {
        public App(IPlatformInitializer initializer):base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var result = await NavigationService.NavigateAsync("NavigationPage/MainPage");

            if (!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();

            containerRegistry.Register<IWebApi, WebApi>();
        }
    }
}
