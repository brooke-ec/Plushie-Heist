using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevelInteraction : MonoBehaviour, IInteractable
{
    [field:SerializeField] public String interactionPrompt{get; private set;}

    [SerializeField] private int sceneIndexToGoTo;

    public void PrimaryInteract(Interactor interactor)
    {
        LoadingSceneController.instance.LoadSceneAsync(sceneIndexToGoTo);
    }


}
