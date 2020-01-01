namespace CrossRefNC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    [DebuggerDisplay("Name: {this.Name}, Comment: {this.Comment}")]
    public class Variable : INotifyPropertyChanged
    {
        public Variable(string file, string name, string comment)
        {
            this.File = file;
            this.Name = name;
            this.Comment = comment;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string File { get; }

        public string Name { get; }

        public string Comment { get; }

        public static bool TryParse(string line, string file, out IReadOnlyList<Variable> variables)
        {
            if (line.Contains(";"))
            {
                var matches = Regex.Matches(line, "(?<variable>R[0-9]+) *= *(?<comment>[^R]+)");
                if (matches.Count > 0)
                {
                    var results = new List<Variable>(matches.Count);
                    foreach (Match match in matches)
                    {
                        results.Add(new Variable(file, match.Groups["variable"].Value.Trim(), match.Groups["comment"].Value.Trim()));
                    }

                    variables = results;
                    return true;
                }
            }
            else
            {
                var matches = Regex.Matches(line, "(?<variable>R[0-9]+)", RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    var results = new List<Variable>(matches.Count);
                    foreach (Match match in matches)
                    {
                        results.Add(new Variable(file, match.Groups["variable"].Value.Trim(), string.Empty));
                    }

                    variables = results;
                    return true;
                }
            }

            variables = null;
            return false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
