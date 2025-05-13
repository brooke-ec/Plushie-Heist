using System;
using UnityEngine;

public class BossDoor : MonoBehaviour, IInteractable
{
    string IInteractable.interactionPrompt => "Press E to " + (lastLevel ? "Fight the Next Boss!!!" : "Progress to the Next Floor...");
    public bool lastLevel => NightManager.instance.levelProgress >= (SharedUIManager.instance.plushieIndex / 2);

    public void PrimaryInteract(Interactor interactor)
    {
        LoadingSceneController.SurviveNextLoad(PlayerController.instance.gameObject);
        LoadingSceneController.SurviveNextLoad(SharedUIManager.instance.transform.parent.gameObject);
        LoadingSceneController.SurviveNextLoad(MovementUIManager.instance.gameObject);
        LoadingSceneController.SurviveNextLoad(NightManager.instance.transform.parent.gameObject);

        bool boss = lastLevel;
        NightManager.instance.levelProgress++;
        LoadingSceneController.instance.LoadSceneAsync(boss ? 3 : 2);
    }
}
