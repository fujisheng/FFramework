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
        public string Name { get; private set; }
        string connectionPath = string.Empty;

        public SqliteDatabase(string dbName, string path = null)
        {
            Name = dbName;
            if (string.IsNullOrEmpty(path))
            {
                path = Application.streamingAssetsPath;
            }

            string completePath = $"{Application.persistentDataPath}/{Name}.db";
            string streamingPath = $"{Application.streamingAssetsPath}/{Name}.db";

            //Debug.Log($"包含persistent=>{File.Exists(completePath)}    {completePath}");
            //Debug.Log($"包含文件=>{File.Exists(streamingPath)}   {streamingPath}");

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
            connectionPath = "Data Source = " + $"{path}/{Name}.db";
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

        public IDataReader ExecuteQuery(IQuery query)
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

        public IDataReader ExecuteString(string queryString)
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
            IDataReader dataReader = ExecuteString(queryString);
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
                        ExecuteString($"DROP TABLE {data.Value}");
                    }
                }
            }
        }
    }
}