using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TheVault.Utils
{
    // State object for reading client data asynchronously  
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
    }

    public class AsynchronousSocketListener
    {
        public ManualResetEvent AllDone { get; set; }

        public bool IsStopped { private get; set; }
        
        public AsynchronousSocketListener()
        {
            AllDone = new ManualResetEvent(false);
            IsStopped = false;
        }

        public void StartListening()
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList.Last();
            ConsoleManager.WriteLine($"=== Ip server : {ipAddress} ===");
            var localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (!IsStopped)
                {
                    AllDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    ConsoleManager.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(AcceptCallback, listener);

                    // Wait until a connection is made before continuing.  
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                ConsoleManager.WriteLine(e is ThreadInterruptedException ? "*Server interrupted" : $"{e}");
            }

            listener.Close();
            AllDone.Close();
            AllDone = null;
            ConsoleManager.WriteLine("**Server stopped");
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                AllDone.Set();

                // Get the socket that handles the client request.  
                var listener = (Socket)ar.AsyncState;
                var handler = listener.EndAccept(ar);
    
                // Create the state object.  
                var state = new StateObject {WorkSocket = handler};
                ConsoleManager.WriteLine($"-- Begin receive");
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is ObjectDisposedException)
                    ConsoleManager.WriteLine("***Listener forced EndAccept");
                else
                    ConsoleManager.WriteLine($"{e}");
            }
        }

        public void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            var state = (StateObject)ar.AsyncState;
            var handler = state.WorkSocket;
            
            // Read data from the client socket.   
            var bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.StringBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                var content = state.StringBuilder.ToString();
                if (content.IndexOf("<EOF>", StringComparison.Ordinal) > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    ConsoleManager.WriteLine($"-- Read {content.Length} bytes from socket.");
                    ConsoleManager.WriteLine($"-- Data : {content}");
                
                    // Echo the data back to the client.  
                    //Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
                }
            }
        }
        
        /*
            int bytesRead;
            int current = 0;
         
            ServerSocket serverSocket = null;
            serverSocket = new ServerSocket(13267);
               
            while(true) {
                Socket clientSocket = null;
                clientSocket = serverSocket.accept();
                 
                InputStream in = clientSocket.getInputStream();
                 
                DataInputStream clientData = new DataInputStream(in); 
                 
                String fileName = clientData.readUTF();   
                OutputStream output = new FileOutputStream(fileName);   
                long size = clientData.readLong();   
                byte[] buffer = new byte[1024];   
                while (size > 0 && (bytesRead = clientData.read(buffer, 0, (int)Math.min(buffer.length, size))) != -1)   
                {   
                    output.write(buffer, 0, bytesRead);   
                    size -= bytesRead;   
                }
                 
                // Closing the FileOutputStream handle
                output.close();
            }
        */

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
                ConsoleManager.WriteLine($"Sent {bytesSent} bytes to client.");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                ConsoleManager.WriteLine($"{e}");
            }
        }
    }
}
