using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CTabControl.xaml
    /// </summary>
    public partial class CTabControl : UserControl
    {
        private bool isSelected = false;

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(CTabControl));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconClickedProperty =
            DependencyProperty.Register(nameof(IconClicked), typeof(ImageSource), typeof(CTabControl));

        public ImageSource IconClicked
        {
            get => (ImageSource)GetValue(IconClickedProperty);
            set => SetValue(IconClickedProperty, value);
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Brush), typeof(CTabControl), new PropertyMetadata(Properties.Colors.External_Brush));

        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty PresenceProperty =
            DependencyProperty.Register(nameof(Presence), typeof(string), typeof(CNewsControl));

        public string Presence
        {
            get => (string)GetValue(PresenceProperty);
            set => SetValue(PresenceProperty, value);
        }

        public CTabControl()
        {
            InitializeComponent();
        }

        public void Tab_Default()
        {
            Clicked.Visibility = Visibility.Hidden;
            Default.Visibility = Visibility.Visible;
            Color = Properties.Colors.External_Brush;
            isSelected = false;
            AnimateSize(40, 8);
        }

        public void Tab_Clicked()
        {
            Default.Visibility = Visibility.Hidden;
            Clicked.Visibility = Visibility.Visible;

            var colorAnimation = new ColorAnimation
            {
                From = ((SolidColorBrush)Properties.Colors.External_Brush).Color,
                To = ((SolidColorBrush)Properties.Colors.Blue_Brush).Color,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var brush = new SolidColorBrush();
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

            Color = brush;

            Utilities.Presence.Update(Presence);
            AnimateSize(45, 7);
            isSelected = true;
        }

        private void CTabControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateSize(45, 7);
        }

        private void CTabControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isSelected)
            {
                AnimateSize(40, 8);
            }
        }

        private void AnimateSize(int size, int margin)
        {
            var sizeAnimation = new DoubleAnimation(size, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            var marginAnimation = new ThicknessAnimation(new Thickness(margin), TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            this.BeginAnimation(WidthProperty, sizeAnimation);
            this.BeginAnimation(HeightProperty, sizeAnimation);
            this.BeginAnimation(MarginProperty, marginAnimation);
        }
    }
}
