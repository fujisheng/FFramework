using UnityEngine;

namespace Framework.Module.Network
{
    internal sealed class NetworkManager : Module
    {
        IConnector connector;
        IPacker packer;
        IObjectPool<IMessage> messagePool;

        public NetworkManager()
        {
            messagePool = new MessagePool();
        }

        /// <summary>
        /// 设置消息打包器
        /// </summary>
        /// <param name="converter"></param>
        public void SetPacker(IPacker packer)
        {
            this.packer = packer;
            packer.SetMessagePool(messagePool);
        }

        /// <summary>
        /// 设置连接器
        /// </summary>
        /// <param name="connector"></param>
        public void SetConnector(IConnector connector)
        {
            this.connector = connector;

            if(packer == null)
            {
                Debug.LogError("Converter为空！！！需要先调用SetConverter");
                return;
            }

            connector.OnReceive = OnReceive;
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string ip, int port)
        {
            if(connector == null)
            {
                Debug.LogError("Connector为空！！！");
                return;
            }
            connector.Connect(ip, port);
        }

        /// <summary>
        /// 发送一个消息
        /// </summary>
        /// <param name="message"></param>
        public void Send(IMessage message)
        {
            if(connector == null || !connector.IsConnected)
            {
                Debug.LogError("Connector为空 或者没有连接到服务器！！！");
                return;
            }
            byte[] result = packer.Pack(message);
            if(result == null || result.Length == 0)
            {
                Debug.LogWarning("要发送的消息为空!!!");
                return;
            }
            connector.Send(result);
        }

        internal override void OnUpdate()
        {
            connector?.OnUpdate();
        }

        /// <summary>
        /// 当收到消息的时候
        /// </summary>
        /// <param name="message">最终需要的东西</param>
        void OnReceive(byte[] bytes)
        {
            IMessage message = packer.Unpack(bytes);
            //TODO 分发消息
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if(connector == null)
            {
                Debug.Log("Connector为空!!!");
                return;
            }
            connector.Close();
        }
    }
}