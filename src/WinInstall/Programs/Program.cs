using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinInstall.Programs
{
    abstract class Program : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Selected { get; set; }

        private string _icon;
        public string Icon => _icon ??= $"Assets/{GetType().Name}.ico";

        private bool? _installed;
        public bool Installed
        {
            get
            {
                if (_installed.HasValue)
                {
                    return _installed.Value;
                }
                _installed = Directory.Exists(Root) && Directory.EnumerateFileSystemEntries(Root).Any();
                return _installed.Value;
            }
        }

        private bool? _updated;
        public bool? Updated
        {
            get
            {
                if (_updated.HasValue)
                {
                    return _updated.Value;
                }
                Task.Run(async () => (Installed && new Version(await GetAvailableVersion()).CompareTo(new Version(await GetInstalledVersion())) <= 0)).ContinueWith(t => { _updated = t.Result; OnPropertyChanged("Updated"); });
                return _updated != null ? _updated.Value : (bool?)null;
            }
        }

        public abstract string Name { get; }

        public abstract string Info { get; }

        public abstract string Type { get; }

        public abstract string Root { get; }

        public abstract Task<string> Download();

        public abstract Task Install();

        protected abstract Task<string> GetAvailableVersion();

        protected abstract Task<string> GetInstalledVersion();
    }
}
