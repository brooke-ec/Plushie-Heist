using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class Recorder
{
    public const string UPLOAD_URL = "https://plushie-heist.nimahost.net/upload/{0}";
    public const string FILE_PATH = "recording.mp4";

    private static Process process = null;

    /// <summary>
    /// The callback used with <see cref="EnumThreadWindows"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms633496(v=vs.85)">Read More</see>
    /// </remarks>
    /// <param name="hwnd">The handle to an enumerated window.</param>
    /// <param name="lParam">The arbitrary value passed by <see cref="EnumThreadWindows"/>.</param>
    /// <returns>TRUE to continue enumeration.</returns>
    private delegate bool EnumThreadDelegate(IntPtr hwnd, IntPtr lParam);

    /// <summary>
    /// Retrieves the thread identifier of the calling thread.
    /// </summary>
    /// <remarks>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getcurrentthreadid">Read More</see>
    /// </remarks>
    /// <returns>The thread identifier of the calling thread.</returns>
    [DllImport("Kernel32.dll")] private static extern int GetCurrentThreadId();

    /// <summary>
    /// Invokes a callback for each nonchild window associated with the thread <i>dwThreadId</i>
    /// while the return value of the callback is TRUE.
    /// </summary>
    /// <remarks>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumthreadwindows">Read More</see>
    /// </remarks>
    /// <param name="dwThreadId">The identifier of the thread whose windows are to be enumerated.</param>
    /// <param name="lpfn">A callback to recieve window handles. See <see cref="EnumThreadDelegate"/>.</param>
    /// <param name="lParam">An arbitrary value to be passed to the callback function.</param>
    /// <returns>
    /// If the callback function returned TRUE for all windows
    /// </returns>
    [DllImport("user32.dll")] private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

    /// <summary>
    /// Retrieves the handle of the main window of the application.
    /// </summary>
    /// <returns>The handle of the application's main window.</returns>
    private static IntPtr GetWindowHandle()
    {
        IntPtr returnHwnd = IntPtr.Zero;
        var threadId = GetCurrentThreadId();
        EnumThreadWindows(threadId,
            (hWnd, lParam) => {
                if (returnHwnd == IntPtr.Zero)
                    returnHwnd = hWnd;
                return false;
            }, IntPtr.Zero);
        return returnHwnd;
    }

    /// <summary>
    /// Starts the FFmpeg screen recorder.
    /// </summary>
    public static void StartRecording()
    {
        if (process != null) throw new Exception("Recorder already active");
        process = new Process();
        process.StartInfo.FileName = Application.streamingAssetsPath + "\\ffmpeg.exe";
        process.StartInfo.Arguments = $"-y -f gdigrab -framerate 30 -i hwnd={GetWindowHandle()} {FILE_PATH}";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();

        // Attach event handler to cleanup
        process.EnableRaisingEvents = true;
        process.Exited += new EventHandler((object sender, EventArgs e) => {
            process.Close();
            process = null;
        });
    }

    /// <summary>
    /// Returns wether the FFmpeg recorder process is currently running.
    /// </summary>
    /// <returns>True if the recorder process running.</returns>
    public static bool IsActive()
    {
        return process != null && !process.HasExited;
    }

    /// <summary>
    /// Requests FFmpeg to finish recording.
    /// </summary>
    /// <returns>Suspends the coroutine execution until FFmpeg has exited.</returns>
    public static CustomYieldInstruction EndRecording()
    {
        if (process == null) throw new Exception("Recorder is not active");
        process.StandardInput.Write("q");
        process.StandardInput.Flush();

        return new RecorderWaiter();
    }

    /// <summary>
    /// Custom yield instruction to suspend until the process has exited.
    /// </summary>
    /// <remarks>
    /// <see href="https://docs.unity3d.com/6000.0/Documentation/ScriptReference/CustomYieldInstruction.html">Read More</see>
    /// </remarks>
    private class RecorderWaiter : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                return IsActive();
            }
        }
    }

    /// <summary>
    /// Generates a valid Id for the playtest dashboard
    /// </summary>
    /// <returns></returns>
    public static string GenerateId()
    {
        return Guid.NewGuid().ToString().Replace("-", "");
    }

    /// <summary>
    /// Upload the most recent recording to the playtest dashboard.
    /// </summary>
    /// <exception cref="Exception">Thrown if the server does not reply with successful status code.</exception>
    public static IEnumerator UploadRecording()
    {
        return UploadRecordingTo(String.Format(UPLOAD_URL, ""));
    }

    /// <summary>
    /// Upload the most recent recording to the playtest dashboard.
    /// </summary>
    /// <param name="id">An Id used to identify this recording.</param>
    /// <exception cref="Exception">Thrown if the server does not reply with successful status code.</exception>
    public static IEnumerator UploadRecording(string id)
    {
        return UploadRecordingTo(String.Format(UPLOAD_URL, id));
    }

    /// <summary>
    /// Upload the most recent recording to the specified <paramref name="URL"/>.
    /// </summary>
    /// <exception cref="Exception">Thrown if the server does not reply with successful status code.</exception>
    private static IEnumerator UploadRecordingTo(string URL)
    {
        if (IsActive()) throw new Exception("Recorder is currently active");

        using UnityWebRequest r = new UnityWebRequest(URL.TrimEnd('/'), UnityWebRequest.kHttpVerbPUT);
        r.uploadHandler = new UploadHandlerFile(FILE_PATH);
        r.downloadHandler = new DownloadHandlerBuffer();

        yield return r.SendWebRequest();
        if (r.result != UnityWebRequest.Result.Success) throw new Exception(r.downloadHandler.text);
    }
}
