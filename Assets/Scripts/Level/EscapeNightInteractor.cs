using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeNightInteractor : MonoBehaviour, IInteractable
{
    [field:SerializeField] public string interactionPrompt { get; private set; }


    public void PrimaryInteract(Interactor interactor)
    {
        NightManager.instance.OnEndNight(true);
    }
}
