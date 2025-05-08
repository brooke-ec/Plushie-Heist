using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[SelectionBase]
public class PlayerController : MonoBehaviour
{
    #region Serialized fields
    [Header("Base Player Values")]
    /// <summary>  player Speed</summary>
    [SerializeField] private float walkSpeed;
    ///<summary>Crouch speed</summary>
    [SerializeField] private float crouchSpeed;
    ///<summary>Acceleration when in the air</summary>
    [SerializeField] private float airAcceleration;
    /// <summary> Friction of ground</summary>
    [SerializeField] private float groundFriction;
    ///<summary>Gravity Value</summary>
    [SerializeField] private float gravity;
    ///<summary>The base Ground Acceleration</summary>
    [SerializeField] private float baseGroundAcceleration;
    /// <summary>The Ability that the Player currently has equiped</summary>
    [SerializeField] private Ability currentAbility;



    [Header("Sprinting Values")]
    /// <summary> Maximum player Sprint Speed rework to be an increase on top of the maxspeed</summary>
    [SerializeField] private float sprintMaxSpeed;
    /// <summary> The players Max Stamina</summary>
    [SerializeField] private float maxStamina;
    /// <summary> The speed at which the players stamina is drained</summary>
    [SerializeField] private float staminaDrain;
    /// <summary> The speed at which the player stamina recovers</summary>
    [SerializeField] private float staminaRecover;


    [Header("Camera Settings")]
    /// <summary> Sensitivity of the mouse </summary>
    [SerializeField] private float lookSensitivity;
    /// <summary>Maximum pitch of the camera</summary>
    [SerializeField] private float MaxPitch;


    [Header("Glide Values")]
    ///<summary>Gliding Gravity</summary>
    [SerializeField] private float glideGravity;

    [Header("Jump Values")]
    ///<summary>Velocity given for jumping</summary>
    [SerializeField] private float jumpSpeed;
    ///<summary>Number of Jumps</summary>
    [SerializeField] private int noJumps;
    ///<summary>time allowed to press jump before landing for the input to still register in seconds</summary>
    [SerializeField] private float jumpWindow;
    ///<summary>Coyote Time, the time after leaving a ledge you can still jump</summary>
    [SerializeField] private float coyoteTime;
    ///<summary>Speed threshold for sliding MUST BE BIGGER THAN 5.1</summary>

    [Header("Slide Values")]
    [SerializeField] private float slideThreshold;
    ///<summary>Friction when sliding</summary>
    [SerializeField] private float slideFriction;

    [Header("Boost Values")]
    ///<summary>The new acceleration speed when the palyer boosts</summary>
    [SerializeField] private float boostSpeedCap;

    ///<summary>The boosting Ground Acceleration</summary>
    [SerializeField] private float boostGroundAcceleration;
    ///<summary>The maximum boost time</summary>
    [SerializeField] private float maxBoostTime;
    ///<summary>The speed at which the Player regains the boost at</summary>
    [SerializeField] private float boostRecoverSpeed;

    [Header("Dash Values")]
    /// <summary>the speed given when dashing</summary>
    [SerializeField] private float dashSpeed;
    /// <summary>The number of Dashes available</summary>
    [SerializeField] private int noDashes;
    /// <summary> Amount of time to regain a dash charge</summary>
    [SerializeField] private float dashRechargeTime;
    /// <summary> Amount of time between dashes</summary>
    [SerializeField] private float dashCooldownTime;

    [Header("WaLL Running Values")]
    /// <summary>Distance for detecting walls</summary>
    [SerializeField] private float wallDetectionDistance;
    /// <summary> minimum speed in x and z axis required to wall run </summary>
    [SerializeField] private float wallSpeedThreshold;
    ///<summary>Speed when wall running</summary>
    [SerializeField] private float wallRunningSpeed;

    ///<summary>The Time it takes for the players y velocity to reach zero Must be between 0 and 1</summary>
    //[SerializeField] private float timeToReachZero;

