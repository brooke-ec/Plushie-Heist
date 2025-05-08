using System.Collections;
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
        //LOAD LIST OF SAVE FILES

        /*
        List<SaveFile?> saveFiles = new List<SaveFile>();
        for(int i=0;i<saveFiles.Count; i++)
        {
            GameObject savefileUI = Instantiate(saveFileIconPrefab, saveFileContentTransform);
            savefileUI.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = saveFiles[i].totalTimePlayed;

            savefileUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => LoadSave(saveFiles[i]));
        }

        noSavedGamesText.SetActive(saveFiles.Count == 0);
        */
    }

    /*
    public void LoadSave(SaveFile? saveFile)
    {
        //TO-DO

        //EXAMPLE
        savefileUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => LoadingSceneController.instance.LoadSceneAsync("Shop 1"));
    }*/

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
