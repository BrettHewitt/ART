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
