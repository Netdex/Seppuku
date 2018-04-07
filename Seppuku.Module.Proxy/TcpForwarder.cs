using System;
using System.Net;
using System.Net.Sockets;
using NLog;

namespace Seppuku.Module.Internal.Proxy
{
    // http://blog.brunogarcia.com/2012/10/simple-tcp-forwarder-in-c.html
    public class TcpForwarderSlim
    {
        private static readonly NLog.Logger L = NLog.LogManager.GetCurrentClassLogger();

        private readonly Socket _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public IPEndPoint Remote { get; set; }

        public void Start(IPEndPoint local)
        {
            _mainSocket.Bind(local);
            _mainSocket.Listen(10);

            try
            {
                while (true)
                {
                    var source = _mainSocket.Accept();
                    var destination = new TcpForwarderSlim();
                    var state = new State(source, destination._mainSocket);
                    destination.Connect(Remote, source);
                    source.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                }
            }
            catch (SocketException ex)
            {
                L.Error(ex, "TCP forwarder exception");
            }

        }

        public void Stop()
        {
            if (_mainSocket != null)
            {
                _mainSocket.Close();
            }
        }
        private void Connect(EndPoint remoteEndpoint, Socket destination)
        {
            var state = new State(_mainSocket, destination);
            _mainSocket.Connect(remoteEndpoint);
            _mainSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnDataReceive, state);
        }

        private static void OnDataReceive(IAsyncResult result)
        {
            var state = (State)result.AsyncState;
            try
            {
                var bytesRead = state.SourceSocket.EndReceive(result);
                if (bytesRead > 0)
                {
                    state.DestinationSocket.Send(state.Buffer, bytesRead, SocketFlags.None);
                    state.SourceSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnDataReceive, state);
                }
                else
                {
                    state.DestinationSocket.Shutdown(SocketShutdown.Both);
                    state.DestinationSocket.Close();
                    state.SourceSocket.Shutdown(SocketShutdown.Both);
                    state.SourceSocket.Close();
                }
            }
            catch(Exception ex)
            {
                //L.Error(ex, "data receive error");
                state.DestinationSocket.Close();
                state.SourceSocket.Close();
            }
        }

        private class State
        {
            public Socket SourceSocket { get; private set; }
            public Socket DestinationSocket { get; private set; }
            public byte[] Buffer { get; private set; }

            public State(Socket source, Socket destination)
            {
                SourceSocket = source;
                DestinationSocket = destination;
                Buffer = new byte[8192];
            }
        }
    }
}
