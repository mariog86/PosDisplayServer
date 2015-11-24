using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pos.DisplayServer
{
    public class Server
    {
        private ConcurrentBag<Task> tasks;
        private TcpListener tcpListener;
        private CancellationToken cancellationToken;
        private CancellationTokenSource cancellationTokenSource;
        private bool disposed;

        public event Action<object, string, IPAddress> MessageReceived;

        public bool IsRunning { get; private set; }

        public void Start(int port)
        {
            var task = this.Run(port);
        }

        public void Stop()
        {
            this.cancellationTokenSource?.Cancel();
            try
            {
                if (this.tasks != null)
                {
                    Task.WaitAll(this.tasks.ToArray());
                }
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions.Any(ex => !((ex is TaskCanceledException) || (ex is ObjectDisposedException))))
                {
                    throw;
                }
            }
            finally
            {
                this.IsRunning = false;
                this.cancellationTokenSource?.Dispose();
                this.cancellationTokenSource = null;
                this.tasks = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnMessageReceived(object sender, string message, IPAddress source)
        {
            this.MessageReceived?.Invoke(sender, message, source);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Stop();
            }

            this.disposed = true;
        }

        private static IPAddress GetSenderIpAddress(TcpClient client)
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address;
        }

        private Task Run(int port)
        {
            this.IsRunning = true;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.tasks = new ConcurrentBag<Task>();
            var listenerTask = Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        this.tcpListener = new TcpListener(IPAddress.Any, port);
                        this.tcpListener.Start();
                        Task<TcpClient> acceptTask = this.tcpListener.AcceptTcpClientAsync();
                        do
                        {
                            if (acceptTask.IsCompleted)
                            {
                                TcpClient acceptTcpClient = acceptTask.Result;
                                var processingTask = Task.Factory.StartNew(() => this.ReadAsync(acceptTcpClient, this.cancellationToken), this.cancellationToken);
                                this.tasks.Add(processingTask);

                                acceptTask = this.tcpListener.AcceptTcpClientAsync();
                                this.tasks.Add(acceptTask);
                            }

                            try
                            {
                                acceptTask.Wait(this.cancellationToken);
                            }
                            catch (OperationCanceledException)
                            {
                            }
                        }
                        while (!this.cancellationToken.IsCancellationRequested);
                    }
                    finally
                    {
                        this.tcpListener?.Stop();
                    }
                }, this.cancellationToken);

            this.tasks.Add(listenerTask);
            return listenerTask;
        }

        private void ReadAsync(TcpClient client, CancellationToken ct)
        {
            using (client)
            {
                var stream = client.GetStream();
                string message = null;
                do
                {
                    try
                    {
                        var reader = new StreamReader(stream, Encoding.UTF8);
                        var readLineAsync = reader.ReadLineAsync();
                        readLineAsync.Wait(this.cancellationToken);
                        message = readLineAsync.Result;
                        if (message != null)
                        {
                            IPAddress ipAddress = GetSenderIpAddress(client);
                            this.OnMessageReceived(this, message, ipAddress);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
                while (!ct.IsCancellationRequested && message != null);
            }
        }
    }
}
