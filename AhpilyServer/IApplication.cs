using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    public interface IApplication
    {
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client"></param>
        void OnDisconnect(ClientPeer client);

        /// <summary>
        /// 接受数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        void OnReceive(ClientPeer client, SocketMsg msg);

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="client"></param>
        void OnConnect(ClientPeer client);
    }
}
