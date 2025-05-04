using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoSidewaysAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary> If 0, 1 then up, if 1, 0 then right, etc </summary>
    public Vector2 side = new Vector2(0, 1);
    public float movementAmount = 20;

    private Vector2 startPos;

    private void Awake()
    {
        startPos = transform.position;
    }

    public void SetStartPos(Vector3 startPos)
    {
        this.startPos = startPos;
    }

    /// <summary>
    /// Performs animation when mouse enters
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOMove(startPos + (movementAmount * side), 0.5f);
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIhover);
    }

    /// <summary>
    /// Performs animation when mouse leaves
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOMove(startPos, 0.5f);
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick);
    }
    private void OnDestroy()
    {
        DOTween.Clear();
    }
}
