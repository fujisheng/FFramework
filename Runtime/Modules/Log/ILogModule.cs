namespace Framework.Module.Log
{
    public interface ILogModule
    {
        void SetLogger(ILogger debugger);
        void SetLevel(LogLevel level);
        void Info(object message, string color = "");
        void InfoG(object message);
        void InfoR(object message);
        void InfoY(object message);
        void InfoB(object message);
        void Warning(object message);
        void Error(object message);
    }
}

