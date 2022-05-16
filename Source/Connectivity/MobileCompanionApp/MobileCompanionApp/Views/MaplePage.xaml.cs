using Xamarin.Forms;

namespace MobileCompanionApp
{
    public partial class MaplePage : ContentPage
    {
        public MaplePage()
        {
            InitializeComponent();
            BindingContext = new MapleViewModel();
        }
    }
}