using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndNightInteraction : MonoBehaviour, IInteractable
{
    [field:SerializeField] public string interactionPrompt { get; private set; }


    public void PrimaryInteract(Interactor interactor)
    {
        FindAnyObjectByType<NightManager>().OnEndNight(true);
    }
}
