/*Automated Rodent Tracker - A program to automatically track rodents
Copyright(C) 2015 Brett Michael Hewitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<http://www.gnu.org/licenses/>.*/

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace AutomatedRodentTracker.Behaviours
{
    public static class WindowClosingBehavior
    {
        public static readonly DependencyProperty CloseProperty = 
            DependencyProperty.RegisterAttached("Close", 
            typeof(bool), typeof(WindowClosingBehavior),
            new UIPropertyMetadata(false, OnCloseChanged));

        public static bool GetClose(DependencyObject source)
        {
            return (bool) source.GetValue(CloseProperty);
        }

        public static void SetClose(DependencyObject source, bool value)
        {
            source.SetValue(CloseProperty, value);
        }

        private static void OnCloseChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var win = sender as Window;

            if (win == null)
            {
                return;
            }

            bool closing = (e.NewValue is bool) && (bool)e.NewValue;
            if (closing)
            {
                win.Close();
            }
        }

        public static readonly DependencyProperty ClosingProperty =
            DependencyProperty.RegisterAttached("Closing",
            typeof(ICommand), typeof(WindowClosingBehavior),
            new UIPropertyMetadata(null, OnClosingEventInfoChanged));

        public static ICommand GetClosing(DependencyObject source)
        {
            return (ICommand)source.GetValue(ClosingProperty);
        }

        public static void SetClosing(DependencyObject source, ICommand command)
        {
            source.SetValue(ClosingProperty, command);
        }

        private static void OnClosingEventInfoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var win = sender as Window;
            if (win != null)
            {
                win.Closing -= OnWindowClosing;
                if (e.NewValue != null)
                {
                    win.Closing += OnWindowClosing;
                }
            }
        }

        private static void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var dpo = (DependencyObject)sender;
            ICommand closingCommand = GetClosing(dpo);
            if (closingCommand != null)
            {
                closingCommand.Execute(e);
            }                
        }
    }
}
