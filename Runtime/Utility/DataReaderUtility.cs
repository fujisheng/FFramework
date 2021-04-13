using Framework.Module.Database;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility
{
    public static class DataReaderUtility
    {
        /// <summary>
        /// 将DataReader里面的数据转换成Dictionary
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Dictionary<int,Dictionary<string,object>> ReaderToDictionarys(IDataReader reader)
        {
            if (reader == null)
            {
                return null;
            }

            if (reader.IsClosed)
            {
                Debug.Log("dataReader已经被关闭！！！");
                return null;
            }
            Dictionary<int, Dictionary<string, object>> result = new Dictionary<int, Dictionary<string, object>>();
            int fieldCount = reader.FieldCount;
            int column = 0;
            while (reader.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int i = 0; i < fieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader[i]);
                }
                result.Add(column, row);
                column++;
            }
            return result.Count == 0 ? null : result;
        }
    }
}