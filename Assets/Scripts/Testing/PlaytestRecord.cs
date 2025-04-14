using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class RecordTest : MonoBehaviour
{
    [SerializeField] private GameObject browser;
    [SerializeField] private TextMeshProUGUI percentage;

    private bool close = false;

    // Start is called before the first frame update
    void Start()
    {
        Recorder.StartRecording();
    }

    private void Update()
    {
        if (browser.activeSelf) percentage.text = $"Recording Uploading ({System.Math.Round(Recorder.progress * 100, 2)}%)";
    }

    public void OnPause()
    {
        if (browser.activeSelf) return;

        browser.SetActive(true);
        string id = Recorder.GenerateId();
        StartCoroutine(FinalizeRecording());
        StartCoroutine(OpenForm());
    }

    IEnumerator FinalizeRecording()
    {

        yield return Recorder.EndRecording();
        yield return Recorder.UploadRecording();
#if UNITY_EDITOR
        if (close) EditorApplication.ExitPlaymode();
#else
        if (close) Application.Quit();
#endif
        else close = true;
    }

    IEnumerator OpenForm()
    {
        yield return new WaitForSeconds(8);
        Application.OpenURL($"https://docs.google.com/forms/d/e/1FAIpQLSfmXc1aChPw2Bv-Jd704ak2xHITVK54Mgwz8AD2Sot2nAB58A/viewform?usp=pp_url&entry.1477077091={Recorder.id}");
#if UNITY_EDITOR
        if (close) EditorApplication.ExitPlaymode();
#else
        if (close) Application.Quit();
#endif
        else close = true;
    }

    private void OnDestroy()
    {
        if (Recorder.IsActive()) Recorder.EndRecording();
    }
}
