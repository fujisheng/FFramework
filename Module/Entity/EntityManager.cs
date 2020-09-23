using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Module.Resource;

namespace Framework.Module.Entity
{
    public class EntityManager
    {
        Dictionary<string, int> assetNameGroupIdMapping = new Dictionary<string, int>();
        Dictionary<int, IEntityGroup> groups = new Dictionary<int, IEntityGroup>();
        IResourceManager resourceManager;

        public IEntity CreateEntitySync(string name)
        {
            IAsset asset = resourceManager.LoadSync<GameObject>(name);
            GameObject obj = Object.Instantiate(asset.asset as GameObject);
            asset.Require(obj);
            bool getId = assetNameGroupIdMapping.TryGetValue(name, out int groupId);
            if (!getId)
            {
                groupId = name.GetHashCode();
                assetNameGroupIdMapping.Add(name, groupId);
            }
            IEntity entity = new Entity(obj, groupId);
            bool getGroup = groups.TryGetValue(groupId, out IEntityGroup group);
            if (!getGroup)
            {
                IEntityGroup newGroup = new EntityGroup();
                newGroup.Add(entity);
                groups.Add(groupId, newGroup);
            }
            return entity;
        }
    }
}