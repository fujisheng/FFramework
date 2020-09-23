using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Entity
{
    public interface IEntityGroup
    {
        void Add(IEntity entity);
    }
}