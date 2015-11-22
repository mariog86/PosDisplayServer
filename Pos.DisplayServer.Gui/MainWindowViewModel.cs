using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Pos.DisplayServer.Gui
{
    public class MainWindowViewModel
    {
        //private HttpListener listener;
        private ObservableCollection<string> items;
        private TcpListener listener;
        private Task listenerTask;
        private CancellationToken cancellationToken;

        public MainWindowViewModel()
        {
            //httpListener = new HttpListener();
            //httpListener.Start();
            //var httpListenerContext = httpListener.GetContextAsync();

            items = new ObservableCollection<string>();
            Items = new ReadOnlyObservableCollection<string>(items);
            StartListener();
        }

        private void StartListener()
        {
            var tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;
            listenerTask = Task.Run(() =>
            {
                try
                {
                    listener = new TcpListener(IPAddress.Any, 6740);
                    listener.Start();
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        TcpClient acceptTcpClient = listener.AcceptTcpClient();
                        Task.Run(() => ProcessRequest(acceptTcpClient));
                    }
                }
                finally
                {
                    listener?.Stop();
                }
            }, cancellationToken);
        }

        void ProcessRequest(TcpClient client)
        {
            string text;
            using (var reader = new StreamReader(client.GetStream(), Encoding.UTF8))
            {
                text = reader.ReadLine();
                if (!string.IsNullOrEmpty(reader.ReadToEnd()))
                {
                    return;
                }
            }

            Invoke(() => items.Add($"{"127.0.0.1"}: {text}"));
        }

        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.Invoke(action);
            }
        }

        public ReadOnlyObservableCollection<string> Items { get; }
    }
}
