namespace Framework.Service.Database
{
    public interface IDatabaseService
    {
        void AddDatabase<TDatabase>(string name, string path) where TDatabase : class;
        void AddDatabase(IDatabase database);
        void RemoveDatabase(string name);
        void Connect();
        void Disconnect();
        IDataReader Execute(string databaseName, IQuery query);
        IDataReader Execute(string databaseName, string query);
    }
}