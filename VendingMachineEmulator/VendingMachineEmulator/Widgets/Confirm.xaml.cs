using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VendingMachineEmulator
{
    /// <summary>
    /// Логика взаимодействия для Confirm.xaml
    /// </summary>
    public partial class Confirm : UserControl
    {
        public Action cancelCallback;
        public Action confirmCallback;
        
        public Confirm()
        {
            InitializeComponent();
            CancelButton.Click += OnCancelClicked;
            OKButton.Click += OnConfirmClicked;
        }

        public void Popup(string question, Action cancel, Action confirm)
        {
            QuestionText.Text = question;
            cancelCallback = cancel;
            confirmCallback = confirm;
            Visibility = Visibility.Visible;
        }

        private void OnConfirmClicked(object sender, RoutedEventArgs e)
        {
            confirmCallback?.Invoke();
            Visibility = Visibility.Hidden;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            cancelCallback?.Invoke();
            Visibility = Visibility.Hidden;
        }
    }
}
