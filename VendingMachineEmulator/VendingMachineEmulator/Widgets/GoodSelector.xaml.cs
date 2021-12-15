using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VendingMachineEmulator.Models;
using VendingMachineEmulator.Util;

namespace VendingMachineEmulator
{
    public partial class GoodSelector : UserControl
    {
        private static List<Good> goods;

        public int currentGoodIndex;
        public int currentSlotPosition;

        public bool lockGood;

        public GoodSelector()
        {
            InitializeComponent();
            OkButton.Click += OnOKButtonPressed;
            CloseButton.Click += OnCloseButtonClick;
            Visibility = Visibility.Hidden;
            DetailPanel.Visibility = Visibility.Hidden;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs args)
        {
            Hide();
        }

        public void Show(int slotPosition)
        {
            currentSlotPosition = slotPosition;
            Visibility = Visibility.Visible;
            DetailPanel.Visibility = Visibility.Hidden;
            LockGood(false);
        }

        public void Show(Good good, int slotPosition, bool _lock = false)
        {
            Show(good, slotPosition, 0, _lock);
        }

        public void Show(Good good, int slotPosition, int currentCount, bool _lock)
        {
            currentSlotPosition = slotPosition;
            for (int i = 0; i < goods.Count; i++)
            {
                Good good1 = goods[i];
                if (good.GoodId == good1.GoodId)
                {
                    currentGoodIndex = i;
                    ShowPanel(good);
                }
            }

            AddCountField.Text = currentCount.ToString();
            Visibility = Visibility.Visible;
            LockGood(_lock);
        }

        public void LockGood(bool _lock)
        {
            foreach (var items in Goods.Children)
            {
                ImageButton button = items as ImageButton;
                button.IsEnabled = !_lock;
                button.SetTint(_lock);
            }

            lockGood = _lock;
        }


        public void Hide()
        {
            Visibility = Visibility.Hidden;
            DetailPanel.Visibility = Visibility.Hidden;
        }

        public async Task LoadGoodsAsync()
        {
            List<Good> result = await MainWindow.client.GetJsonAsync<List<Good>>($"{MainWindow.backEndUrl}/api/Good/Get");
            goods = result;

            for (int i = 0; i < goods.Count; i++)
            {
                int index = i;
                Dispatcher.Invoke(() =>
                {
                    Good good = goods[index];
                    int x = index % 4;
                    int y = index / 4;

                    if (Goods.RowDefinitions.Count <= y)
                    {
                        Goods.RowDefinitions.Add(new RowDefinition());
                    }

                    ImageButton newControl = new ImageButton(index);
                    MainWindow.console.Log($"{MainWindow.backEndUrl}/Images/{good.IconPath}");
                    newControl.Icon.SetImage($"{MainWindow.backEndUrl}/Images/{good.IconPath}");
                    newControl.Button.Click += OnGoodButtonPressed;

                    Grid.SetColumn(newControl, x);
                    Grid.SetRow(newControl, y);
                    Goods.Children.Add(newControl);
                });
                await Task.Delay(50);
            }
        }

        private void OnGoodButtonPressed(object sender, RoutedEventArgs e)
        {
            if (lockGood) return;

            Button source = e.OriginalSource as Button;
            ImageButton button = source.Parent as ImageButton;

            if (button.buttonIndex < goods.Count)
            {
                Good good = goods[button.buttonIndex];
                currentGoodIndex = button.buttonIndex;
                ShowPanel(good);
            }
        }

        private void ShowPanel(Good good)
        {
            GoodIcon.SetImage($"{MainWindow.backEndUrl}/Images/{good.IconPath}");
            GoodName.Text = good.Name;
            GoodCost.Content = $"Покупка: {good.PurchaseCost}   Продажа: {good.SaleCost}";
            DetailPanel.Visibility = Visibility.Visible;
        }

        private void OnOKButtonPressed(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = int.Parse(AddCountField.Text);
                if (count > 0)
                {
                    Good good = goods[currentGoodIndex];
                    MainWindow.instance.AddGood(good, currentSlotPosition, count);
                    MainWindow.console.Log($"Adding good {good.GoodId}, {good.Name}, {count}");
                    Hide();
                }
                else if (count == 0)
                {
                    MainWindow.instance.ClearSlot(currentSlotPosition);
                    Hide();
                }
            }
            catch (FormatException exception)
            {
                MainWindow.console.LogError(exception.Message);
            }
        }


        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PreviewText(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String) e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}