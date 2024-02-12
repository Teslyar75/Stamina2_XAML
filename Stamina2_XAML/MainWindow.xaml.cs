using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Stamina2_XAML
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Random random = new Random();
        private Button[] buttons;
        private char?[] sequence;
        private int sequenceLength;
        private int currentIndex = 0;
        private int _sequenceLength;

        private Stopwatch stopwatch;
        private int errorsCount;
        private int charactersTyped;

        public int SequenceLength
        {
            get { return _sequenceLength; }
            set
            {
                if (_sequenceLength != value)
                {
                    _sequenceLength = value;
                    OnPropertyChanged(nameof(SequenceLength));
                }
            }
        }

        public int ErrorsCount
        {
            get { return errorsCount; }
            set
            {
                if (errorsCount != value)
                {
                    errorsCount = value;
                    OnPropertyChanged(nameof(ErrorsCount));
                }
            }
        }

        public int CharactersTyped
        {
            get { return charactersTyped; }
            set
            {
                if (charactersTyped != value)
                {
                    charactersTyped = value;
                    OnPropertyChanged(nameof(CharactersTyped));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeButtons();
            GenerateSequence();
            UpdateSequenceDisplay();
            PickRandomButtonAndChangeColor();
            InitializeStatistics();
        }

        private void InitializeButtons()
        {
            buttons = new Button[]
            {
                button1, button2, button3,
                button4, button5, button6,
                button7, button8, button9,
                button10, button11, button12,
                button13, button14, button15,
                button16, button17, button18,
                button19, button20, button21,
                button22, button23, button24,
                button25, button26, button27,
                button28, button29, button30,
                button31, button32, button33,
                button34, button35, button36,
                button37, button38, button39,
                button40, button41, button42,
                button43, button44, button45,
            };

            foreach (Button button in buttons)
            {
                button.Tag = button.Background;
            }
        }

        private void InitializeStatistics()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        private void GenerateSequence()
        {
            char[] allowedLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int baseSequenceLength = 5;
            int additionalLength = (int)Math.Round(SequenceLength / 2.5);
            sequenceLength = baseSequenceLength + additionalLength + random.Next(1, 10);
            sequence = new char?[sequenceLength];

            for (int i = 0; i < sequenceLength; i++)
            {
                sequence[i] = allowedLetters[random.Next(0, allowedLetters.Length)];
            }
        }

        private void PickRandomButtonAndChangeColor()
        {
            char? currentLetter = sequence[currentIndex];

            foreach (Button button in buttons)
            {
                button.Background = (Brush)button.Tag;
            }

            Button currentButton = buttons.FirstOrDefault(btn => btn.Content?.ToString() == currentLetter?.ToString());
            if (currentButton != null)
            {
                currentButton.Background = Brushes.Red;
            }
        }

        private void UpdateSequenceDisplay()
        {
            sequenceDisplay.Text = " ";
            for (int i = currentIndex; i < sequenceLength; i++)
            {
                sequenceDisplay.Text += sequence[i] + " ";
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (("ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(e.Key.ToString())) && e.Key.ToString().Length == 1)
            {
                char pressedKey = e.Key.ToString()[0];
                CharactersTyped++;

                if (pressedKey == sequence[currentIndex])
                {
                    currentIndex++;
                    sequenceDisplay.Text = sequenceDisplay.Text.Substring(2);

                    if (currentIndex < sequenceLength)
                    {
                        PickRandomButtonAndChangeColor();
                    }
                    else
                    {
                        ProcessSequenceCompletion();
                    }
                }
                else
                {
                    ErrorsCount++;
                }
            }
            else if (e.Key == Key.Escape)
            {
                MessageBoxResult result = MessageBox.Show("Желаете продолжить или закончить?", " Выйти или продолжить", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    this.Close();
                }
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int roundedValue = (int)Math.Round(slider.Value);
            SequenceLength = (roundedValue - 1) * 5 + 5; // Приводим к нужному диапазону
            GenerateSequence();
            UpdateSequenceDisplay();
            PickRandomButtonAndChangeColor();
        }


        private void ProcessSequenceCompletion()
        {
            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            double charactersPerMinute = (CharactersTyped / (double)elapsedMilliseconds) * 60000;

            MessageBoxResult result = MessageBox.Show($"Поздравляем! Вы завершили последовательность!\n" +
                                                      $"Введено символов: {CharactersTyped}\n" +
                                                      $"Ошибки: {ErrorsCount}\n" +
                                                      $"Символов в минуту:: {charactersPerMinute:F2}\n" +
                                                      "Хотите продолжить?", "Последовательность завершена", MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Yes)
            {
                currentIndex = 0;
                GenerateSequence();
                UpdateSequenceDisplay();
                PickRandomButtonAndChangeColor();
                InitializeStatistics();
                ResetStatistics();
            }
            else if (result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
            {
                this.Close();
            }
        }

        private void ResetStatistics()
        {
            ErrorsCount = 0;
            CharactersTyped = 0;
        }
    }
}
