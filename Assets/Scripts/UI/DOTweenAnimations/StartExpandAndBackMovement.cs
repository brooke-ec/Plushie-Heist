using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandAndBackMovement : MonoBehaviour
{
    private Vector3 startScale;
    public float endScale = 1.05f;
    void Start()
    {
        startScale = transform.localScale;
        transform.DOScale(startScale * endScale, 0.4f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }
}
