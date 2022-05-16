using Xamarin.Forms;

namespace MobileCompanionApp
{
    public partial class BluetoothPage : ContentPage
    {
        public BluetoothPage()
        {
            InitializeComponent();
            BindingContext = new BluetoothViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BluetoothViewModel).CmdSearchForDevices.Execute(null);
        }
    }
}