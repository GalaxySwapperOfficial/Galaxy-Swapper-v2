using Galaxy_Swapper_v2.Workspace.Components;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Galaxy_Swapper_v2.Workspace.Utilities
{
    public static class Message
    {
        public static MessageBoxResult DisplaySTA(string header, string description, MessageBoxButton buttontype, List<string> socials = null, List<string> solutions = null, bool close = false)
        {
            var Result = MessageBoxResult.Cancel;
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                var NMessage = new CMessageboxControl(header, description, buttontype, socials, solutions, close);
                NMessage.ShowDialog();

                Result = NMessage.Result;
            });

            return Result;
        }

        public static MessageBoxResult Display(string header, string description, MessageBoxButton buttontype, List<string> socials = null, List<string> solutions = null, bool close = false)
        {
            var NMessage = new CMessageboxControl(header, description, buttontype, socials, solutions, close);
            NMessage.ShowDialog();
            return NMessage.Result;
        }
    }
}
