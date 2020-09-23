using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.Component.UI
{
    public class AnimationButton : Button
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (interactable == false)
            {
                return;
            }
            transform.DOKill();
            transform.localScale = Vector3.one;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.one * 0.9f, 0.15f));
            sequence.Play();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (interactable == false)
            {
                return;
            }
            transform.DOKill();
            transform.localScale = Vector3.one;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.one * 1.1f, 0.15f));
            sequence.Append(transform.DOScale(Vector3.one * 0.95f, 0.1f));
            sequence.Append(transform.DOScale(Vector3.one, 0.1f));
            sequence.Play();
        }
    }
}