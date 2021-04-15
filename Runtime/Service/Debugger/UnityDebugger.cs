using UnityEngine;


namespace Framework.Service.Debug
{
    public class UnityDebugger : IDebugger
    {
        public void Log(object message, string color = "white")
        {
            string msg = string.Format("<color={0}>{1}</color>", color, message.ToString());
            UnityEngine.Debug.Log(msg);
        }

        public void LogWarning(object messgae)
        {
            UnityEngine.Debug.LogWarning(messgae);
        }

        public void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}

