using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Database
{
    internal sealed class DatabaseManager : Module, IDatabaseManager
    {
        Dictionary<string, IDatabase> databaseMap = new Dictionary<string, IDatabase>();

        internal DatabaseManager()
        {
            IDatabase userData = new SqliteDatabase("userData", Application.persistentDataPath);
            IDatabase levelsData = new SqliteDatabase("levelsData");
            AddDatabase(userData);
            AddDatabase(levelsData);
        }

        public void AddDatabase(IDatabase database)
        {
            if (databaseMap.ContainsKey(database.Name))
            {
                Debug.LogError($"已经包含这个数据库！！！{database.Name}");
                return;
            }

            databaseMap.Add(database.Name, database);
        }

        public void Connect()
        {
            foreach(var database in databaseMap)
            {
                database.Value.Connect();
            }
        }

        public void Disconnect()
        {
            foreach (var database in databaseMap)
            {
                database.Value.Disconnect();
            }
        }

        public IDatabase GetDatabase(string databaseName)
        {
            bool get = databaseMap.TryGetValue(databaseName, out IDatabase database);
            if (!get)
            {
                Debug.Log($"没有这个数据库！！！{databaseName}");
                return null;
            }

            return database;
        }

        public IDataReader ExecuteQuery(string databaseName, IQuery query)
        {
            IDatabase database = GetDatabase(databaseName);
            if(database == null)
            {
                return null;
            }

            return database.ExecuteQuery(query);
        }

        public IDataReader ExecuteString(string databaseName, string queryStr)
        {
            IDatabase database = GetDatabase(databaseName);
            if (database == null)
            {
                return null;
            }

            return database.ExecuteString(queryStr);
        }

        internal override void OnTearDown()
        {
            Disconnect();
        }
    }
}