using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Framework.Component.UI
{
    public class ItemObjectPool : ObjectPool<RectTransform>
    {
        protected override RectTransform New()
        {
            throw new System.NotImplementedException();
        }
    }
}