using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;


[SelectionBase]
public class PlayerController : MonoBehaviour
{
    #region Serialized fields
    /// <summary> Maximum player Speed</summary>
    [SerializeField] private float maxSpeed;
    /// <summary> Maximum player Sprint Speed rework to be an increase on top of the maxspeed</summary>
    [SerializeField] private float sprintMaxSpeed;
    /// <summary> The players Stamina</summary>
    [SerializeField] private float stamina;
    /// <summary> The players Max Stamina</summary>
    [SerializeField] private float maxStamina;
    /// <summary> The speed at which the players stamina is drained</summary>
    [SerializeField] private float staminaDrain;
    /// <summary> The speed at which the player stamina recovers</summary>
    [SerializeField] private float staminaRecover;
    /// <summary>  player Speed</summary>
    [SerializeField] private float walkSpeed;
    ///<summary>Crouch speed</summary>
    [SerializeField] private float crouchSpeed;
    /// <summary> Sensitivity of the mouse </summary>
    [SerializeField] private float lookSensitivity;
    /// <summary>Maximum pitch of the camera</summary>
    [SerializeField] private float MaxPitch;
    ///<summary>Acceleration when in the air</summary>
    [SerializeField] private float airAcceleration;
    /// <summary> Friction of ground</summary>
    [SerializeField] private float groundFriction;
    ///<summary>Gravity Value</summary>
    [SerializeField] private float gravity;
    ///<summary>Gliding Gravity</summary>
    [SerializeField] private float glideGravity;
    ///<summary>Velocity given for jumping</summary>
    [SerializeField] private float jumpSpeed;
    ///<summary>Number of Jumps</summary>
    [SerializeField] private int noJumps;
    ///<summary>time allowed to press jump before landing for the input to still register in seconds</summary>
    [SerializeField] private float jumpWindow;
    ///<summary>Coyote Time, the time after leaving a ledge you can still jump</summary>
    [SerializeField] private float coyoteTime;
    ///<summary>Speed threshold for sliding MUST BE BIGGER THAN 5.1</summary>
    [SerializeField] private float slideThreshold;
    ///<summary>Friction when sliding</summary>
    [SerializeField] private float slideFriction;
    /// <summary>The Ability that the Player currently has equiped</summary>
    [SerializeField] private Ability currentAbility;
    ///<summary>The new acceleration speed when the palyer boosts</summary>
    [SerializeField] private float boostSpeedCap;
    ///<summary>The base Ground Acceleration</summary>
    [SerializeField] private float baseGroundAcceleration;
    ///<summary>The boosting Ground Acceleration</summary>
    [SerializeField] private float boostGroundAcceleration;
    ///<summary>The maximum boost time</summary>
    [SerializeField] private float maxBoostTime;
    ///<summary>The time the player will be boosting for</summary>
    [SerializeField] private float boostTime;
    ///<summary>The speed at which the Player regains the boost at</summary>
    [SerializeField] private float boostRecoverSpeed;
    /// <summary>the speed given when dashing</summary>
    [SerializeField] private float dashSpeed;
    /// <summary>The number of Dashes available</summary>
    [SerializeField] private int noDashes;
    /// <summary> Amount of time to regain a dash charge</summary>
    [SerializeField] private float dashRechargeTime;
    /// <summary> Amount of time between dashes</summary>
    [SerializeField] private float dashCooldownTime;

    /// <summary>Distance for detecting walls</summary>
    [SerializeField] private float wallDetectionDistance;
    /// <summary> minimum speed in x and z axis required to wall run </summary>
    [SerializeField] private float wallSpeedThreshold;
    ///<summary>Speed when wall running</summary>
    [SerializeField] private float wallRunningSpeed;

    ///<summary>The Time it takes for the players y velocity to reach zero Must be between 0 and 1</summary>
    [SerializeField] private float timeToReachZero;

