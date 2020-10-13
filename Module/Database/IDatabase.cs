namespace Framework.Module.Database
{
    public interface IDatabase
    {
        string Name { get; }
        string Path { get; }
        void Connect();
        
        void Disconnect();
        
        IDataReader Execute(IQuery query);
        
        IDataReader Execute(string queryStr);
        
        void ClearTables();
    }
}