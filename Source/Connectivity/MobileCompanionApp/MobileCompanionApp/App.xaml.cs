using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCompanionApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
                { BarTextColor = Color.White, BarBackgroundColor = (Color)Current.Resources["ButtonActive"] };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
