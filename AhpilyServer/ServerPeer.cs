using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AhpilyServer
{
    /// <summary>
    /// 服务器端
    /// </summary>
    public class ServerPeer
    {
        /// <summary>
        /// 服务器端的socket对象
        /// </summary>
        private Socket serverSocket;

        private Semaphore acceptSemaphore;

        public ServerPeer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 用来开启服务器
        /// </summary>
        /// <param name="prot">端口号</param>
        /// <param name="maxCount">最大连接数</param>
        public void Start(int port, int maxCount)
        {
            try
            {
                acceptSemaphore = new Semaphore(maxCount, maxCount);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(10);

                Console.WriteLine("服务器启动...");

                startAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 开始等待客户端的连接
        /// </summary>
        private void startAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += accept_Completed;
            }

            //限制线程的访问
            acceptSemaphore.WaitOne();



            bool result = serverSocket.AcceptAsync(e);
            //返回值判断异步事件是否执行完毕  true 代表正在执行 执行完毕后会触发
            //                                false 代表已经执行完成 直接处理
            if (result == false)
            {
                processAccept(e);
            }
        }

        /// <summary>
        /// 接受连接请求异步事件完成时触发
        /// </summary>
        private void accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            processAccept(e);
        }

        /// <summary>
        /// 处理连接请求
        /// </summary>
        private void processAccept(SocketAsyncEventArgs e)
        {
            //得到客户端的对象
            Socket clientSocket = e.AcceptSocket;
            //TODO
        }


    }
}
