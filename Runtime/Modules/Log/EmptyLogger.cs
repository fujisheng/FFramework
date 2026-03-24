namespace Framework.Module.Log
{
    public class EmptyLogger : ILogger
    {
        public virtual void Info(object message, string color = "white"){ }
        public virtual void Warning(object messgae){}
        public virtual void Error(object message){}
    }
}