    ///<summary>The Length of the Grapple</summary>
    [SerializeField] private float grappleLength;
    ///<summary>The Prefab for the grapple hook</summary>
    [SerializeField] private GameObject grappleHook;


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
    /// <summary> CoyoteTimer time since leaving a ledge</summary>
    private float coyoteTimer;
    /// <summary> user wants to sprint</summary>
    private bool wishSprint;
    ///<summary>True if crouchPressed</summary>
    private bool isCrouchPressed;
    /// <summary>is true if has currently crouched</summary>
    private bool hasCrouched;
    /// <summary>current friction value</summary>
    private float curFriction;
    /// <summaryo>Is currently sliding has has slid</summary>
    private bool hasSlide;
    /// <summary>Acceleration when on the ground</summary>
    private float groundAcceleration;
    /// <summary>Checks whether the player is boosting</summary>
    private bool isBoosting;
    /// <summary>Checks whether the boost has been used</summary>
    private bool boostSpent;
    /// <summary>Number of Dashes used</summary>
    private int dashesUsed;
    /// <summary>time to gain a dash back</summary>
    private float dashRechargeTimer;
    /// <summary>time till can next use a dash</summary>
    private float dashCooldownTimer;
    /// <summary>if Dash on cooldown</summary>
    private bool hasDashed;

    /// <summary>Current wall running ray, if not on wall is -1  </summary>
    private int rayNumber;
    /// <summary> if currently wall running</summary>
    private bool wallRunning;
    /// <summary>Current normal of the wall we're running on</summary>
    private Vector3 curNormal;

    /// <summary>Checks whetehr the player is Gliding<summary>
    private bool isGliding;
    /// <summary>The current gravity force that affects the player</summary>
    private float playerGravity;
    ///<summary>Boolean to see wether the player is currently Grappling</summary>
    private bool isGrappling;

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
        maxSpeed = walkSpeed;
        curFriction = groundFriction;
        groundAcceleration = baseGroundAcceleration;

        rayNumber = -1;

