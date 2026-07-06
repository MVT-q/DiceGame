using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice_game.Controls
{
    public class DiceView : Frame
    {
        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var dice = (DiceView)bindable;
            dice.SetValue((int)newValue);
        }

        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(int), typeof(DiceView), 1, propertyChanged: OnValueChanged);

        private readonly BoxView[,] pips = new BoxView[3, 3];

        public DiceView()
        {
            WidthRequest = 100;
            HeightRequest = 100;
            Padding = 10;
            CornerRadius = 12;
            BorderColor = Colors.Black;
            BackgroundColor = Colors.White;

            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Star)
                },

                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Star)
                }
            };

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var pip = new BoxView
                    {
                        Color = Colors.Black,
                        WidthRequest = 14,
                        HeightRequest = 14,
                        CornerRadius = 7,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        IsVisible = false
                    };

                    pips[row, col] = pip;
                    grid.Add(pip, col, row);
                }
            }
            Content = grid;

            SetValue(1);
        }

        public void SetValue(int value)
        {
            HideAll();
            switch (value)
            {
                case 1:
                    Show(1, 1);
                    break;

                case 2:
                    Show(0, 0);
                    Show(2, 2);
                    break;

                case 3:
                    Show(0, 0);
                    Show(1, 1);
                    Show(2, 2);
                    break;

                case 4:
                    Show(0, 0);
                    Show(2, 0);
                    Show(0, 2);
                    Show(2, 2);
                    break;

                case 5:
                    Show(0, 0);
                    Show(2, 0);
                    Show(1, 1);
                    Show(0, 2);
                    Show(2, 2);
                    break;

                case 6:
                    Show(0, 0);
                    Show(1, 0);
                    Show(2, 0);
                    Show(0, 2);
                    Show(1, 2);
                    Show(2, 2);
                    break;
            }
        }

        private void HideAll()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    pips[row, col].IsVisible = false;
                }
            }
        }

        private void Show(int row, int col)
        {
            pips[row, col].IsVisible = true;
        }
    }
}
