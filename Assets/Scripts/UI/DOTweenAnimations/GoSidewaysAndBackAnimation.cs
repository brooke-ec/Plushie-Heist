using DG.Tweening;
using UnityEngine;

public class GoSidewaysAndBackAnimation : MonoBehaviour
{
    /// <summary> If 0, 1 then up, if 1, 0 then right, etc </summary>
    public Vector2 side = new Vector2(0, 1);
    public float movementAmount = 20;

    private Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
        DoAnimation();
    }

    public void SetStartPos(Vector3 startPos)
    {
        this.startPos = startPos;
    }

    public void DoAnimation()
    {
        transform.DOMove(startPos + (movementAmount * side), 0.5f, true).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }
}
