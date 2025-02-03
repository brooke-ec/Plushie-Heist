using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
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
    /// <summary> Friction of ground</summary>
    [SerializeField] private float groundFriction;
    ///<summary>Gravity Value</summary>
    [SerializeField] private float gravity;
    ///<summary>Velocity given for jumping</summary>
    [SerializeField] private float jumpSpeed;
    ///<summary>Number of Jumps</summary>
    [SerializeField] private int noJumps;
    ///<summary>time allowed to press jump before landing for the input to still register in seconds</summary>
    [SerializeField] private float jumpWindow;

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
    /// <summary> No of jumps used</summary>
    private int jumpsUsed;
    /// <summary>user wants to jump</summary>
    private bool wishJump;
    /// <summary>Time since last jump input</summary>
    private float jumpTimer;
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
        wishJump = false;
    }

    public void Update()
    {
        ApplyGravity();
        ApplyJumps();
        LookandRotate();
        Move();
        cc.Move(velocity * Time.deltaTime);
    }
    #endregion


    #region Movement Methods
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
    /// Deals movement in the x and z axis
    /// </summary>
    private void Move()
    {
        Vector3 wishdir = new Vector3(wasdInput.x,0,wasdInput.y);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();

        float wishSpeed = wishdir.magnitude * maxSpeed;
        if (cc.isGrounded)
        {
            ApplyFriction();
            Accelerate(wishSpeed, wishdir, groundAcceleration);
        }
    }
    /// <summary>
    /// Applys friciton for being on the ground
    /// </summary>
    private void ApplyFriction()
    {
  
        float curSpeed = velocity.magnitude;

        if (curSpeed <= 0) return;
        float newSpeed = curSpeed - Time.deltaTime * curSpeed * groundFriction;

        if (newSpeed < 0) newSpeed = 0;

        newSpeed /= curSpeed;

        velocity.x *= newSpeed;
        velocity.z *= newSpeed;
    }

    /// <summary>
    /// accelerates the player by a set value or the max acceleration speed which ever is smaller
    /// </summary>
    /// <param name="wishspeed"></param>
    /// <param name="wishdir"></param>
    /// <param name="accel"></param>
    private void Accelerate(float wishspeed, Vector3 wishdir, float accel)
    {
        float dotSpeed = Vector3.Dot(wishdir, velocity);
        float addSpeed = wishspeed - dotSpeed;

        if (addSpeed <= 0) return;

        float accelSpeed = accel  *Time.deltaTime * wishspeed;
        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;
        Debug.Log("accelwishdir" + wishdir);
        velocity.x += accelSpeed * wishdir.x;
        velocity.z += accelSpeed * wishdir.z;
    }

    private void ApplyGravity()
    {
        if (cc.isGrounded) return;
        velocity.y += Time.deltaTime * -gravity;
    }

    private void ApplyJumps()
    {
        if (cc.isGrounded) jumpsUsed = 0;
        if (wishJump && jumpsUsed < noJumps)
        {
            velocity.y = jumpSpeed;
            wishJump = false;
            jumpsUsed++;
        }
        if (wishJump && jumpTimer < jumpWindow) jumpTimer += Time.deltaTime;
        if (wishJump && jumpTimer > jumpWindow) wishJump = false;
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

    public void getJump(InputAction.CallbackContext ctx)
    {
        wishJump = true;
    }

    #endregion
}
