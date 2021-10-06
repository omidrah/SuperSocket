using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using Socket.Domain.Infra;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;
using SuperSocket.WebSocket;

namespace Socket.ConsoleApp
{
    public class Program : ServiceBase
    {
        private static MyAppServer appServer;

        static void Main(string[] args)
        {
            WebSocketServer server = new WebSocketServer();
            server.NewSessionConnected += server_NewSessionConnected;
            server.NewMessageReceived += server_NewMessageReceived;
            server.SessionClosed += server_SessionClosed;
            try
            {
                server.Setup("127.0.0.1", 40001);//Set port
                server.Start();//Open monitoring
                PushMsg();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }

        static void PushMsg()
        {
            Thread.Sleep(1000);
            try { sessionList.ForEach(o => { o.Send("Test message push"); }); }
            catch (Exception ex) { }
            PushMsg();
        }

        public static List<WebSocketSession> sessionList = new List<WebSocketSession>();

        static void server_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            sessionList.Remove(session);
            Console.WriteLine(session.Origin);
        }

        static void server_NewMessageReceived(WebSocketSession session, string value)
        {
            Console.WriteLine(value);
            session.Send(value);
        }

        static void server_NewSessionConnected(WebSocketSession session)
        {
            sessionList.Add(session);
            Console.WriteLine(session.Origin);
        }
    }
}