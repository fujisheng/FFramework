namespace Framework.Module.Debugger
{
    public interface IDebugger
    {
        void Log(object message, string color);
        void LogWarning(object message);
        void LogError(object message);
        void LogFormat(string format, string color, params object[] args);
        void LogWarningFormat(string format, params object[] args);
        void LogErrorFormat(string format, params object[] args);
    }
}