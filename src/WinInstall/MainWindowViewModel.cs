using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinInstall.Programs;

namespace WinInstall
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Program> _programs;
        public ObservableCollection<Program> Programs
        {
            get => _programs;
            set { _programs = value; OnPropertyChanged("Programs"); }
        }

        public MainWindowViewModel()
        {
            _programs = new ObservableCollection<Program>(GetPrograms());
            _installCommand = new DelegateCommand(OnInstall, CanInstall);
            _refreshCommand = new DelegateCommand(OnRefresh, CanRefresh);
        }

        private List<Program> GetPrograms()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.BaseType == typeof(Program))
                .Select(Activator.CreateInstance).Cast<Program>().ToList();
        }

        private List<Program> GetSelected()
        {
            return _programs.Where(p => p.Selected).ToList();
        }

        private readonly DelegateCommand _installCommand;
        public ICommand InstallCommand => _installCommand;
        private async void OnInstall(object commandParameter)
        {
            var window = Application.Current.Windows.OfType<MetroWindow>().FirstOrDefault();
            var programsTable = window.FindChild<DataGrid>();
            var progressGauge = window.FindChild<ProgressBar>();
            var refreshButton = window.FindChildren<Button>().ElementAtOrDefault(0);
            var installButton = window.FindChildren<Button>().ElementAtOrDefault(1);

            var selected = GetSelected();

            if (selected == null || !selected.Any())
            {
                await window.ShowMessageAsync(null, "You haven't selected anything!");
                return;
            }

            refreshButton.IsEnabled = false;
            installButton.IsEnabled = false;
            programsTable.IsEnabled = false;

            var progress = 100 / selected.Count() / 5;
            progressGauge.Value = progress;

            for (var i = 0; i < selected.Count(); i++)
            {
                try
                {
                    await selected[i].Install();
                }
                catch
                {
                    // TODO: Add to failed installations.
                }
                progress = (i + 1) * 100 / selected.Count();
                progressGauge.Value = progress;
            }

            Programs = new ObservableCollection<Program>(GetPrograms());
            await window.ShowMessageAsync(null, "The installation has been successfully completed!");
            refreshButton.IsEnabled = true;
            installButton.IsEnabled = true;
            programsTable.IsEnabled = true;
            progressGauge.Value = 0;
        }

        private bool CanInstall(object commandParameter)
        {
            return true;
        }

        private readonly DelegateCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;
        private void OnRefresh(object commandParameter)
        {
            Programs = new ObservableCollection<Program>(GetPrograms());
        }

        private bool CanRefresh(object commandParameter)
        {
            return true;
        }
    }
}
