using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Resource
{
    public enum DownloadState
    {
        HeadRequest,
        BodyRequest,
        FinishRequest,
        Completed,
    }
}
