using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VendingMachineEmulator.Models;
using VendingMachineEmulator.Util;

namespace VendingMachineEmulator
{

    public partial class ItemButton : UserControl
    {
        public int slotIndex;
        public Good good;
        public bool phantom;
        
        public ItemButton(int slotIndex)
        {
            InitializeComponent();
            this.slotIndex = slotIndex;
        }

        public void SetGood(Good value, int goodCount, bool _phantom = false)
        {
            good = value;
            Text.Text = $"{slotIndex}    {good.SaleCost}";
            Button.ToolTip = good.Name;
            Count.Text = goodCount.ToString();
            ToolTipService.SetIsEnabled(Button, true);
            
            var fullFilePath = $"{MainWindow.backEndUrl}/Images/{good.IconPath}";
            BitmapImage image = ImageIconHelper.GetImage(fullFilePath);

            Icon.Source = image;
            Tint.OpacityMask = new ImageBrush()
            {
                ImageSource = image
            };
            
            
            Icon.Visibility = Visibility.Visible;
            phantom = _phantom;
            if (phantom)
            {
                Tint.Visibility = Visibility.Visible;
            }
            else
            {
                Tint.Visibility = Visibility.Hidden;
            }
        }

        public void UpdateCount(int goodCount)
        {
            Count.Text = goodCount.ToString(); 
        }
        

        public void Clear()
        {
            good = null;
            Text.Text = "";
            Icon.Visibility = Visibility.Hidden;
            Button.ToolTip = "";
            Count.Text = "";
            ToolTipService.SetIsEnabled(Button, false);
            Tint.Visibility = Visibility.Hidden;
        }
    }
}