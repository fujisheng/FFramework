using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Service.Database
{
    internal sealed class DatabaseService : Service, IDatabaseService
    {
        Dictionary<string, IDatabase> databaseMap = new Dictionary<string, IDatabase>();
        List<string> connectedDatabase = new List<string>();

        /// <summary>
        /// 添加一个数据库 
        /// </summary>
        /// <typeparam name="TDatabase">要添加的数据库类型</typeparam>
        /// <param name="name">数据库名称</param>
        /// <param name="path">数据库路径</param>
        public void AddDatabase<TDatabase>(string name, string path) where TDatabase : class
        {
            var databaseType = typeof(TDatabase);
            var database = Activator.CreateInstance(databaseType, true);

            Utility.Assert.IfNot<IDatabase>(database, new Exception($"{databaseType.FullName} is not Database"));
            Utility.Assert.IfContainsKey(databaseMap, name, new Exception($"already exists database name:{name}"));

            databaseMap.Add(name, database as IDatabase);
        }

        /// <summary>
        /// 添加一个数据库实例
        /// </summary>
        /// <param name="database">要添加的数据库</param>
        public void AddDatabase(IDatabase database)
        {
            Utility.Assert.IfContainsKey(databaseMap, database.Name, new Exception($"already exists database name:{database.Name}"));

            databaseMap.Add(database.Name, database as IDatabase);
        } 

        /// <summary>
        /// 移除一个数据库
        /// </summary>
        /// <param name="name">要移除的数据库名字</param>
        public void RemoveDatabase(string name)
        {
            if(databaseMap.TryGetValue(name, out IDatabase database))
            {
                database.Disconnect();
            }
            databaseMap.Remove(name);
            if (connectedDatabase.Contains(name))
            {
                connectedDatabase.Remove(name);
            }
        }

        /// <summary>
        /// 连接数据库 已经连接的不会重复连接
        /// </summary>
        public void Connect()
        {
            foreach(var database in databaseMap)
            {
                if (!connectedDatabase.Contains(database.Key))
                {
                    database.Value.Connect();
                    connectedDatabase.Add(database.Key);
                }
            }
        }

        /// <summary>
        /// 断开数据库连接
        /// </summary>
        public void Disconnect()
        {
            foreach (var database in databaseMap)
            {
                database.Value.Disconnect();
            }
            connectedDatabase.Clear();
        }

        IDatabase GetDatabase(string databaseName)
        {
            bool get = databaseMap.TryGetValue(databaseName, out IDatabase database);
            if (!get)
            {
                UnityEngine.Debug.Log($"Database:{databaseName} does not exist");
                return null;
            }

            return database;
        }

        /// <summary>
        /// 执行一个命令
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <param name="query">命令/param>
        /// <returns>IDataReader</returns>
        public IDataReader Execute(string databaseName, IQuery query)
        {
            IDatabase database = GetDatabase(databaseName);
            if(database == null)
            {
                return null;
            }

            return database.Execute(query);
        }

        /// <summary>
        /// 执行一串字符
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <param name="queryStr">命令字符串</param>
        /// <returns>IDataReader</returns>
        public IDataReader Execute(string databaseName, string queryStr)
        {
            IDatabase database = GetDatabase(databaseName);
            if (database == null)
            {
                return null;
            }

            return database.Execute(queryStr);
        }

        internal override void OnTearDown()
        {
            Disconnect();
        }
    }
}