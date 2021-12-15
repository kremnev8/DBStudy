using System;
using System.Windows;
using System.Windows.Controls;

namespace VendingMachineEmulator
{
    public partial class KeyPad : UserControl
    {
        string currentNumber = "";

        public Action<string> onInputChanged;
        public Action<int> onItemSelected;
        public Action onInvalidInput;

        
        public KeyPad()
        {
            InitializeComponent();
            
            foreach (UIElement c in Buttons.Children)
            {
                if (c is Button button)
                {
                    button.Click += NumberPanel_Click;
                }
            }
        }


        private void NumberPanel_Click(object sender, RoutedEventArgs e)
        {
            
            string s = (string) ((Button) e.OriginalSource).Content;

            switch (s)
            {
                case "CLEAR":
                    currentNumber = "";
                    break;
                case "OK":
                    if (currentNumber.Length > 0)
                    {
                        onItemSelected?.Invoke(int.Parse(currentNumber));
                        currentNumber = "";
                    }
                    else
                    {
                        onInvalidInput?.Invoke();
                    }
                    
                    currentNumber = "";
                    break;
                default:
                    if (currentNumber.Length < 2)
                    {
                        currentNumber += s;
                    }

                    break;
            }
            onInputChanged?.Invoke(currentNumber);
        }
    }
}