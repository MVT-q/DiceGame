using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dice_game.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private int maxRounds;
        public int MaxRounds
        {
            get => maxRounds;
            set { maxRounds = value; OnPropertyChanged(); }
        }

        private bool useBonuses;
        public bool UseBonuses
        {
            get => useBonuses;
            set { useBonuses = value; OnPropertyChanged(); }
        }

        private bool showHistory;
        public bool ShowHistory
        {
            get => showHistory;
            set { showHistory = value; OnPropertyChanged(); }
        }

        private bool isTwoPlayer;
        public bool IsTwoPlayer
        {
            get => isTwoPlayer;
            set { isTwoPlayer = value; OnPropertyChanged(); }
        }

        public ICommand ApplyCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand ShowBonusInfoCommand { get; }

        private readonly Action<SettingsViewModel> applyAction;
        private readonly Func<Task> closeAction;

        public SettingsViewModel(Action<SettingsViewModel> apply, Func<Task> close)
        {
            applyAction = apply;
            closeAction = close;

            ApplyCommand = new Command(async () =>
            {
                applyAction(this);
                await closeAction();
            });

            ResetCommand = new Command(() =>
            {
                MaxRounds = 5;
                UseBonuses = false;
                ShowHistory = false;
                IsTwoPlayer = false;
            });

            CloseCommand = new Command(async () =>
            {
                await closeAction();
            });

            ShowBonusInfoCommand = new Command(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Bonus rules", "Double → +5 points\n(1+1) → total reset", "OK");
            });
        }
    }
}
