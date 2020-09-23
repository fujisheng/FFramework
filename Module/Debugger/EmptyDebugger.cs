using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Debugger
{
    public class EmptyDebugger : IDebugger
    {
        public virtual void Log(object message, string color = "white"){ }
        public virtual void LogWarning(object messgae){}
        public virtual void LogError(object message){}
        public virtual void LogFormat(string format, string color, params object[] args){}
        public virtual void LogWarningFormat(string format, params object[] args){}
        public virtual void LogErrorFormat(string format, params object[] args){}
    }
}