using UnityEngine;


namespace Framework.Module.Debugger
{
    public class UnityDebuger : IDebugger
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

        public void LogFormat(string format, string color, params object[] args)
        {
            format = string.Format("<color={0}>{1}</color>", color, format);
            Debug.LogFormat(format, args);
        }

        public void LogWarningFormat(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }
    }
}

