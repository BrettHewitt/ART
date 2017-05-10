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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AutomatedRodentTracker.Controls
{
    public class SliderIgnoreDelta : Slider
    {
        public bool DragStarted
        {
            get;
            set;
        }

        public static readonly DependencyProperty DragCompleteProperty = DependencyProperty.RegisterAttached("DragComplete", typeof(ICommand), typeof(SliderIgnoreDelta));

        public SliderIgnoreDelta()
        {
            DragStarted = false;
        }

        public static ICommand GetDragComplete(DependencyObject source)
        {
            return (ICommand)source.GetValue(DragCompleteProperty);
        }

        public static void SetDragComplete(DependencyObject source, ICommand command)
        {
            source.SetValue(DragCompleteProperty, command);
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);

            DragStarted = false;
            ICommand completeCommand = GetDragComplete(this);
            if (completeCommand != null)
            {
                completeCommand.Execute(Value);
            }  
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);

            DragStarted = true;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            if (DragStarted)
            {
                return;
            }

            ICommand completeCommand = GetDragComplete(this);
            if (completeCommand != null)
            {
                completeCommand.Execute(newValue);
            } 
        }
    }
}
