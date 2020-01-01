// <copyright file="ViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CrossRefNC
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class ViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Variable> innerVariables = new ObservableCollection<Variable>();
        private readonly ObservableCollection<string> innerUniqueVariables = new ObservableCollection<string>();
        private string csharpPath;
        private string ncPath;
        private string fileInProgress;
        private int noReadFiles;
        private string fileContent;

        public ViewModel()
        {
            this.Variables = new ReadOnlyObservableCollection<Variable>(this.innerVariables);
            this.UniqueVariables = new ReadOnlyObservableCollection<string>(this.innerUniqueVariables);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CsharpPath
        {
            get
            {
                string csharpPath = this.csharpPath;
                return csharpPath;
            }

            set
            {
                if (value == this.csharpPath)
                {
                    return;
                }

                this.csharpPath = value;
                this.OnPropertyChanged();
            }
        }

        public string NcPath
        {
            get => this.ncPath;
            set
            {
                if (value == this.ncPath)
                {
                    return;
                }

                this.ncPath = value;
                this.OnPropertyChanged();
            }
        }

        public string FileInProgress
        {
            get => this.fileInProgress; set
            {
                if (value == this.fileInProgress)
                {
                    return;
                }

                this.fileInProgress = value;
                this.OnPropertyChanged();
            }
        }

        public ReadOnlyObservableCollection<Variable> Variables { get; }

        public ReadOnlyObservableCollection<string> UniqueVariables { get; }

        public int NoReadFiles
        {
            get => this.noReadFiles; set
            {
                if (value == this.noReadFiles)
                {
                    return;
                }

                this.noReadFiles = value;
                this.OnPropertyChanged();
            }
        }

        public string FileContent
        {
            get => this.fileContent; set
            {
                if (value == this.fileContent)
                {
                    return;
                }

                this.fileContent = value;
                this.OnPropertyChanged();
            }
        }

        public void ReadFiles()
        {
            this.innerVariables.Clear();
            this.innerUniqueVariables.Clear();
            if (string.IsNullOrEmpty(this.ncPath) &&
                string.IsNullOrEmpty(this.csharpPath))
            {
                throw new NoNullAllowedException();
            }

            if (!string.IsNullOrEmpty(this.ncPath))
            {
                this.ReadFiles(this.ncPath, "*.*");
            }

            if (!string.IsNullOrEmpty(this.csharpPath))
            {
                this.ReadFiles(this.csharpPath, "*.cs");
            }

            foreach (var name in this.Variables
                                     .Select(x => x.Name.ToUpperInvariant())
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .OrderBy(x => x))
            {
                this.innerUniqueVariables.Add(name);
            }
        }

        public int ShowSelectedFile(string filePath, string selectedVariable)
        {
            var lineNr = 0;
            var index = 0;
            this.FileContent = string.Empty;
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (line.Contains(selectedVariable))
                {
                    lineNr = index;
                }

                this.FileContent += line + "\r\n";
                index++;
            }

            return lineNr;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ReadFiles(string directory, string pattern)
        {
            foreach (var file in Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
            {
                this.FileInProgress = file;
                this.NoReadFiles++;

                foreach (var line in File.ReadAllLines(file))
                {
                    if (Variable.TryParse(line, file, out var results))
                    {
                        foreach (var item in results)
                        {
                            this.innerVariables.Add(item);
                        }
                    }
                }
            }
        }
    }
}