        playerGravity = gravity;

    }

    public void Update()
    {

        if (wallRunning) 
        { 
            Wallrun();
        }
        else
        {
            ApplyGravity(playerGravity);
            Move();
        }

        if (cc.isGrounded)
        {
            playerGravity = gravity;
            isGliding = false;
        }
        
        

        ApplyJumps();
        LookandRotate();
        WallRotate();
        StaminaRecovery();
        Boost();
        DashCooldowns();
        cc.Move(velocity * Time.deltaTime); // this has to go after all the move logic
        //Debug.Log(new Vector2(velocity.x,velocity.z).magnitude);
        

    }

    public void FixedUpdate()
    {
        if(!wallRunning)
        {
            CheckForWall();
        }
        else
        {
            CheckStillWall();
        }
    }
    #endregion


    #region Movement Methods

    

    /// <summary>
    /// Deals movement in the x and z axis <br/>
    /// Checks wether the Player is trying to sprint <br/>
    /// If so the sprintMaxSpeed is used in place of maxSpeed <br/>
    /// </summary>
    private void Move()
    {
        Vector3 wishdir = new Vector3(wasdInput.x,0,wasdInput.y);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        float wishSpeed;


        if (wishSprint && stamina > 0)
        {
            wishSpeed = wishdir.magnitude * (maxSpeed + sprintMaxSpeed);
            stamina -= (staminaDrain * Time.deltaTime);
        }
        else
        {
            wishSpeed = wishdir.magnitude * maxSpeed;
        }

        ///ground movement
        if (cc.isGrounded)
        {
            ApplyCrouchAndSlide();
            ApplyFriction();
            //Accelerate(wishSpeed, wishdir, groundAcceleration);
            Accelerate(wishSpeed, wishdir, groundAcceleration);

        }
        //air movement
        else
        {
            if (wishSpeed > wishSpeed/5) wishSpeed = 2;
            Accelerate(wishSpeed, wishdir, airAcceleration);
        }
    }

    /// <summary>
    /// Applys friciton for being on the ground
    /// </summary>
    private void ApplyFriction()
    {

        float curSpeed = new Vector2(velocity.x, velocity.z).magnitude;

        if (curSpeed <= 0) return;
        float newSpeed = curSpeed - Time.deltaTime * curSpeed * curFriction;

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
        {
            accelSpeed = addSpeed;
        }

        velocity.x += accelSpeed * wishdir.x;
        velocity.z += accelSpeed * wishdir.z;
    }

    /// <summary>
    /// Applys gravity if not grounded
    /// </summary>
    private void ApplyGravity(float gravity)
    {
        if (cc.isGrounded) 
        {
            return;
        }
        velocity.y += Time.deltaTime * -gravity;
    }

    /// <summary>
    /// Jump logic <br/>
    /// <br/>
    /// Increments coyote timer(this could be done anywhere its used every frame but as its related to jumps ive put it here) <br/>
    /// Number of jumps to be used is reset when on the ground or on the wall<br/>
    /// if your wall running and want tojump gives jump both up and away from the wall
    /// If off the ground and didn't use a jump to get there lose a jump, ie falling off ledges uses a jump.
    /// If a jump input has been entered and there is jumps available gives speed in the y direction<br/>
    /// If wanting to jump while not on ground increments jump timer<br/>
    /// If the jump timer is greater than the input window stop wanting to jump<br/>
    /// </summary>
    private void ApplyJumps()
    {
        if (coyoteTimer < coyoteTime) coyoteTimer += Time.deltaTime;

        if (cc.isGrounded || wallRunning) jumpsUsed = 0;

        if ((!cc.isGrounded && !wallRunning) && jumpsUsed < 1 && coyoteTime<coyoteTimer) jumpsUsed = 1;

        if(wallRunning && wishJump && jumpsUsed < noJumps)
        {
            Vector3 jumpVel= (new Vector3(0, 1, 0) + curNormal) * jumpSpeed;
            velocity.y = jumpVel.y;
            velocity.x += jumpVel.x;
            velocity.z += jumpVel.z;

            wishJump = false;
            jumpsUsed++;
        }
        else if (wishJump && jumpsUsed < noJumps)
        {
            velocity.y = jumpSpeed;
            wishJump = false;
            jumpsUsed++;
        }

        if (wishJump && jumpTimer < jumpWindow) jumpTimer += Time.deltaTime;

        if (wishJump && jumpTimer > jumpWindow) wishJump = false;
    }

    /// <summary>
    /// Crouch and sliding logic <br/>
    /// checks if crouch button is pressed and has enough velocity if so slide
    /// checks if crouch is pressed and not enough velocity then just crouch
    /// checks if crouch is not pressed but was just sliding or crouched then go back to walking
    /// </summary>
    private void ApplyCrouchAndSlide()
    {
        if(isCrouchPressed && velocity.magnitude > slideThreshold)
        {
            curFriction = slideFriction;
            maxSpeed = 1.5f;
            hasSlide = true;
            Debug.Log("Sliding");
            
        }
        if(isCrouchPressed && velocity.magnitude < slideThreshold && !hasCrouched)
        {
            maxSpeed = crouchSpeed;
            hasCrouched = true;
            curFriction = groundFriction;
            Debug.Log(curFriction);
        }
        if (!isCrouchPressed && (hasCrouched || hasSlide))
        {
            maxSpeed = walkSpeed;
            hasCrouched = false;
            hasSlide = false;
            curFriction = groundFriction;    
        }
    }

    /// <summary>
    /// Stamina Recorvery Logic <br/>
    /// <br/>
    /// If the Player has maxStamina or is trying to Sprint then the function returns <br/>
    /// Otherwise the Player will gain back the stamina dependant on the value of staminaRecover. <br/>
    /// If the Stamina exceeds the maxStamina value. Stamina becomes the max value<br/>
    /// </summary>
    private void StaminaRecovery()
    {
        //checks to see if sprinting then if it can add any more stamina
        if (wishSprint)
        {
            return;
        }
        if (stamina == maxStamina)
        {
            return;
        }

        stamina += (staminaRecover * Time.deltaTime);
        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
    }

    /// <summary>
    /// Increases Acceleration and Speed Cap
    /// </summary>
    private void Boost()
    {
        if (isBoosting && !boostSpent)
        {
            groundAcceleration = boostGroundAcceleration;
            maxSpeed = boostSpeedCap;
            boostTime -= Time.deltaTime;
            if (boostTime <= 0)
            {
                isBoosting = false;
                boostSpent = true;
                groundAcceleration = baseGroundAcceleration;
                maxSpeed = walkSpeed;
            }
        }
        else if (boostSpent)
        {
            boostTime += boostRecoverSpeed * Time.deltaTime;
            if (boostTime >= maxBoostTime)
            {
                boostTime = maxBoostTime;
                boostSpent = false;
            }
        }
    }
    
    /// <summary>
    /// Called when pressing ability key while dash is the ability
    /// </br>
    /// If dashes are available gives velocity in direction you are loooking
    /// </summary>
    private void Dash()
    {
        if (dashesUsed < noDashes && !hasDashed)
        {
            Vector3 wishDashDir = cam.transform.forward;
            Vector3 wishDashVel = wishDashDir * dashSpeed;
            velocity += wishDashVel;
            dashesUsed++;
            hasDashed = true;
        }
    }

    /// <summary>
    /// Called Every Frame</br>
    /// Ticks up the dash cool down timer between dashes, then once cooldown timer met, allows dash again</br>
    /// Ticks up dash recharge timer after dashes have been used, once timer met, gives one dash back
    /// </summary>
    private void DashCooldowns()
    {
        // dash cooldown timer( time between consecutive dashes)
        if(hasDashed&& dashCooldownTimer < dashCooldownTime)
        {
            dashCooldownTimer += Time.deltaTime;
        }
        else if (hasDashed)
        {
            dashCooldownTimer = 0;
            hasDashed = false;
        }

        // dash recharge timer
        if (dashesUsed > 0 && dashRechargeTimer < dashRechargeTime)
        {
            dashRechargeTimer += Time.deltaTime;
        }
        else if(dashesUsed > 0)
        {
            dashRechargeTimer = 0;
            dashesUsed--;
        }
    }

    /// <summary>
    /// checks if there is a wall nearby then sets the rayNumber to the current ray theat is on the wall and sets wallrunning to true
    /// </br>
    /// should be called every fixed update
    /// </summary>
    private void CheckForWall()
    {
        LayerMask mask = LayerMask.GetMask("Env");
        // gets shortest ray closest to the wall
        float shortesthitdist = wallDetectionDistance + 1;
        RaycastHit ray = new RaycastHit();
        int rayNo = -1;
        for (int i = 0; i < 5; i++)
        {
            RaycastHit hitinfo;
            Ray tempRay = new Ray(transform.position + new Vector3(0, 1, 0), Quaternion.AngleAxis(45 * i, transform.up) * (-transform.right));
            Debug.DrawRay(transform.position+new Vector3(0,1,0), Quaternion.AngleAxis(45*i,transform.up)*(-transform.right));

            if(Physics.Raycast(tempRay, out hitinfo, wallDetectionDistance, mask) && hitinfo.distance<shortesthitdist)
            {
                shortesthitdist = hitinfo.distance;
                ray = hitinfo;
                rayNo = i;
                //Debug.Log(rayNo);
                //Debug.Log(Vector3.Dot(ray.normal, Vector3.up));
                //Debug.Log(new Vector2(velocity.x, velocity.z).magnitude);
            }
        }

        
        // if correct ray and is perpendicular to player and above the correct speed and we're in the air eneter wall running mode
        if((rayNo is 0 or 1 or 3 or 4)&& Vector3.Dot(ray.normal,Vector3.up) == 0 && new Vector2(velocity.x,velocity.z).magnitude >= wallSpeedThreshold && !cc.isGrounded)
        {
            rayNumber = rayNo;
            wallRunning = true;
            curNormal = ray.normal;
            velocity.y = Mathf.Clamp(velocity.y,-100,1);

        }
        else
        {
            rayNumber = -1;
        }
    }
    /// <summary>
    /// Checks if still on the wall if on the wall 
    /// </br>
    /// called every fixed update 
    /// </summary>
    private void CheckStillWall()
    {
        LayerMask mask = LayerMask.GetMask("Env");
        RaycastHit hit;
        if (rayNumber == 1)
        {
            rayNumber = 0;
        }
        else if (rayNumber == 3) 
        { 
            rayNumber = 4;
        }
        Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), Quaternion.AngleAxis(45 * rayNumber, transform.up) * (-transform.right));
        if (Physics.Raycast(ray, out hit, wallDetectionDistance, mask) && new Vector2(velocity.x, velocity.z).magnitude >= wallSpeedThreshold && !cc.isGrounded)
        {
            wallRunning = true;
            curNormal = hit.normal;
            //Debug.DrawRay(transform.position, ray.direction);
        }
        else 
        { 
            wallRunning = false;
            curFriction = groundFriction;
            cam.transform.rotation = Quaternion.identity ;
            return;
        }
    }
    /// <summary>
    /// function that allows the player to wall run called every frame your wall running
    /// </summary>
    private void Wallrun()
    {
        //gets wall direction to run along wall
        Vector3 wallRunDirection = Vector3.Cross(curNormal, Vector3.up);
        wallRunDirection = wallRunDirection.normalized;
        if (Vector3.Dot(wallRunDirection, transform.forward) < 0) { wallRunDirection *= -1; }

        //moves player along the wall
        wallRunDirection *= wasdInput.y;
        Accelerate(wallRunningSpeed, wallRunDirection, groundAcceleration);
        ApplyFriction();

        //applys some gravity while on the wall 
        velocity.y += Time.deltaTime * -3;
        Debug.DrawRay(transform.position, wallRunDirection * 100);
    }


    /// <summary>
    /// The function will handle gliding
    /// </summary>
    private void Glide()
    {
        if (isGliding)
        {
            playerGravity = gravity;
            isGliding = false;
        }
        else
        {
            playerGravity = glideGravity;
            isGliding = true;
            velocity.y = 0;
            //velocity.y = Mathf.SmoothStep(velocity.y, 0, timeToReachZero);
        }
    }

    ///<summary>
    ///The Grapple Function <- make this nicer once closer to working
    ///</summary>
    private void GrappleShot()
    {
        if (isGrappling)
        {
            //Debug.Log("How the fuck are you seeing this");
            BroadcastMessage("KillHook");
            isGrappling = false;
            return;
        }
        LayerMask mask = LayerMask.GetMask("Env");

        RaycastHit HitInfo;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, grappleLength))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.forward*100, Color.yellow, 10f);
            GameObject p = Instantiate(grappleHook, HitInfo.point, Quaternion.identity, this.transform);
            isGrappling = true;
        }
    }

    #endregion

    #region Camera methods

    /// <summary>
    /// rotates the camera when it conncts with the wall and rotates when it leaves
    /// </summary>
    private void WallRotate()
    {
        if(wallRunning && cam.transform.localEulerAngles.z ==0 && rayNumber is 1 or 0)
        {
            cam.transform.Rotate(0, 0, -20);
            Debug.Log("rotating");
        }
        else if(wallRunning && cam.transform.localEulerAngles.z == 0)
        {
            cam.transform.Rotate(0, 0, 20);
            Debug.Log("rotating");
        }
        else if(!wallRunning && cam.transform.localEulerAngles.z != 0)
        {
            float rot = 0 - cam.transform.localEulerAngles.z;
            cam.transform.Rotate(0, 0, rot);
        }
    }

    /// <summary>
    /// Called every frame to update where the camera looks and to rotate the character
    /// </summary>
    private void LookandRotate()
    {
        transform.Rotate(new Vector3(0, lookInput.x, 0));

        camPitch = Mathf.Clamp(camPitch + lookInput.y, -MaxPitch, MaxPitch);

        cam.transform.localEulerAngles = new Vector3(-camPitch, 0, 0);

    }
    #endregion 

    #region Input

    public void GetMoveInput(InputAction.CallbackContext ctx)
    {
        wasdInput = ctx.ReadValue<Vector2>();
        if (wasdInput.y <= 0)
        {
            wishSprint = false;
        }
    }

    public void GetLookInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        lookInput = input * lookSensitivity;
    }

    public void getJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            wishJump = true;
        }
    }

    public void getSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.started && wasdInput.y > 0)
        {
            wishSprint = true;
        }
        if (ctx.canceled)
        {
            wishSprint = false;
        }
    }
    public void getCrouch(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isCrouchPressed = true;
            cam.transform.localPosition = new Vector3(0, 0.5f, 0);
            cc.height = 1;
            cc.center = new Vector3(0,0.5f,0);
        }
        else if (ctx.canceled)
        {
            isCrouchPressed = false;
            cam.transform.localPosition = new Vector3(0, 1f, 0);
            cc.height = 2;
            cc.center = new Vector3(0, 1f, 0);
        }
    }

    /// <summary>
    /// Called when the player tries to use the current ability. Checks which ability is currently seected using Enum.
    /// Branched off down whichever path is recuired for each ability
    /// </summary>
    public void getAbility(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            switch (currentAbility)
            {
                case Ability.Dash:
                    Debug.Log("Dashing");
                    Dash();
                    break;
                case Ability.Grapple:
                    Debug.Log("Grappling");
                    GrappleShot();
                    break;
                case Ability.Boost:
                    Debug.Log("Boosting");
                    if (!boostSpent)
                    {
                        isBoosting = true;
                    }
                    break;
                case Ability.Glide:
                    Debug.Log("Gliding");
                    Glide();
                    break;
                default:
                    Debug.Log("No Ability Selected");
                    break;
            }
        }
    }

    public void getInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Interact");
        }
    }

    #endregion
}

/// <summary>
/// The Enum for the ability that the player currently has selected
/// </summary>
enum Ability
{
    None,
    Dash,
    Grapple,
    Boost,
    Glide
}
