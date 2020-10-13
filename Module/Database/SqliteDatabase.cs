using Framework.Utility;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Module.Database
{
    public class SqliteDatabase : IDatabase
    {
        SqliteConnection connection;
        SqliteCommand command;
        IDataReader reader;
        string connectionPath = string.Empty;

        public string Name { get; }
        public string Path { get; }
        internal SqliteDatabase() { }
        public SqliteDatabase(string dbName, string path = null)
        {
            Name = dbName;
            Path = path;

            if (string.IsNullOrEmpty(path))
            {
                path = Application.streamingAssetsPath;
            }

            string completePath = $"{Application.persistentDataPath}/{dbName}.db";
            string streamingPath = $"{Application.streamingAssetsPath}/{dbName}.db";

#if UNITY_ANDROID
            if (!File.Exists(completePath) )//&& File.Exists(streamingPath))
            {
                UnityWebRequest request = UnityWebRequest.Get(streamingPath);
                request.SendWebRequest();
                while (true)
                {
                    if (request.isDone)
                    {
                        break;
                    }
                }
                File.WriteAllBytes(completePath, request.downloadHandler.data);
                Debug.Log($"writeAllBytes=>{request.downloadHandler.data.Length}");
                request.Dispose();
            }
#endif

#if UNITY_IOS || UNITY_EDITOR
            connectionPath = "Data Source = " + $"{path}/{dbName}.db";
#elif UNITY_ANDROID
            connectionPath = "URI = file:" + completePath;
#endif

            if (!File.Exists(completePath))
            {

                connection = new SqliteConnection(connectionPath);
                connection.Open();
                connection.Close();
            }
        }

        public void Connect()
        {
            try
            {
                connection = new SqliteConnection(connectionPath);
                connection.Open();
                Debug.Log($"Connected {connectionPath}");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        public void Disconnect()
        {
            if (command != null)
            {
                command.Cancel();
            }
            command = null;

            if (reader != null)
            {
                reader.Close();
            }
            reader = null;

            if (connection != null)
            {
                connection.Close();
            }
            connection = null;

            Debug.Log($"Disconnection  {connectionPath}");
        }

        public IDataReader Execute(IQuery query)
        {
            command = connection.CreateCommand();
            command.CommandText = query.ToString();
            try
            {
                reader = new SqliteDataReader(command.ExecuteReader());
                return reader;
            }
            catch(Exception ex)
            {
                Debug.Log($"ExecuteQueryError:{ex}");
                return null;
            }
        }

        public IDataReader Execute(string queryString)
        {
            command = connection.CreateCommand();
            command.CommandText = queryString;
            try
            {
                reader = new SqliteDataReader(command.ExecuteReader());
                return reader;
            }
            catch (Exception ex)
            {
                Debug.Log($"ExecuteQueryError:{queryString}:{ex}");
                return null;
            }
        }

        public void ClearTables()
        {
            string queryString = "SELECT * FROM sqlite_master WHERE type = 'table'";
            IDataReader dataReader = Execute(queryString);
            Dictionary<int, Dictionary<string, object>> dic = DataReaderUtility.ReaderToDictionarys(dataReader);

            if (dic == null)
            {
                return;
            }

            foreach (var d in dic)
            {
                if (d.Value == null)
                {
                    continue;
                }

                foreach (var data in d.Value)
                {
                    if (data.Key == "name")
                    {
                        Execute($"DROP TABLE {data.Value}");
                    }
                }
            }
        }
    }
}