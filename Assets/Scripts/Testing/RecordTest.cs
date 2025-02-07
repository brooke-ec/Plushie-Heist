using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RecordTest : MonoBehaviour
{
    [SerializeField] private Text text;

    // Start is called before the first frame update
    void Start()
    {
        Recorder.StartRecording();
    }

    public void End()
    {
        StartCoroutine(FinalizeRecording());
    }

    IEnumerator FinalizeRecording()
    {
        yield return Recorder.EndRecording();
        text.text = "Uploading...";
    }

    private void OnDestroy()
    {
        if (Recorder.IsActive()) Recorder.EndRecording();
    }
}
