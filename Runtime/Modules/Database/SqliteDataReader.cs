using System;
using System.Collections;
using System.Data;

namespace Framework.Module.Database
{
    public class SqliteDataReader : IDataReader
    {
        Mono.Data.Sqlite.SqliteDataReader sqliteDataReader;
        public SqliteDataReader(Mono.Data.Sqlite.SqliteDataReader sqliteDataReader)
        {
            this.sqliteDataReader = sqliteDataReader;
        }

        public object this[int i] => sqliteDataReader[i];

        public object this[string name] => sqliteDataReader[name];

        public int Depth => sqliteDataReader.Depth;

        public bool IsClosed => sqliteDataReader.IsClosed;

        public int RecordsAffected => sqliteDataReader.RecordsAffected;

        public int FieldCount => sqliteDataReader.FieldCount;

        public void Close()
        {
            sqliteDataReader.Close();
        }

        public void Dispose()
        {
            sqliteDataReader.Dispose();
        }

        public bool GetBoolean(int i)
        {
            return sqliteDataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return sqliteDataReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return sqliteDataReader.GetBytes(i, FieldCount, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return sqliteDataReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return sqliteDataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public System.Data.IDataReader GetData(int i)
        {
            return sqliteDataReader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return sqliteDataReader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return sqliteDataReader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return sqliteDataReader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return sqliteDataReader.GetDouble(i);
        }

        public IEnumerator GetEnumerator()
        {
            return sqliteDataReader.GetEnumerator();
        }

        public System.Type GetFieldType(int i)
        {
            return sqliteDataReader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return sqliteDataReader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return sqliteDataReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return sqliteDataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return sqliteDataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return sqliteDataReader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return sqliteDataReader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return sqliteDataReader.GetOrdinal(name);
        }

        public DataTable GetSchemaTable()
        {
            return sqliteDataReader.GetSchemaTable();
        }

        public string GetString(int i)
        {
            return sqliteDataReader.GetString(i);
        }

        public object GetValue(int i)
        {
            return sqliteDataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return sqliteDataReader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return sqliteDataReader.IsDBNull(i);
        }

        public bool NextResult()
        {
            return sqliteDataReader.NextResult();
        }

        public bool Read()
        {
            return sqliteDataReader.Read();
        }
    }
}

