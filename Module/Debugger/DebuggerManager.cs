namespace Framework.Module.Debugger
{
    internal sealed class DebuggerManager : Module, IDebugManager
    {
        IDebugger debugger;
        int level = (int)DebugLevel.All;

        public void SetDebugger(IDebugger debugger)
        {
            this.debugger = debugger;
        }

        public void SetLevel(DebugLevel debugLevel)
        {
            this.level = (int)debugLevel;
        }

        bool HasLevel(DebugLevel level)
        {
            return !((this.level & (int)level) == 0);
        }

        public void Log(object message, string color = "white")
        {
            if (!HasLevel(DebugLevel.Normal))
            {
                return;
            }
            debugger.Log(message, color);
        }

        public void LogG(object message)
        {
            if (!HasLevel(DebugLevel.Green))
            {
                return;
            }
            Log(message, "green");
        }

        public void LogR(object message)
        {
            if (!HasLevel(DebugLevel.Red))
            {
                return;
            }
            Log(message, "red");
        }

        public void LogY(object message)
        {
            if (!HasLevel(DebugLevel.Yellow))
            {
                return;
            }
            Log(message, "yellow");
        }

        public void LogB(object message)
        {
            if (!HasLevel(DebugLevel.Blue))
            {
                return;
            }
            Log(message, "blue");
        }

        public void LogWarning(object message)
        {
            if (!HasLevel(DebugLevel.Warning))
            {
                return;
            }
            debugger.LogWarning(message);
        }

        public void LogError(object message)
        {
            if (!HasLevel(DebugLevel.Error))
            {
                return;
            }
            debugger.LogError(message);
        }

        public void LogFormat(string format, params object[] args)
        {
            if (!HasLevel(DebugLevel.Normal))
            {
                return;
            }
            debugger.LogFormat(format, "white", args);
        }

        public void LogFormat(string format, string color, params object[] args)
        {
            if (!HasLevel(DebugLevel.Normal))
            {
                return;
            }
            debugger.LogFormat(format, color, args);
        }

        public void LogFormatR(string format, params object[] args)
        {
            if (!HasLevel(DebugLevel.Red))
            {
                return;
            }
            LogFormat(format, "red", args);
        }

        public void LogFormatG(string format, params object[] args)
        {
            if (!HasLevel(DebugLevel.Green))
            {
                return;
            }
            LogFormat(format, "green", args);
        }
        public void LogFormatB(string format, params object[] args)
        {
            if (!HasLevel(DebugLevel.Blue))
            {
                return;
            }
            LogFormat(format, "blue", args);
        }
        public void LogFormatY(string format, params object[] args)
        {
            if (!HasLevel(DebugLevel.Yellow))
            {
                return;
            }
            LogFormat(format, "yellow", args);
        }

        public void LogWarningFormat(string format, params object[] args)
        {
            if (!HasLevel(DebugLevel.Warning))
            {
                return;
            }
            debugger.LogWarningFormat(format, args);
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            if (!HasLevel(DebugLevel.Error))
            {
                return;
            }
            debugger.LogErrorFormat(format, args);
        }
    }
}

