using System;

namespace Framework.Module.Message
{
    public class MessageCenter
    {
        static MessageBase<IArgs> center = new MessageBase<IArgs>();

        public static void AddListener(string message, Action<IArgs> listener)
        {
            center.AddListener(message, listener);
        }

        public static void RemoveListener(string message, Action<IArgs> listener)
        {
            center.RemoveListener(message, listener);
        }

        public static void SendMessage(string message, IArgs args = null)
        {
            center.SendMessage(message, args);
            args.Release();
        }
    }
}
