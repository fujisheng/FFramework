namespace Framework.Service.Debug
{
    public interface IDebugService
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
    }
}

