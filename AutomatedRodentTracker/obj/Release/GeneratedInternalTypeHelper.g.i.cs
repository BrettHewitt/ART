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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XamlGeneratedNamespace {
    
    
    /// <summary>
    /// GeneratedInternalTypeHelper
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class GeneratedInternalTypeHelper : System.Windows.Markup.InternalTypeHelper {
        
        /// <summary>
        /// CreateInstance
        /// </summary>
        protected override object CreateInstance(System.Type type, System.Globalization.CultureInfo culture) {
            return System.Activator.CreateInstance(type, ((System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic) 
                            | (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.CreateInstance)), null, null, culture);
        }
        
        /// <summary>
        /// GetPropertyValue
        /// </summary>
        protected override object GetPropertyValue(System.Reflection.PropertyInfo propertyInfo, object target, System.Globalization.CultureInfo culture) {
            return propertyInfo.GetValue(target, System.Reflection.BindingFlags.Default, null, null, culture);
        }
        
        /// <summary>
        /// SetPropertyValue
        /// </summary>
        protected override void SetPropertyValue(System.Reflection.PropertyInfo propertyInfo, object target, object value, System.Globalization.CultureInfo culture) {
            propertyInfo.SetValue(target, value, System.Reflection.BindingFlags.Default, null, null, culture);
        }
        
        /// <summary>
        /// CreateDelegate
        /// </summary>
        protected override System.Delegate CreateDelegate(System.Type delegateType, object target, string handler) {
            return ((System.Delegate)(target.GetType().InvokeMember("_CreateDelegate", (System.Reflection.BindingFlags.InvokeMethod 
                            | (System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)), null, target, new object[] {
                        delegateType,
                        handler}, null)));
        }
        
        /// <summary>
        /// AddEventHandler
        /// </summary>
        protected override void AddEventHandler(System.Reflection.EventInfo eventInfo, object target, System.Delegate handler) {
            eventInfo.AddEventHandler(target, handler);
        }
    }
}

