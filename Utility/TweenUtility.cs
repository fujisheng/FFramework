using Coffee.UIExtensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Utility
{
    public static class TweenUtility
    {
        /// <summary>
        /// 做一个gameobject的整体fade
        /// </summary>
        /// <param name="rectTransform"></param>
        public static Sequence DOAllFade(this GameObject gameObject, float endValue, float duration)
        {
            Sequence sequence = DOTween.Sequence();
            foreach (var com in gameObject.GetComponentsInChildren<Graphic>(true))
            {
                if (!(com is Image || com is Text || com is RawImage))
                {
                    continue;
                }

                sequence.Join(com.DOFade(endValue, duration));
            }
            return sequence;
        }

        /// <summary>
        /// 设置所有的Graphic的a
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="fade"></param>
        public static void SetAllFade(this GameObject gameObject, float fade)
        {
            foreach (var com in gameObject.GetComponentsInChildren<Graphic>(true))
            {
                if (!(com is Image || com is Text || com is RawImage))
                {
                    continue;
                }

                var currentColor = com.color;
                currentColor.a = fade;
                com.color = currentColor;
            }
        }

        public static void SetAllGray(this GameObject gameObject, bool gray)
        {
            foreach (var com in gameObject.GetComponentsInChildren<UIEffect>(true))
            {
                if(com.effectMode == EffectMode.Grayscale)
                {
                    com.effectFactor = gray ? 1 : 0;
                    com.colorFactor = gray ? 0 : 1;
                }
            }
        }
    }
}