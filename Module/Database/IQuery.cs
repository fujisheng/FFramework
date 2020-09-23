using System;
using System.Collections.Generic;

namespace Framework.Module.Database
{
    public interface IQuery : IDisposable
    {
        IQuery Active();
        IQuery Custom(string query);
        IQuery CreateTable(string tableName, Dictionary<string, string> columnsAndTypes);
        IQuery DropTable(string tableName);
        IQuery AlterTable(string tableName);
        IQuery RenameTo(string tableName);
        IQuery AddColumn(string column, string type);
        IQuery InsertInto(string tableName);
        IQuery Values(params string[] values);
        IQuery Select(params string[] columns);
        IQuery From(string tableName);
        IQuery Where(string condition);
        IQuery Update(string tableName);
        IQuery Set(Dictionary<string, string> columnsAndValues);
        IQuery Delete();
        IQuery Like(string str);
        string ToString();
    }
}

