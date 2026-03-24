namespace Framework.Module.Log
{
    public interface ILogger
    {
        void Info(object message, string color);
        void Warning(object message);
        void Error(object message);
    }
}