using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlushieBox : MonoBehaviour, IInteractable
{
    /// <summary>The Player</summary>
    [SerializeField] public PlayerController _player;


    [field: SerializeField] public string interactionPrompt { get; private set; } = "Press F to Pick Up";

    public bool outline => true;
    public void PrimaryInteract(Interactor interactor) 
    { 
        _player.PickupBean();
    }
}
