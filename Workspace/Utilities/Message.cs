using Galaxy_Swapper_v2.Workspace.Components;
using System;
using System.Windows;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Message
    {
        public static MessageBoxResult DisplaySTA(string header, string description, MessageBoxButton button = MessageBoxButton.OK, string[] links = null, string[] solutions = null, bool discord = false, bool exit = false)
        {
            MessageBoxResult result = MessageBoxResult.Cancel;
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                var messageView = new CMessageboxControl(header, description, button, links, solutions, discord, exit);
                messageView.ShowDialog();
                result = messageView.Result;
            });
            return result;
        }

        public static MessageBoxResult Display(string header, string description, MessageBoxButton button = MessageBoxButton.OK, string[] links = null, string[] solutions = null, bool discord = false, bool exit = false)
        {
            var messageView = new CMessageboxControl(header, description, button, links, solutions, discord, exit);
            messageView.ShowDialog();
            return messageView.Result;
        }
    }
}
