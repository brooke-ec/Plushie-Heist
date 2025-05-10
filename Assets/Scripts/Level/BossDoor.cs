using System;
using UnityEngine;

public class BossDoor : MonoBehaviour, IInteractable
{
    [field:SerializeField] public String interactionPrompt{get; private set;}

    [SerializeField] private int sceneIndexToGoTo;

    public void PrimaryInteract(Interactor interactor)
    {
        LoadingSceneController.SurviveNextLoad(PlayerController.instance.gameObject);
        LoadingSceneController.SurviveNextLoad(SharedUIManager.instance.transform.parent.gameObject);
        LoadingSceneController.SurviveNextLoad(MovementUIManager.instance.gameObject);
        LoadingSceneController.SurviveNextLoad(NightManager.instance.transform.parent.gameObject);
        LoadingSceneController.instance.LoadSceneAsync(sceneIndexToGoTo);
    }
}
