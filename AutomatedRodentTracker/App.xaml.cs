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
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutomatedRodentTracker.Repository;
using AutomatedRodentTracker.RepositoryInterface;

namespace AutomatedRodentTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IRepository repository = RepositoryResolver.Resolve<IRepository>();
            if (!repository.GetValue<bool>("SeenLicense"))
            {
                var result = MessageBox.Show("This program is distrubuted under the General Public License. You must agree to the terms and conditions of this license before using the program", "GPL License", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            MainWindow view = new MainWindow();
            MainWindowViewModel viewModel = new MainWindowViewModel(e.Args);

            view.DataContext = viewModel;

            view.Show();

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unexpected error has occured, please send the file CrashLog.txt to brett_hewitt@live.co.uk " + e.Exception, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            string path = @"CrashLog.txt";
            if (!File.Exists(path))
            {
                // Create a file to write to. 
                using (File.CreateText(path))
                {

                }
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("Date Time: " + DateTime.Now);
                sw.WriteLine(e.Exception.Message);
                sw.WriteLine(e.Exception.StackTrace);
            }
        }
    }
}
