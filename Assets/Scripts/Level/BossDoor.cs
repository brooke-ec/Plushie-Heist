using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDoor : MonoBehaviour, IInteractable
{
    [field:SerializeField] public String interactionPrompt{get; private set;}

    [SerializeField] private int sceneIndexToGoTo;

    public void PrimaryInteract(Interactor interactor)
    {
        DontDestroyOnLoad(PlayerController.instance.gameObject);
        DontDestroyOnLoad(SharedUIManager.instance.transform.parent.gameObject);
        DontDestroyOnLoad(MovementUIManager.instance.gameObject);
        DontDestroyOnLoad(NightManager.instance.transform.parent.gameObject);
        LoadingSceneController.instance.LoadSceneAsync(sceneIndexToGoTo);
    }
}
