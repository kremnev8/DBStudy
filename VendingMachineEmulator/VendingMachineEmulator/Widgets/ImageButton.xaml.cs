using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VendingMachineEmulator.Util;

namespace VendingMachineEmulator
{
    /// <summary>
    /// Логика взаимодействия для ImageButton.xaml
    /// </summary>
    public partial class ImageButton : UserControl
    {
        public static readonly DependencyProperty ButtonImageUrlProperty = DependencyProperty.Register
        ("ImageSrc", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageButton;
            control.Icon.Source = control.ImageSrc;
        }

        public ImageSource ImageSrc
        {
            get => (ImageSource)GetValue(ButtonImageUrlProperty);
            set => SetValue(ButtonImageUrlProperty, value);
        }

        public int buttonIndex;
        
        public ImageButton()
        {
            InitializeComponent();
        }

        public ImageButton(int buttonIndex)
        {
            InitializeComponent();
            this.buttonIndex = buttonIndex;
        }

        public void SetTint(bool enabled)
        {
            Tint.OpacityMask = new ImageBrush()
            {
                ImageSource = Icon.Source
            };
            Tint.Visibility = enabled ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
