using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dice_game.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int round = 1;
        private int totalScore = 0;
        private readonly Random random = new Random();

        public ObservableCollection<string> History { get; } = new();

        public string CurrentPlayerText => $"Player {CurrentPlayer}";

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private int dice1;
        public int Dice1
        {
            get => dice1;
            set { dice1 = value; OnPropertyChanged(); }
        }

        private int dice2;
        public int Dice2
        {
            get => dice2;
            set { dice2 = value; OnPropertyChanged(); }
        }

        private string sum = "Sum: 0";
        public string Sum
        {
            get => sum;
            set { sum = value; OnPropertyChanged(); }
        }

        private string total = "Total: 0";
        public string Total
        {
            get => total;
            set { total = value; OnPropertyChanged(); }
        }

        private string roundText = "Round 0/5";
        public string RoundText
        {
            get => roundText;
            set { roundText = value; OnPropertyChanged(); }
        }

        private string message;
        public string Message
        {
            get => message;
            set { message = value; OnPropertyChanged(); }
        }

        private bool isRollEnabled = true;
        public bool IsRollEnabled
        {
            get => isRollEnabled;
            set { isRollEnabled = value; OnPropertyChanged(); }
        }

        private int maxRounds = 5;
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

        private bool showHistory = false;
        public bool ShowHistory
        {
            get => showHistory;
            set { showHistory = value; OnPropertyChanged(); }
        }

        private bool isTwoPlayer = false;
        public bool IsTwoPlayer
        {
            get => isTwoPlayer;
            set { isTwoPlayer = value; OnPropertyChanged(); }
        }

        private int currentPlayer = 1;
        public int CurrentPlayer
        {
            get => currentPlayer;
            set { currentPlayer = value; OnPropertyChanged(); OnPropertyChanged(nameof(CurrentPlayerText)); }
        }

        private int player1Score;
        public int Player1Score
        {
            get => player1Score;
            set { player1Score = value; OnPropertyChanged(); }
        }

        private int player2Score;
        public int Player2Score
        {
            get => player2Score;
            set { player2Score = value; OnPropertyChanged(); }
        }

        private Color resultColor = Colors.Black;
        public Color ResultColor
        {
            get => resultColor;
            set { resultColor = value; OnPropertyChanged(); }
        }

        private string resultStatus;
        public string ResultStatus
        {
            get => resultStatus;
            set { resultStatus = value; OnPropertyChanged(); }
        }

        public ICommand RollCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public MainViewModel()
        {
            RollCommand = new Command(Roll);
            NewGameCommand = new Command(NewGame);
            OpenSettingsCommand = new Command(async () => await OpenSettings());
            Reset();
        }

        private async void Roll()
        {
            if(round > MaxRounds)
            {
                Message = "Game over. Start a new";
                return;
            }

            IsRollEnabled = false;

            await Animate();

            Dice1 = random.Next(1, 7);
            Dice2 = random.Next(1, 7);

            int sumValue = Dice1 + Dice2;

            ResultStatus = "";
            ResultColor = Colors.Black;

            if (UseBonuses)
            {
                if (Dice1 == 1 && Dice2 == 1)
                {
                    sumValue = 0;

                    if (IsTwoPlayer)
                    {
                        if (CurrentPlayer == 1)
                            Player1Score = 0;
                        else
                            Player2Score = 0;
                    }
                    else
                    {
                        totalScore = 0;
                    }

                    ResultStatus = "Total reset!";
                    ResultColor = Colors.Red;
                }
                else if (Dice1 == Dice2)
                {
                    sumValue += 5;
                    ResultStatus = "Double! +5 bonus";
                    ResultColor = Colors.Green;
                }
            }

            if (IsTwoPlayer)
            {
                if (CurrentPlayer == 1)
                    Player1Score += sumValue;
                else
                    Player2Score += sumValue;
            }
            else
            {
                totalScore += sumValue;
                Total = $"Total: {totalScore}";
            }

            Sum = $"Sum: {sumValue}";
            RoundText = $"Round {round}/{MaxRounds}";

            if (IsTwoPlayer)
                History.Add($"Round {round} | Player{CurrentPlayer}: {Dice1} + {Dice2} = {sumValue}");
            else
                History.Add($"Round {round}: {Dice1} + {Dice2} = {sumValue}");

            if (IsTwoPlayer)
            {
                if (CurrentPlayer == 2)
                    round++;
            }
            else
                round++;

            if (round > MaxRounds)
            {
                IsRollEnabled = false;

                if (IsTwoPlayer)
                {
                    if (Player1Score > Player2Score)
                        Message = "Player 1 wins!";
                    else if (Player2Score > Player1Score)
                        Message = "Player 2 wins!";
                    else
                        Message = "Draw!";
                }
                else
                    Message = "Game finished.";
            }
            else
                IsRollEnabled = true;

            if (IsTwoPlayer)
                CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
        }

        private async Task Animate()
        {
            int delay = 30;

            for (int i = 0; i < 12; i++)
            {
                Dice1 = random.Next(1, 7);
                Dice2 = random.Next(1, 7);
                await Task.Delay(delay);
                delay += 10;
            }
        }

        private void NewGame()
        {
            Reset();
        }

        private void Reset()
        {
            round = 1;
            totalScore = 0;

            Dice1 = 1;
            Dice2 = 1;

            Sum = "Sum: 0";
            Total = $"Total: {totalScore}";
            RoundText = $"Round {round}/{MaxRounds}";
            Message = "";

            IsRollEnabled = true;
            History.Clear();

            Player1Score = 0;
            Player2Score = 0;
            CurrentPlayer = 1;

            ResultStatus = "";
            ResultColor = Colors.Black;
        }

        private async Task OpenSettings()
        {
            SettingsPage page = null;

            var settingsVm = new SettingsViewModel(
                apply: vm =>
                {
                    MaxRounds = vm.MaxRounds;
                    UseBonuses = vm.UseBonuses;
                    ShowHistory = vm.ShowHistory;
                    IsTwoPlayer = vm.IsTwoPlayer;
                    NewGameCommand.Execute(null);
                },
                close: async () =>
                {
                    await page.Navigation.PopModalAsync();
                });

            settingsVm.MaxRounds = MaxRounds;
            settingsVm.UseBonuses = UseBonuses;
            settingsVm.ShowHistory = ShowHistory;
            settingsVm.IsTwoPlayer = IsTwoPlayer;

            page = new SettingsPage(settingsVm);

            await Application.Current.MainPage.Navigation
                .PushModalAsync(page);
        }
    }
}
