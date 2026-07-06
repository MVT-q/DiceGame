using Dice_game.ViewModels;

namespace Dice_game
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }       
    }
}
