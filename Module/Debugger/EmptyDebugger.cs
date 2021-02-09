﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Debugger
{
    public class EmptyDebugger : IDebugger
    {
        public virtual void Log(object message, string color = "white"){ }
        public virtual void LogWarning(object messgae){}
        public virtual void LogError(object message){}
    }
}