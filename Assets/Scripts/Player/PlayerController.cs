using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[SelectionBase]
public class PlayerController : MonoBehaviour
{
    #region Serialized fields
    /// <summary> Maximum player Speed</summary>
    [SerializeField] private float maxSpeed;
    /// <summary> Sensitivity of the mouse </summary>
    [SerializeField] private float lookSensitivity;
    /// <summary>Maximum pitch of the camera</summary>
    [SerializeField] private float MaxPitch;
    /// <summary> Acceleration when on the ground</summary>
    [SerializeField] private float groundAcceleration;

    #endregion

    #region private fields
    /// <summary>Current Player Velocity (NOT the character controller velocity we dont like that one)</summary>
    private Vector3 velocity;
    /// <summary> Character controller asset</summary>
    private CharacterController cc;
    /// <summary> WASD Input  value </summary>
    private Vector2 wasdInput;
    /// <summary> Mouse Input Value </summary>
    private Vector2 lookInput;
    /// <summary> Camera Asset attached to player </summary>
    private Camera cam;
    /// <summary> current pitch of the camera </summary>
    private float camPitch;
    #endregion

    #region core methods
    public void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        
    }

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        LookandRotate();
        groundMove();
        cc.Move(velocity * Time.deltaTime);
    }
    #endregion


    #region private Methods
    /// <summary>
    /// Called every frame to update where the camera looks and to rotate the character
    /// </summary>
    private void LookandRotate()
    {
        transform.Rotate(new Vector3(0, lookInput.x, 0));

        camPitch = Mathf.Clamp(camPitch+lookInput.y, -MaxPitch, MaxPitch);

        cam.transform.localEulerAngles = new Vector3(-camPitch, 0, 0);
        
    }
    /// <summary>
    /// moves you when on ground currently brokey
    /// </summary>
    private void groundMove()
    {
        Debug.Log("wasd"+wasdInput);


        Vector2 wishdir = transform.TransformDirection(wasdInput);
        Debug.Log("wasdinput 1"+wishdir);
        wishdir.Normalize();
        Debug.Log("wishdir2"+wishdir);

        float wishSpeed = wishdir.magnitude * maxSpeed;
        accelerate(wishSpeed, wishdir, groundAcceleration);
    }
    /// <summary>
    /// accelerates the player by a set value or the max acceleration speed which ever is smaller
    /// </summary>
    /// <param name="wishspeed"></param>
    /// <param name="wishdir"></param>
    /// <param name="accel"></param>
    private void accelerate(float wishspeed, Vector2 wishdir, float accel)
    {
        float dotSpeed = Vector2.Dot(wishdir, velocity);
        float addSpeed = wishspeed - dotSpeed;

        if (addSpeed <= 0) return;

        float accelSpeed = accel * Time.deltaTime * wishspeed;
        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;
        Debug.Log("accelwishdir" + wishdir);
        velocity.x += accelSpeed * wishdir.x;
        velocity.z += accelSpeed * wishdir.y;
    }
    #endregion
    #region Input

    public void GetMoveInput(InputAction.CallbackContext ctx)
    {
        wasdInput = ctx.ReadValue<Vector2>();
    }

    public void GetLookInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        lookInput = input * lookSensitivity;
    }

    #endregion
}
