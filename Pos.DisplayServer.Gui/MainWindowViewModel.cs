using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Pos.DisplayServer.Gui
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ObservableCollection<string> items;
        private readonly Server server = new Server();
        private bool disposed;
        private ushort port;

        public MainWindowViewModel()
        {
            this.items = new ObservableCollection<string>();
            this.Items = new ReadOnlyObservableCollection<string>(this.items);
            this.server.MessageReceived += this.Server_MessageReceived;
            this.Port = 6740;
            var application = Application.Current;
            if (application != null)
            {
                application.Exit += this.Current_Exit;
            }

            this.StartCommand = new Command(o => !this.server.IsRunning, o => this.StartListener());
            this.StopCommand = new Command(o => this.server.IsRunning, o => this.StopListener());
            this.StartListener();
        }

        private void StopListener()
        {
            try
            {
                this.server.Stop();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            ((Command)this.StartCommand).ThrowExecuteChanged();
            ((Command)this.StopCommand).ThrowExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ushort Port
        {
            get
            {
                return this.port;
            }

            set
            {
                if (value == this.port)
                {
                    return;
                }

                this.port = value;
                this.OnPropertyChanged();
            }
        }

        //public bool PortEditable

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public ReadOnlyObservableCollection<string> Items { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.server.Stop();
            }

            this.disposed = true;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            this.server.Stop();
        }

        private void StartListener()
        {
            try
            {
                this.server.Start(this.Port);
            }
            catch (Exception e)
            {
                this.server.Stop();
                MessageBox.Show(e.Message);
            }

            ((Command)this.StartCommand).ThrowExecuteChanged();
            ((Command)this.StopCommand).ThrowExecuteChanged();
        }

        private void Server_MessageReceived(object sender, string message, IPAddress source)
        {
            Invoke(() => this.items.Add($"{source}: {message}"));
        }

        private static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current?.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.Invoke(action);
            }
        }
    }
}
