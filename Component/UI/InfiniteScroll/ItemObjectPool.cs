using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Framework.Component.UI
{
    public class ItemObjectPool : ObjectPool<RectTransform>
    {
        public override RectTransform New()
        {
            throw new System.NotImplementedException();
        }
    }
}