    [Header("Grapple Values")]
    ///<summary>The Length of the Grapple</summary>
    [SerializeField] private float grappleLength;
    ///<summary>The Prefab for the grapple hook</summary>
    [SerializeField] private GameObject grappleHook;
    ///<summary>The Speed of the Grapple Hook</summary>
    [SerializeField] private float grappleSpeed;
    ///<summary>The Acceleration of the Grapple Hook</summary>
    [SerializeField] private float grappleAccel;
    ///<summary>The distance at which the grapple will release</summary>
    [SerializeField] private float grappleCancelLength;
    ///<summary>The Cooldown for the grapple Ability</summary> 
    [SerializeField] private float grappleCooldown;
    ///<summary>The rate at which grapple recovers from cooldown</summary>
    [SerializeField] private float grappleCooldownSpeed;
    /// <summary>The Strength that the player throws the beanbags at</summary>
    [SerializeField] private float _throwStrength;
    /// <summary>The beanBag that is attached to the player</summary>
    [SerializeField] private GameObject _beanBag;
    /// <summary>The BeanBag Prefab</summary>
    [SerializeField] private GameObject _beanBagPrefab;
    #endregion

    #region private fields
    /// <summary>Current Player Velocity (NOT the character controller velocity we dont like that one)</summary>
    private Vector3 velocity;
    /// <summary> Character controller asset</summary>
    private CharacterController cc;
    /// <summary> Maximum player Speed</summary>
    private float maxSpeed;
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
    /// <summary> The players Stamina</summary>
    private float stamina;
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
    ///<summary>The time the player will be boosting for</summary>
    private float boostTime;
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
    ///<summary>The Hook when it exists</summary>
    private GameObject Hook;
    ///<summary></summary>
    private float grappleCooldownMax;
    /// <summary> The current offset of the camera </summary>
    private Vector3 cameraOffset;

    /// <summary>rotation to adjust camera to away from wall should only be 5 or -5 </summary>
    private Vector3 rotAdjustVal;

    /// <summary>ther animator component </summary>
    private Animator animator;

    /// <summary>the array of the guards that are chasing you </summary>
    public List<GaurdAI> guardsChasing;
    
    /// <summary>number of times been arrested</summary>
    private int arrestCount =0;

    /// <summary>The inital position of the player</summary>
    private Vector3 initalPos;

    private bool nightEnded;

    private bool inventoryOpen;

    /// <summary>Has the player entered a bouncePad</summary>
    private bool _isBouncing;

    /// <summary>The direction that the bounce pad will launch the player in</summary>
    private Vector3 _BouncePadWishVel;

    /// <summary>Wether the player is currently holding a beanbag</summary>
    private bool _holdingBeanBag;
    #endregion

    #region Public Fields
    [HideInInspector]public bool arrested = false;
    [HideInInspector] public Transform seat = null;
    /// <summary>bool if second chance activated</summary>
    public bool secondChance;

    public bool wallRunEnabled;
    #endregion

