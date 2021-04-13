namespace Framework.Module.Debugger
{
    public enum DebugLevel
    {
        None = 0,
        Normal = 1,
        Warning = 1 << 1,
        Error = 1 << 2,
        Red = 1 << 3,
        Green = 1 << 4,
        Blue = 1 << 5,
        Yellow = 1 << 6,
        All = Normal | Warning | Error | Red | Green | Blue | Yellow,
    }
}
