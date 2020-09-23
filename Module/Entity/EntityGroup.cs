using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Entity
{
    public class EntityGroup : IEntityGroup
    {
        public int groupId;
        public int assetName;
        public List<IEntity> entities = new List<IEntity>();

        public void Add(IEntity entity)
        {

        }
    }
}