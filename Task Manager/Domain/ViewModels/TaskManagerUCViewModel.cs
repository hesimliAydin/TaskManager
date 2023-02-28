using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Task_Manager.Domain.Commands;

namespace Task_Manager.Domain.ViewModels
{
    public class TaskManagerUCViewModel:BaseViewModel
    {
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand CreateProcessCommand { get; set; }
        public RelayCommand AddProcessToBlackBoxCommand { get; set; }



        private ObservableCollection<Process> processes=new ObservableCollection<Process>();
        public ObservableCollection<Process> Processes
        {
            get { return processes; }
            set { processes = value; OnPropertyChanged();}
        }



        private Process selectedProcess;
        public Process SelectedProcess
        {
            get { return selectedProcess; }
            set { selectedProcess = value;OnPropertyChanged(); }
        }


        private string newProcessName;
        public string NewProcessName
        {
            get { return newProcessName; }
            set { newProcessName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> blackBoxProcesses=new ObservableCollection<string>();
        public ObservableCollection<string>  BlackBoxProcesses
        {
            get { return blackBoxProcesses; }
            set { blackBoxProcesses= value; OnPropertyChanged(); }
        }



        private Process selectedBlackBoxProcess;

        public Process SelectedBlackBoxProcess
        {
            get { return selectedBlackBoxProcess; }
            set { selectedBlackBoxProcess = value;OnPropertyChanged(); }

        }



        private DispatcherTimer _dispatcherTimer;


        public TaskManagerUCViewModel()
        {
            UpdateProcesses(null, null);
            DispatcherTimerSetup();


            CreateProcessCommand = new RelayCommand((c) =>
            {
                var name=string.Empty;
                name = NewProcessName;

                if (!name.EndsWith(".exe"))
                {
                    name += ".exe";
                }

                try
                {
                    Process.Start(name);
                }
                catch (Exception)
                {

                    MessageBox.Show($"'{NewProcessName}' does not exist.");
                }

            });

            ExitCommand = new RelayCommand((c) =>
            {
                App.Current.MainWindow.Close();
            });

            
        }

        public void DispatcherTimerSetup()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += UpdateProcesses;
            _dispatcherTimer.Interval=TimeSpan.FromSeconds(1);
            _dispatcherTimer.Start();

        }


        public void UpdateProcesses(object sender,EventArgs e)
        {
            Processes = new ObservableCollection<Process>(Process.GetProcesses().OrderBy(p => p.ProcessName));
            Processes.ToList().Where(p => BlackBoxProcesses.Contains(p.ProcessName)).ToList().ForEach(pr => pr.Kill());
        }


    }
}
