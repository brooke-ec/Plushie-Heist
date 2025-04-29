using System;
using UnityEngine;

/// <summary>
/// Rotates the attached <see cref="GameObject"/> to always face the camera on the specified axes
/// </summary>
[ExecuteInEditMode]
public class BillboardController : MonoBehaviour
{
    [SerializeField] private BillboardAxis axes = BillboardAxis.X | BillboardAxis.Y | BillboardAxis.Z;

    void Update()
    {
        Vector3 rotation = transform.eulerAngles;
        Vector3 camera = Vector3.zero;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Camera[] cameras = UnityEditor.SceneView.GetAllSceneCameras();
            if (cameras.Length > 0) camera = cameras[0].transform.eulerAngles;
        } else // Else run next line (will always run in builds)
#endif
        camera = Camera.main.transform.eulerAngles;

        if (axes.HasFlag(BillboardAxis.X)) rotation.x = camera.x;
        if (axes.HasFlag(BillboardAxis.Y)) rotation.y = camera.y;
        if (axes.HasFlag(BillboardAxis.Z)) rotation.z = camera.z;

        transform.eulerAngles = rotation;
    }

    [Flags]
    public enum BillboardAxis
    {
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2
    }
}
