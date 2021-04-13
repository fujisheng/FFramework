using UnityEngine;


namespace Framework.Module.Debugger
{
    public class UnityDebugger : IDebugger
    {
        public void Log(object message, string color = "white")
        {
            string msg = string.Format("<color={0}>{1}</color>", color, message.ToString());
            Debug.Log(msg);
        }

        public void LogWarning(object messgae)
        {
            Debug.LogWarning(messgae);
        }

        public void LogError(object message)
        {
            Debug.LogError(message);
        }
    }
}

