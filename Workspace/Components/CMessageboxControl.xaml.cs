using Galaxy_Swapper_v2.Workspace.Utilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Galaxy_Swapper_v2.Workspace.Components
{
    /// <summary>
    /// Interaction logic for CMessageboxControl.xaml
    /// </summary>
    public partial class CMessageboxControl : Window
    {
        public MessageBoxResult Result { get; set; } = default!;
        private MessageBoxButton ButtonType { get; set; } = default!;
        private List<string> Socials { get; set; } = default!;
        private List<string> Solutions { get; set; } = default!;
        private bool ShouldClose = false;
        public CMessageboxControl(string header, string description, MessageBoxButton buttontype, List<string> socials = null, List<string> solutions = null, bool close = false)
        {
            InitializeComponent();
            ButtonType = buttontype;
            Header.Text = header;
            Description.Text = description;
            Socials = socials;
            Solutions = solutions;
            ShouldClose = close;
        }

        private void CMessageboxControl_Loaded(object sender, RoutedEventArgs e)
        {
            switch (ButtonType)
            {
                case MessageBoxButton.OK:
                    Ok.IsEnabled = true;
                    Ok.Visibility = Visibility.Visible;
                    CloseButton.IsEnabled = true;
                    break;
                case MessageBoxButton.YesNo:
                    Yes.IsEnabled = true;
                    Yes.Visibility = Visibility.Visible;
                    No.IsEnabled = true;
                    No.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    Yes.IsEnabled = true;
                    Yes.Visibility = Visibility.Visible;
                    No.IsEnabled = true;
                    No.Visibility = Visibility.Visible;
                    Cancel.IsEnabled = true;
                    Cancel.Visibility = Visibility.Visible;
                    break;
            }

            if (Solutions != null)
            {
                //Need to make lan
                Description.Text += "\n\nPlease try the following solutions";
                foreach (string Solution in Solutions)
                {
                    Description.Text += $"\n・{Solution}";
                }
            }

            System.Media.SystemSounds.Beep.Play();
        }

        private void Drag_Click(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void Close_Click(object sender, MouseButtonEventArgs e) => MessageClose();
        private void MessageClose()
        {
            if (Socials != null)
            {
                foreach (string Social in Socials)
                {
                    if (string.IsNullOrEmpty(Social))
                        continue;

                    Social.UrlStart();
                }
            }

            if (ShouldClose)
                Environment.Exit(0);
            else
                Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            MessageClose();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            MessageClose();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            MessageClose();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            MessageClose();
        }
    }
}