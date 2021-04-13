namespace Framework.Module.Debugger
{
    public interface IDebugger
    {
        void Log(object message, string color);
        void LogWarning(object message);
        void LogError(object message);
    }
}