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

using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace AutomatedRodentTracker.Services.FileBrowser
{
    public static class FileBrowser
    {
        public static string BrowseForImageFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg; *.jpeg; *.gif; *.bmp; *.tif; *.png";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

        public static string BroseForVideoFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files|*.avi;*.mpg;*.mpeg;*.mp4;*.mov";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

        public static string BrowseForFile(string fileTypes, string title = "")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileTypes;

            if (!string.IsNullOrWhiteSpace(title))
            {
                openFileDialog.Title = title;

            }

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

        public static string BrowseForFile(string fileTypes, string title, string initialDirectory)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = fileTypes;

            if (!string.IsNullOrWhiteSpace(title))
            {
                openFileDialog.Title = title;
            }

            if (!string.IsNullOrWhiteSpace(initialDirectory))
            {
                openFileDialog.InitialDirectory = initialDirectory;
            }

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

        public static string SaveFile(string fileType, string initialDirectory = "")
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = fileType;

            if (!string.IsNullOrWhiteSpace(initialDirectory))
            {
                saveFileDialog.InitialDirectory = initialDirectory;
            }

            bool? result =saveFileDialog.ShowDialog();

            if (result == true)
            {
                return saveFileDialog.FileName;
            }

            return string.Empty;
        }

        public static string SaveFile(string fileType, string fileName, string initialDirectory)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = fileType;
            saveFileDialog.FileName = fileName;

            if (!string.IsNullOrWhiteSpace(initialDirectory))
            {
                saveFileDialog.InitialDirectory = initialDirectory;
            }

            bool? result =saveFileDialog.ShowDialog();

            if (result == true)
            {
                return saveFileDialog.FileName;
            }

            return string.Empty;
        }

        public static string BrowseForFolder(string startingLocation = "")
        {
            var dialog = new FolderBrowserDialog();

            if (!string.IsNullOrWhiteSpace(startingLocation))
            {
                dialog.SelectedPath = startingLocation;
            }
            
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }

            return string.Empty;
        }
    }
}
