using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Framework.Module.Database
{
    public interface IDataReader: IEnumerable, System.Data.IDataReader, IDataRecord, IDisposable
    {
    }
}