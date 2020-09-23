namespace Framework.Module.Database
{
    public interface IDatabase
    {
        string Name { get; }
        void Connect();
        void Disconnect();
        IDataReader ExecuteQuery(IQuery query);
        IDataReader ExecuteString(string queryStr);
        void ClearTables();
    }
}