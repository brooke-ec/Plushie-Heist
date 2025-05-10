using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpandMovementAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 startScale;
    public float endScale = 1.05f;

    private void Start()
    {
        startScale = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(startScale * endScale, 0.5f);
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIhover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(startScale, 0.5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOScale(startScale, 0.5f);
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick);
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }
}
