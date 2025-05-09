using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject saveFileIconPrefab;
    public Transform saveFileContentTransform;
    public GameObject noSavedGamesText;
    public Dialogue dialoguePrefab;
    private void Start()
    {
        MakeSavesAsUI();
    }

    public void MakeSavesAsUI()
    {
        SaveFile[] saveFiles = SaveManager.GetSaveList();
        foreach (var saveFile in saveFiles)
        {
            GameObject savefileUI = Instantiate(saveFileIconPrefab, saveFileContentTransform);
            savefileUI.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = saveFile.formattedPlaytime;

            savefileUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => LoadSave(saveFile));
        }

        noSavedGamesText.SetActive(saveFiles.Length == 0);
    }


    public void LoadSave(SaveFile saveFile)
    {
        SaveManager.slot = saveFile.slot;
        LoadingSceneController.instance.LoadSceneAsync(1);
    }

    public void NewGame()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        Dialogue dialogue = Instantiate(dialoguePrefab, transform.GetChild(1));
        
        //TO-DO-SAVING dialogue.onDialogueEnd = (LOAD NIGHT SCENE METHOD)
        dialogue.SetUp(Dialogue.DialogueEnum.mainMenu);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
