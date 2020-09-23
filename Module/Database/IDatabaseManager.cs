namespace Framework.Module.Database
{
    public interface IDatabaseManager
    {
        void AddDatabase(IDatabase database);
        void Connect();
        void Disconnect();
        IDatabase GetDatabase(string databaseName);
        IDataReader ExecuteQuery(string databaseName, IQuery query);
    }
}