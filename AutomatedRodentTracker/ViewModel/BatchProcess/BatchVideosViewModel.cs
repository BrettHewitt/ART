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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedRodentTracker.Commands;
using AutomatedRodentTracker.Services.FileBrowser;

namespace AutomatedRodentTracker.ViewModel.BatchProcess
{
    public class BatchVideosViewModel : WindowViewModelBase
    {
        private ActionCommand m_AddTgFileCommand;
        private ActionCommand m_AddTgFolderCommand;
        private ActionCommand m_RemoveTgFileCommand;
        private ActionCommand m_ClearTgFilesCommand;
        private ActionCommand m_AddNtgFileCommand;
        private ActionCommand m_AddNtgFolderCommand;
        private ActionCommand m_RemoveNtgFileCommand;
        private ActionCommand m_ClearNtgFilesCommand;
        private ActionCommand m_ProcessCommand;

        public ActionCommand AddTgFileCommand
        {
            get
            {
                return m_AddTgFileCommand ?? (m_AddTgFileCommand = new ActionCommand()
                {
                    ExecuteAction = AddTgFile,
                });
            }
        }

        public ActionCommand AddTgFolderCommand
        {
            get
            {
                return m_AddTgFolderCommand ?? (m_AddTgFolderCommand = new ActionCommand()
                {
                    ExecuteAction = AddTgFolder,
                });
            }
        }

        public ActionCommand RemoveTgFileCommand
        {
            get
            {
                return m_RemoveTgFileCommand ?? (m_RemoveTgFileCommand = new ActionCommand()
                {
                    ExecuteAction = RemoveTgFile,
                    CanExecuteAction = CanRemoveTgFile,
                });
            }
        }

        public ActionCommand ClearTgFilesCommand
        {
            get
            {
                return m_ClearTgFilesCommand ?? (m_ClearTgFilesCommand = new ActionCommand()
                {
                    ExecuteAction = ClearTgList,
                });
            }
        }

        public ActionCommand AddNtgFileCommand
        {
            get
            {
                return m_AddNtgFileCommand ?? (m_AddNtgFileCommand = new ActionCommand()
                {
                    ExecuteAction = AddNtgFile,
                });
            }
        }

        public ActionCommand AddNtgFolderCommand
        {
            get
            {
                return m_AddNtgFolderCommand ?? (m_AddNtgFolderCommand = new ActionCommand()
                {
                    ExecuteAction = AddNtgFolder,
                });
            }
        }

        public ActionCommand RemoveNtgFileCommand
        {
            get
            {
                return m_RemoveNtgFileCommand ?? (m_RemoveNtgFileCommand = new ActionCommand()
                {
                    ExecuteAction = RemoveNtgFile,
                    CanExecuteAction = CanRemoveNtgFile,
                });
            }
        }

        public ActionCommand ClearNtgFilesCommand
        {
            get
            {
                return m_ClearNtgFilesCommand ?? (m_ClearNtgFilesCommand = new ActionCommand()
                {
                    ExecuteAction = ClearNtgList,
                });
            }
        }

        public ActionCommand ProcessCommand
        {
            get
            {
                return m_ProcessCommand ?? (m_ProcessCommand = new ActionCommand()
                {
                    ExecuteAction = () =>
                    {
                        ExitResult = WindowExitResult.Ok;
                        CloseWindow();
                    },
                });
            }
        }

        private ObservableCollection<string> m_TgItemsSource;
        private string m_SelectedTgItem;

        private ObservableCollection<string> m_NtgItemsSource;
        private string m_SelectedNtgItem;

        public ObservableCollection<string> TgItemsSource
        {
            get
            {
                return m_TgItemsSource;
            }
            set
            {
                if (Equals(m_TgItemsSource, value))
                {
                    return;
                }

                m_TgItemsSource = value;

                NotifyPropertyChanged();
            }
        }

        public string SelectedTgItem
        {
            get
            {
                return m_SelectedTgItem;
            }
            set
            {
                if (Equals(m_SelectedTgItem, value))
                {
                    return;
                }

                m_SelectedTgItem = value;

                NotifyPropertyChanged();
                RemoveTgFileCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public ObservableCollection<string> NtgItemsSource
        {
            get
            {
                return m_NtgItemsSource;
            }
            set
            {
                if (Equals(m_NtgItemsSource, value))
                {
                    return;
                }

                m_NtgItemsSource = value;

                NotifyPropertyChanged();
            }
        }

        public string SelectedNtgItem
        {
            get
            {
                return m_SelectedNtgItem;
            }
            set
            {
                if (Equals(m_SelectedNtgItem, value))
                {
                    return;
                }

                m_SelectedNtgItem = value;

                NotifyPropertyChanged();
                RemoveNtgFileCommand.RaiseCanExecuteChangedNotification();
            }
        }

        public BatchVideosViewModel()
        {
            ResetData();
        }

        private void ResetData()
        {
            ClearNtgList();
            ClearTgList();
        }

        private void ClearTgList()
        {
            SelectedTgItem = string.Empty;
            TgItemsSource = new ObservableCollection<string>();
        }

        private void ClearNtgList()
        {
            SelectedNtgItem = string.Empty;
            NtgItemsSource = new ObservableCollection<string>();
        }

        private void AddTgFile()
        {
            string fileLocation = FileBrowser.BroseForVideoFiles();

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            ObservableCollection<string> currentList = new ObservableCollection<string>(TgItemsSource);
            currentList.Add(fileLocation);
            TgItemsSource = currentList;
        }

        private void AddTgFolder()
        {
            string folderLocation = FileBrowser.BrowseForFolder();

            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                return;
            }

            ObservableCollection<string> currentList = new ObservableCollection<string>(TgItemsSource);

            string[] files = Directory.GetFiles(folderLocation);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);

                if (CheckIfExtensionIsVideo(extension))
                {
                    currentList.Add(file);
                }
            }

            TgItemsSource = currentList;
        }

        private bool CheckIfExtensionIsVideo(string extension)
        {
            return extension.Contains("avi") || extension.Contains("mpg") || extension.Contains("mpeg") || extension.Contains("mp4") || extension.Contains("mov");
        }

        private void RemoveTgFile()
        {
            TgItemsSource.Remove(SelectedTgItem);
            NotifyPropertyChanged("TgItemsSource");
        }

        private bool CanRemoveTgFile()
        {
            if (SelectedTgItem == null)
            {
                return false;
            }

            if (SelectedTgItem.Length == 0)

            {
                return false;
            }

            return true;
        }

        private void AddNtgFile()
        {
            string fileLocation = FileBrowser.BroseForVideoFiles();

            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            ObservableCollection<string> currentList = new ObservableCollection<string>(NtgItemsSource);
            currentList.Add(fileLocation);
            NtgItemsSource = currentList;
        }

        private void AddNtgFolder()
        {
            string folderLocation = FileBrowser.BrowseForFolder();

            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                return;
            }

            ObservableCollection<string> currentList = new ObservableCollection<string>(NtgItemsSource);

            string[] files = Directory.GetFiles(folderLocation);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);

                if (CheckIfExtensionIsVideo(extension))
                {
                    currentList.Add(file);
                }
            }

            NtgItemsSource = currentList;
        }

        private void RemoveNtgFile()
        {
            NtgItemsSource.Remove(SelectedNtgItem);
            NotifyPropertyChanged("NtgItemsSource");
        }

        private bool CanRemoveNtgFile()
        {
            if (SelectedNtgItem == null)
            {
                return false;
            }

            if (SelectedNtgItem.Length == 0)
            {
                return false;
            }

            return true;
        }
    }
}
