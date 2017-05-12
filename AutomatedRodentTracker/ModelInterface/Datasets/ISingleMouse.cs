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

using System.Collections.Generic;
using AutomatedRodentTracker.ModelInterface;
using AutomatedRodentTracker.ModelInterface.Datasets.Types;
using AutomatedRodentTracker.ModelInterface.Results;

namespace AutomatedRodentTracker.ModelInterface.Datasets
{
    public interface ISingleMouse : IModelObjectBase
    {
        string Name
        {
            get;
            set;
        }

        string Id
        {
            get;
            set;
        }

        ITypeBase Type
        {
            get;
            set;
        }

        List<string> Videos
        {
            get;
            set;
        }

        string Class
        {
            get;
            set;
        }

        int Age
        {
            get;
            set;
        }

        List<ISingleFile> VideoFiles
        {
            get;
            set;
        }

        Dictionary<ISingleFile, IMouseDataResult> Results
        {
            get;
            set;
        }

        void GenerateFiles(string fileLocation);

        void AddFile(ISingleFile file);

        void RemoveFile(ISingleFile file);
    }
}
