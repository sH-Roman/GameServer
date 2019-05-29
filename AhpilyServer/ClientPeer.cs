using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 封装的客户端的连接对象
    /// </summary>
    public class ClientPeer
    {
        public Socket ClientSocket { get; set; }

        public ClientPeer()
        {
            this.ReceiveArgs = new SocketAsyncEventArgs();
            this.ReceiveArgs.UserToken = this;
            this.SendArgs = new SocketAsyncEventArgs();
            this.SendArgs.Completed += SendArgs_Completed;
        }



        #region 接受数据

        public delegate void ReceiveCompleted(ClientPeer client, SocketMsg msg);

        /// <summary>
        /// 一个消息解析完成的回调
        /// </summary>
        public ReceiveCompleted receiveCompleted;

        //一旦接收到数据 就存到缓存区里面
        private List<byte> dataCache = new List<byte>();

        //接收的异步套接字请求
        public SocketAsyncEventArgs ReceiveArgs;

        //是否正在处理接收的数据
        private bool isReceiveProcess = false;

        /// <summary>
        /// 自身处理数据包
        /// </summary>
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            if (!isReceiveProcess)
            {
                processReceive();
            }
        }

        /// <summary>
        /// 处理接收的数据
        /// </summary>
        private void processReceive()
        {
            isReceiveProcess = true;
            //解析数据包
            byte[] data = EncodeTool.DecodePacket(ref dataCache);

            if (data == null)
            {
                isReceiveProcess = false;
                return;
            }

            //需要再次转成一个具体的类型供我们使用
            SocketMsg msg = EncodeTool.DecodeMsg(data);
            //回调给上层
            if (receiveCompleted != null)
            {
                receiveCompleted(this, msg);
            }

            //尾递归
            processReceive();
        }

        #endregion

        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            //清空数据
            dataCache.Clear();
            isReceiveProcess = false;
            //TODO 给发送数据预留的

            //断开连接
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ClientSocket = null;
        }



        #endregion

        #region 发送数据

        /// <summary>
        /// 发送的消息的一个队列
        /// </summary>
        private Queue<byte[]> sendQueue = new Queue<byte[]>();

        private bool isSendProcess = false;

        /// <summary>
        /// 发送的异步套接字操作
        /// </summary>
        private SocketAsyncEventArgs SendArgs;


        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        public void Send(int opCode, int subCode, object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value;
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            //存入消息队列里面
            sendQueue.Enqueue(packet);
            if (!isSendProcess)
                processSend();
        }

        /// <summary>
        /// 处理发送的消息
        /// </summary>
        private void processSend()
        {
            isSendProcess = true;

            if (sendQueue.Count == 0)
            {
                isSendProcess = false;
                return;
            }
            //取出一条数据
            byte[] packet = sendQueue.Dequeue();
            //设置消息发送的异步套接字操作的发送数据缓冲区
            SendArgs.SetBuffer(packet, 0, packet.Length);
            bool result = ClientSocket.SendAsync(SendArgs);
            if (result == false)
            {
                sendCompleted();
            }
        }


        private void SendArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            sendCompleted();
        }

        /// <summary>
        /// 当异步发送请求完成的时候调用
        /// </summary>
        private void sendCompleted()
        {
            //发送的有没有错误
            if(SendArgs.SocketError != SocketError.Success)
            {
                //发送出错了
            }
            else
            {

            }

        }


        #endregion
    }
}