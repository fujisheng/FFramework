namespace Framework.Module.Log
{
    public class UnityLogger : ILogger
    {
        public void Info(object message, string color = "white")
        {
            string msg = string.Format("<color={0}>{1}</color>", color, message.ToString());
            UnityEngine.Debug.Log(msg);
        }

        public void Warning(object messgae)
        {
            UnityEngine.Debug.LogWarning(messgae);
        }

        public void Error(object message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}

