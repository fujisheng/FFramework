using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Entity
{
    public class Entity : IEntity
    {
        public int id;
        public GameObject gameObject;

        public Entity(GameObject gameObject, int groupId)
        {
            int localEntityId = gameObject.GetInstanceID();
            string id2String = Convert.ToString(localEntityId, 2);
            int length = id2String.Length;
            this.id = groupId << length + localEntityId;
            this.gameObject = gameObject;
        }
    }
}