    #region core methods
    public void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        animator = GetComponentInChildren<Animator>();
        guardsChasing = new List<GaurdAI>();
    }

    private int frameNo;
    public void Start()
    {
        wishJump = false;
        maxSpeed = walkSpeed;
        curFriction = groundFriction;
        groundAcceleration = baseGroundAcceleration;

        grappleCooldownMax = grappleCooldown;

        rayNumber = -1;

        playerGravity = gravity;

        initalPos = transform.position;
        _beanBag.SetActive(false);
    }

    public void Update()
    {
        frameNo++;

        //Wall movement or regular movement 
        if (wallRunning && !isGrappling && wallRunEnabled)
        {
            Wallrun();
            animator.SetInteger("Falling", 0);
        }
        else
        {
            Move();
        }

        //gravity logic
        if (!cc.isGrounded && !wallRunning)
        {
            ApplyGravity(playerGravity);
        }
        else if (cc.isGrounded && velocity.y != 0)
        {
            velocity.y = 0;
            animator.SetInteger("Falling", 0);
        }
        if (cc.isGrounded || wallRunning)
        {
            playerGravity = gravity;
            isGliding = false;
        }

        //grapple logic
        if (isGrappling)
        {
            ApplyGrappleForce();
        }

        //Methods to be called every frame
        ApplyJumps();
        LookandRotate();
        //WallRotate();
        StaminaRecovery();
        GrappleCooldown();
        Boost();
        DashCooldowns();

        if(_isBouncing)
        {
            _isBouncing = false;
            velocity += _BouncePadWishVel;
        }

        //actuall move the player
        cc.Move(velocity * Time.deltaTime); // this has to go after all the move logic
        //Debug.Log(velocity.magnitude);

        //animates the player
        Animate();

        UpdateAbilitiesCooldowns();

        if (seat != null)
        {
            velocity = Vector3.zero;
            SetCameraOffset(Vector3.up * -0.4f);
            transform.position = seat.position;
            animator.SetTrigger("Sit");
        }
        if (arrested & !nightEnded)
        {
            Arrest();
        }
    }

    /// <summary> Update UI of ability with cooldown </summary>
    private void UpdateAbilitiesCooldowns()
    {
        //In future, if we have a list of active abilities, then pass those instead of all

        for (int i = 0; i < 4; i++)
        {
            float cooldown = 0f, cooldownMax = 0f;
            switch ((Ability)i)
            {
                case Ability.Dash:
                    cooldown = dashRechargeTimer;
                    cooldownMax = dashRechargeTime;
                    break;
                case Ability.Grapple:
                    cooldown = grappleCooldown;
                    cooldownMax = grappleCooldownMax;
                    break;
                case Ability.Glide:
                    cooldown = 0;
                    cooldownMax = 0;
                    //None?
                    break;
                case Ability.Boost:
                    cooldown = boostTime;
                    cooldownMax = maxBoostTime;
                    //None?
                    break;
                default:
                    break;
            }

            if (MovementUIManager.instance != null)
            {
                MovementUIManager.instance.UpdateAbilityCooldown((Ability)i, cooldown, cooldownMax);
                MovementUIManager.instance.UpdateStaminaBar(stamina, maxStamina);
            }
        }
    }

    /// <summary>
    /// Modifies the needed values for skills upgrades. Only "dash", "jump" and "boost"
    /// </summary>
    /// <param name="ability">The ability to modify the values of</param>
    /// <param name="modifier">the modifier of the values</param>
    /// <exception cref="NotImplementedException"></exception>
    public void ModifyAbilityValue(string ability, float modifier)
    {
        switch(ability)
        {
            case "dash":
                dashSpeed *= modifier;
                break;
            case "jump":
                noJumps += (int)modifier;
                break;
            case "boost":
                boostSpeedCap *= modifier;
                boostGroundAcceleration *= modifier;
                break;
            default:
                print("Error modifying ability value");
                break;
        }
    }

    public void FixedUpdate()
    {
        //Wall checking done here as is a physics method
        if (!wallRunning && wallRunEnabled)
        {
            CheckForWall();
        }
        else
        {
            CheckStillWall();
        }
    }

    public void PickupBean()
    {
        _beanBag.SetActive(true);
        _holdingBeanBag = true;
    }

    public void ThrowBean()
    {
        if(_holdingBeanBag)
        {
            _beanBag.SetActive(false);
            _holdingBeanBag = false;
            Quaternion camRot = cam.gameObject.transform.rotation;
            
            BeanBag ben = Instantiate(_beanBagPrefab, (this.transform.position + new Vector3(0,1,0)), camRot).GetComponent<BeanBag>();

            ben.Throw(_throwStrength + this.velocity.magnitude);
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
        Vector3 wishdir = new Vector3(wasdInput.x, 0, wasdInput.y);
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
            if (wishSpeed > wishSpeed / 5) wishSpeed = 2;
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

        float accelSpeed = accel * Time.deltaTime * wishspeed;
        if (accelSpeed > addSpeed)
        {
            accelSpeed = addSpeed;
        }

        velocity.x += accelSpeed * wishdir.x;
        velocity.z += accelSpeed * wishdir.z;
        if (isGrappling)
        {
            velocity.y += accelSpeed * wishdir.y;
        }
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
        if (!cc.isGrounded && !wallRunning && coyoteTimer < coyoteTime) coyoteTimer += Time.deltaTime;

        if (cc.isGrounded || wallRunning) jumpsUsed = 0;

        if ((!cc.isGrounded && !wallRunning) && jumpsUsed < 1 && coyoteTime < coyoteTimer)
        {
            jumpsUsed = 1;
            coyoteTimer = 0;
        }

        if (wallRunning && wishJump && jumpsUsed < noJumps)
        {
            Vector3 jumpVel = (new Vector3(0, 1, 0) + curNormal) * jumpSpeed;
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

        //Debug.Log(coyoteTimer);
    }

    /// <summary>
    /// Crouch and sliding logic <br/>
    /// checks if crouch button is pressed and has enough velocity if so slide
    /// checks if crouch is pressed and not enough velocity then just crouch
    /// checks if crouch is not pressed but was just sliding or crouched then go back to walking
    /// </summary>
    private void ApplyCrouchAndSlide()
    {
        if (isCrouchPressed && velocity.magnitude > slideThreshold)
        {
            curFriction = slideFriction;
            maxSpeed = 1.5f;
            hasSlide = true;
            //Debug.Log("Sliding");
            animator.SetBool("Slide", true);

        }
        if (isCrouchPressed && velocity.magnitude < slideThreshold && !hasCrouched)
        {
            maxSpeed = crouchSpeed;
            hasCrouched = true;
            curFriction = groundFriction;
            animator.SetBool("Slide", false);
            animator.SetBool("Crouch", true);

        }
        if (!isCrouchPressed && (hasCrouched || hasSlide))
        {
            maxSpeed = walkSpeed;
            hasCrouched = false;
            hasSlide = false;
            curFriction = groundFriction;
            animator.SetBool("Slide", false);
            animator.SetBool("Crouch", false);
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
        if (dashesUsed < noDashes && !hasDashed && !arrested)
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
        if (hasDashed && dashCooldownTimer < dashCooldownTime)
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
        else if (dashesUsed > 0)
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
            //Debug.DrawRay(transform.position+new Vector3(0,1,0), Quaternion.AngleAxis(45*i,transform.up)*(-transform.right));

            if (Physics.Raycast(tempRay, out hitinfo, wallDetectionDistance, mask) && hitinfo.distance < shortesthitdist)
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
        if ((rayNo is 0 or 1 or 3 or 4) && Vector3.Dot(ray.normal, Vector3.up) == 0 && new Vector2(velocity.x, velocity.z).magnitude >= wallSpeedThreshold && !cc.isGrounded)
        {
            rayNumber = rayNo;
            wallRunning = true;
            curNormal = ray.normal;
            velocity.y = Mathf.Clamp(velocity.y, -100, 1);
            curFriction = groundFriction;
            Uncrouch();

            if (rayNo is 1 or 0)
            {
                cam.transform.DOLocalRotate(new(0, 0, -20), 0.5f,RotateMode.LocalAxisAdd);
            }
            else if(rayNo is 3 or 4)
            {
                cam.transform.DOLocalRotate(new(0, 0, 20), 0.5f,RotateMode.LocalAxisAdd);
            }
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
            maxSpeed = wallRunningSpeed;
            //Debug.DrawRay(transform.position, ray.direction);
        }
        else
        {
            wallRunning = false;
            curFriction = groundFriction;
            float rotValue = 0 - cam.transform.localEulerAngles.z;
            rotValue = rotValue < -180 ? rotValue+360:rotValue;
            cam.transform.DOLocalRotate(new(0, 0, rotValue), 0.5f, RotateMode.LocalAxisAdd);
            maxSpeed = walkSpeed;
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
        Accelerate(maxSpeed, wallRunDirection, groundAcceleration);
        ApplyFriction();
        //Debug.Log(Quaternion.LookRotation(Quaternion.Euler(rotAdjustVal) * wallRunDirection).eulerAngles.y);
        //applys some gravity while on the wall 
        velocity.y += Time.deltaTime * -3;
        //Debug.DrawRay(transform.position, wallRunDirection * 100);
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
            Hook.SendMessage("KillHook");
            Hook = null;
            isGrappling = false;
            return;
        }
        if (grappleCooldown < grappleCooldownMax)
        {
            return;
        }
        LayerMask mask = LayerMask.GetMask("Env");

        RaycastHit HitInfo;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, grappleLength))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.forward*100, Color.yellow, 10f);
            Hook = Instantiate(grappleHook, HitInfo.point, Quaternion.identity);            
            isGrappling = true;
        }
    }

    /// <summary>
    /// Method for dealing with Grapple physics <br/>
    /// Calculates the direction and speed to apply velocity in and accellerates towards it
    /// </summary>
    private void ApplyGrappleForce()
    {
        Vector3 grappleForce = Hook.transform.position - this.transform.position;
        if (grappleForce.magnitude <= grappleCancelLength)
        {
            GrappleShot();
        }
        grappleForce.Normalize();
        float wishGrappleSpeed = grappleForce.magnitude * grappleSpeed;
        Accelerate(wishGrappleSpeed, grappleForce, grappleAccel);
    }

    /// <summary>
    /// Method for Grapples cooldown
    /// </summary>
    private void GrappleCooldown()
    {
        if (isGrappling)
        {
            grappleCooldown = 0;
            return;
        }
        if (grappleCooldown >= grappleCooldownMax)
        {
            grappleCooldown = grappleCooldownMax;
            return;
        }
        grappleCooldown += Time.deltaTime * grappleCooldownSpeed;
    }

    #endregion

    #region Camera methods

    /// <summary>
    /// rotates the camera when it conncts with the wall and rotates when it leaves; use tweening engine to animate it properly
    /// </summary>
    /*private void WallRotate()
    {
        if (!isGrappling && wallRunning && cam.transform.localEulerAngles.z == 0 && rayNumber is 1 or 0)
        {
            cam.transform.Rotate(0, 0, -20);
            rotAdjustVal = new Vector3(0, 5, 0);
            //Debug.Log("rotating");
        }
        else if (!isGrappling && wallRunning && cam.transform.localEulerAngles.z == 0)
        {
            cam.transform.Rotate(0, 0, 20);
            rotAdjustVal = new Vector3(0, -5, 0);
            //Debug.Log("rotating");
        }
            else if (!wallRunning && cam.transform.localEulerAngles.z != 0)
        {
            float rot = 0 - cam.transform.localEulerAngles.z;
            cam.transform.Rotate(0, 0, rot);
        }

    }*/

    /// <summary>
    /// Called every frame to update where the camera looks and to rotate the character
    /// </summary>
    private void LookandRotate()
    {
        transform.Rotate(new Vector3(0, lookInput.x, 0));

        camPitch = Mathf.Clamp(camPitch + lookInput.y, -MaxPitch, MaxPitch);

        cam.transform.localEulerAngles = new Vector3(-camPitch, 0, cam.transform.localEulerAngles.z);

    }

    /// <summary>
    /// moves camera downwards, sets crouch to true and makes hitbox smaller
    /// </summary>
    private void Crouch()
    {
        isCrouchPressed = true;
        SetCameraOffset(Vector3.up * -0.25f);
        cc.height = 1.5f;
        cc.center = new Vector3(0, 0.75f, 0);
    }

    /// <summary>
    /// moves camera up, sets crouch to false and makes hitbox taller
    /// </summary>
    private void Uncrouch()
    {
        isCrouchPressed = false;
        SetCameraOffset(Vector3.zero);
        cc.height = 2;
        cc.center = new Vector3(0, 1f, 0);
    }

    private void SetCameraOffset(Vector3 vector)
    {
        cam.transform.localPosition += vector - cameraOffset;
        cameraOffset = vector;
    }
    #endregion

    #region animations

    /// <summary>
    /// changes the values for the animator state machine based on what the player is doing
    /// </summary>
    private void Animate()
    {
        if (wishSprint && velocity.magnitude > 1 && stamina > 0)
        {
            //Debug.Log("sprintin");
            animator.SetInteger("Speed", 2);
        }
        else if (velocity.magnitude > 1)
        {
            //Debug.Log("walkin");
            animator.SetInteger("Speed", 1);
        }
        else
        {
            animator.SetInteger("Speed", 0);
        }

        if (velocity.y < -1 && !cc.isGrounded && !wallRunning)
        {
            animator.SetInteger("Falling", 1);
        }
    }
    #endregion

    #region gaurdInteraction
    private void Arrest()
    {
        if(secondChance && arrestCount < 1)
        {
            Debug.Log("arrested");
            arrestCount += 1;
            transform.position = initalPos;
            arrested = false;

            while (guardsChasing.Count > 0)
            {
                guardsChasing[0].loseIntrest();
            }
            
        }
        else 
        {
            nightEnded = true;
            ArrestMovement();
            NightManager.instance.OnEndNight(false);
        }
    }
    private void ArrestMovement()
    {
        wasdInput = Vector2.zero;
        wallRunning = false;
        isGrappling = false;
        isGliding = false;
        isBoosting = false;
    }

    private void applySlow(float slowAmt)
    {
        Vector3 direction = velocity.normalized;
        velocity -= direction * slowAmt;
    }

    public void addGuard(GaurdAI guard)
    {
        if (!guardsChasing.Contains(guard))
        {
            Debug.Log("added guard"+frameNo);
            guardsChasing.Add(guard);
            if (AudioManager.instance.currentMusicPlaying.musicName != AudioManager.MusicEnum.guardChasingMusic)
            {
                AudioManager.instance.PlayMusic(AudioManager.MusicEnum.guardChasingMusic);
            }
        }
    }

    public void removeGuard(GaurdAI guard)
    {
        if (guardsChasing.Contains(guard))
        {
            Debug.Log("removed guard"+frameNo);
            guardsChasing.Remove(guard);
            if (guardsChasing.Count == 0 && AudioManager.instance.currentMusicPlaying.musicName != AudioManager.MusicEnum.nightMusic)
            {
                AudioManager.instance.PlayMusic(AudioManager.MusicEnum.nightMusic);
            }
        }
    }

    #endregion

    #region Hazards
    /// <summary>
    /// Called when the player collides with a Hazard and handles the correct follow up actions
    /// </summary>
    /// <param name="name"></param>
    public void HitHazard(string name, GameObject theHazard)
    {
        //Debug.Log("In Function On Player");
        switch(name)
        {
            case "Bounce Pad":
                //Debug.Log("Calling Bounce Pad");
                BouncePad(theHazard);
                break;
            default:
                break;
        }
    }

    private void BouncePad(GameObject theHazard)
    {
        _isBouncing = true;
        
        Vector3 BouncePadDirection = theHazard.GetComponent<BouncePads>().getDirection();
        float BouncePadStrength = theHazard.GetComponent<BouncePads>().getStrength();

        _BouncePadWishVel = BouncePadDirection * BouncePadStrength;
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
            seat = null;
            wishJump = true;
            jumpTimer = 0;
            animator.SetTrigger("Jump");
            SetCameraOffset(Vector3.zero);
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
        if ((ctx.performed) && !wallRunning)
        {
            Crouch();
        }
        else if (ctx.canceled)
        {
            Uncrouch();
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

    public void toggleInventory(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (SharedUIManager.instance.isMenuOpen) SharedUIManager.instance.CloseMenu();
        else SharedUIManager.instance.OpenMenu(InventoryController.instance);
    }

    private void OnTriggerEnter(Collider other)
    {
        //If a projectile hits the player
        if(other.tag == "Proj")
        {
            Debug.Log("Proj Hit");
            applySlow(other.GetComponent<Projectilescript>().SlowAmount/100*velocity.magnitude);
            Destroy(other.gameObject);
        }
    }

    #endregion

    #region Ability Swapping

    private void SwapActiveAbility(Ability newAbility)
    {
        currentAbility = newAbility;
        MovementUIManager.instance.ChangeMovementUI(newAbility);
    }
    private void ChooseNone()
    {
        SwapActiveAbility(Ability.None);
    }

    private void ChooseDash()
    {
        SwapActiveAbility(Ability.Dash);
    }
    private void ChooseBoost()
    {
        SwapActiveAbility(Ability.Boost);
    }
    private void ChooseGrapple()
    {
        SwapActiveAbility(Ability.Grapple);
    }
    private void ChooseGlide()
    {
        SwapActiveAbility(Ability.Glide);
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
