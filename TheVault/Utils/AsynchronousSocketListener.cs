using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace TheVault.Utils
{
    public class StateObject
    {
        // Client  socket.  
        public Socket WorkSocket;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] Buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder StringBuilder = new StringBuilder();
        // Received data long.
        public long Size;
        // Received bytes.
        public byte[] FileData;
    }

    public class AsynchronousSocketListener
    {
        private ManualResetEvent AllDone { get; set; }

        public bool IsStopped { private get; set; }
        
        public ICommand SocketCallback { get; set; }
        
        private bool IsConnected { get; set; }
        
        public AsynchronousSocketListener()
        {
            AllDone = new ManualResetEvent(false);
            IsStopped = false;
        }

        public void StartListening()
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList.Last();
            var portNumber = 11000;
            
            WriteLine($"=== Ip server : {ipAddress}:{portNumber} ===");
            var localEndPoint = new IPEndPoint(ipAddress, portNumber);
            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (!IsStopped)
                {
                    AllDone.Reset();

                    if (!IsConnected) WriteLine("Waiting for a connection...");
                    listener.BeginAccept(AcceptCallback, listener);

                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                WriteLine(e is ThreadInterruptedException ? "*Server interrupted" : $"{e}");
            }

            listener.Close();
            AllDone.Close();
            AllDone = null;
            WriteLine("**Server stopped");
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                AllDone.Set();

                IsConnected = true;
                var listener = (Socket)ar.AsyncState;
                var handler = listener.EndAccept(ar);
    
                var state = new StateObject {WorkSocket = handler};
                WriteLine($"-- Begin receive");
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is ObjectDisposedException)
                    ConsoleManager.WriteLine("***Listener forced EndAccept");
                else
                    WriteLine($"{e}");
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var handler = state.WorkSocket;
            var bytesRead = handler.EndReceive(ar);
            if (bytesRead <= 0) return;
            
            try
            {
                if (string.IsNullOrWhiteSpace(state.StringBuilder.ToString()))
                    state.StringBuilder.Append(Encoding.UTF8.GetString(state.Buffer, 0, bytesRead));
                else if (state.Size == 0 && !string.IsNullOrWhiteSpace(state.StringBuilder.ToString()))
                {
                    state.Size = BitConverter.ToInt64(state.Buffer.Take(8).Reverse().ToArray(), 0);
                    state.FileData = state.Buffer.Skip(8).ToArray();
                    WriteLine(state.Size);
                }
                else if (state.FileData != null)
                {
                    state.FileData = state.FileData.Concat(state.Buffer.Take(bytesRead)).ToArray();
                    WriteLine(state.FileData.Length);
                }
            }
            catch (Exception e)
            {
                WriteLine($"{e}");
                throw;
            }
                
            var content = state.StringBuilder.ToString();
            if (content.IndexOf("<EOF>", StringComparison.Ordinal) > -1 ||
                state.FileData != null && state.Size != 0 && state.FileData.Length == state.Size)
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    WriteLine($"-- Read {content.Length} bytes from socket");
                    WriteLine($"-- Data : {content}");
                }

                if (state.FileData != null)
                {
                    WriteLine($"-- Decrypted filename : {EncryptionUtil.Decipher(content, 10)}");
                    WriteLine($"-- Read {state.FileData.Length} bytes on {state.Size}");
                    WriteLine(state.FileData);
                }
                
                WriteLine("Socket transfer is done, end communication");
                IsConnected = false;
            }
            else
            {
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
        }

        private void WriteLine(object info)
        {
            if (info is string message)
                ConsoleManager.WriteLine(message);
            
            SocketCallback?.Execute(info);
        }

        //TODO implement sending files
        /* //Send(handler, content);
        
        public static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            var byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                var handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                var bytesSent = handler.EndSend(ar);
                WriteLine($"Sent {bytesSent} bytes to client.");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                WriteLine($"{e}");
            }
        }
        */
    }
}
