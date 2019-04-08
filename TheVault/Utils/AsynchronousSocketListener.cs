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
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
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
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[3];
            Console.WriteLine($@"[DEBUG]Ip server : {ipAddress}");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (!IsStopped)
                {
                    // Set the event to nonsignaled state.  
                    AllDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine(@"[DEBUG]Waiting for a connection...");
                    listener.BeginAccept(AcceptCallback, listener);

                    // Wait until a connection is made before continuing.  
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                if (e is ThreadInterruptedException)
                    Console.WriteLine("[DEBUG]Server interrupted");
                else
                    Console.WriteLine($@"[DEBUG]{e.ToString()}");
            }

            listener.Close();
            //AllDone.Close();
            Console.WriteLine("[DEBUG]Server stopped");
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.
                AllDone.Set();
    
                // Get the socket that handles the client request.  
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
    
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
    
                handler.Close();
                listener.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($@"[DEBUG]{e}");
            }
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            
            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("[DEBUG]Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                
                    // Echo the data back to the client.  
                    //Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
                }
            }

            handler.Close();
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
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine(@"[DEBUG]Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine($@"[DEBUG]{e.ToString()}");
            }
        }
    }
}
