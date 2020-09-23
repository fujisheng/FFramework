namespace Framework.Module.Debugger
{
    public interface IDebugManager
    {
        void SetDebugger(IDebugger debugger);
        void SetLevel(DebugLevel level);
        void Log(object message, string color);
        void LogG(object message);
        void LogR(object message);
        void LogY(object message);
        void LogB(object message);
        void LogWarning(object message);
        void LogError(object message);
        void LogFormat(string format, params object[] args);
        void LogFormat(string format, string color, params object[] args);
        void LogFormatR(string format, params object[] args);
        void LogFormatG(string format, params object[] args);
        void LogFormatB(string format, params object[] args);
        void LogFormatY(string format, params object[] args);
        void LogWarningFormat(string format, params object[] args);
        void LogErrorFormat(string format, params object[] args);
    }
}

