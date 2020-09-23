using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework.Module.Database
{
    public struct SqliteQuery : IQuery
    {
        StringBuilder stringBuilder;
        bool isActive;

        public IQuery Active()
        {
            stringBuilder = StringBuilders.Pop();
            stringBuilder.Clear();
            isActive = true;
            return this;
        }

        public void Dispose()
        {
            isActive = false;
            StringBuilders.Push(stringBuilder);
        }

        bool CheckIsActive()
        {
            if(isActive == false)
            {
                Debug.LogWarning("当前Query没有被激活！！！请先使用Active激活");
            }
            return isActive;
        }

        public IQuery Custom(string query)
        {
            if(CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append(query);
            return this;
        }

        public IQuery CreateTable(string tableName, Dictionary<string, string> columnsAndTypes)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"CREATE TABLE {tableName} (");
            int i = 0;
            foreach (var kv in columnsAndTypes)
            {
                stringBuilder.Append($"{kv.Key} {kv.Value}{(i < columnsAndTypes.Count - 1 ? "," : "")}");
                i++;
            }
            stringBuilder.Append(")");
            return this;
        }

        public IQuery DropTable(string tableName)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"DROP TABLE {tableName}");
            return this;
        }

        public IQuery AlterTable(string tableName)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"ALTER TABLE {tableName} ");
            return this;
        }

        public IQuery RenameTo(string tableName)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"RENAME TO {tableName} ");
            return this;
        }

        public IQuery AddColumn(string column, string type)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"ADD COLUMN {column} {type} ");
            return this;
        }

        public IQuery InsertInto(string tableName)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"INSERT INTO {tableName} ");
            return this;
        }

        public IQuery Values(params string[] values)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append("VALUES ");
            for (int i = 0; i < values.Length; i++)
            {
                if (i == 0)
                {
                    stringBuilder.Append("(");
                }

                stringBuilder.Append(values[i]);

                if (i == values.Length - 1)
                {
                    stringBuilder.Append(")");
                }
                else
                {
                    stringBuilder.Append(", ");
                }
            }

            return this;
        }

        public IQuery Select(params string[] columns)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append("SELECT ");
            for (int i = 0; i < columns.Length; i++)
            {
                stringBuilder.Append(columns[i]);
                if (i != columns.Length - 1)
                {
                    stringBuilder.Append(",");
                }
                else
                {
                    stringBuilder.Append(" ");
                }
            }

            return this;
        }

        public IQuery From(string tableName)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"FROM {tableName} ");
            return this;
        }

        public IQuery Where(string condition)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"WHERE {condition}");
            return this;
        }

        public IQuery Update(string tableName)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append($"UPDATE {tableName} ");
            return this;
        }

        public IQuery Set(Dictionary<string, string> columnAndValues)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append("SET ");
            int i = 0;
            foreach (var kv in columnAndValues)
            {
                stringBuilder.Append($"{kv.Key} = {kv.Value}{(i < columnAndValues.Count - 1 ? "," : " ")}");
                i++;
            }
            return this;
        }

        public IQuery Delete()
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append("DELETE ");
            return this;
        }

        public IQuery Like(string str)
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            stringBuilder.Append("LIKE {str} ");
            return this;
        }

        public override string ToString()
        {
            if (CheckIsActive() == false)
            {
                return null;
            }

            Debug.Log(stringBuilder.ToString());
            return stringBuilder.ToString();
        }
    }
}