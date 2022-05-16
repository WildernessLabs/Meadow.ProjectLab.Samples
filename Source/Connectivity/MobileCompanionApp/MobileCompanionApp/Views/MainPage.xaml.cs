using Xamarin.Forms;

namespace MobileCompanionApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void BtnBluetoothClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new BluetoothPage()
            {
                Title = "Bluetooth Connection"
            });
        }

        void BtnMapleClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MaplePage()
            {
                Title = "Maple Connection"
            });
        }
    }
}