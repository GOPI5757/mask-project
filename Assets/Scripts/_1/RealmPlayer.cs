using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class RealmPlayer : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField] float speed;
    float x;

    [SerializeField] float initialGravityScale;

    Rigidbody2D rb;

    Animator animator;

    [SerializeField] GameObject Canvas;

    [Header("Jump")]

    [SerializeField] float jumpForce;
    [SerializeField] LayerMask JumpLayer;
    [SerializeField] Transform JumpTransform;
    [SerializeField] float Jump_CircleRadius= 0.1f;
    [SerializeField] bool isGrounded, space_clicked;

    [Header("Movable")]

    [SerializeField] float checkLength;
    [SerializeField] bool isHoldingSomething = false, isForward;
    [SerializeField] float pushForce, pullForce;
    [SerializeField] float neutral_force_value;
    [SerializeField] float main_force_value;
    [SerializeField] float pp_elapsedTime, push_force_changeTime, pull_force_changeTime;
    [SerializeField] LayerMask mo_layer;
    [SerializeField] GameObject Ref_Obj;

    [Header("Trampoline")]

    [SerializeField] float TrampolineForce;
    [SerializeField] bool Is_Trampoline = false;

    [Header("Camera")]

    [SerializeField] bool is_cameraMoving = false;
    [SerializeField] Vector2 Cam_startPos, Cam_endPos;
    [SerializeField] float Cam_startSize, Cam_endSize;
    [SerializeField] float CameraElapsedTime, MoveTime;

    [Header("Swing")]

    [SerializeField] float swingForce;
    [SerializeField] bool is_insideSwingBounds;
    [SerializeField] bool canSwing, is_swinging;
    [SerializeField] Vector2 swing_player_startPos, swing_bar_pos;
    [SerializeField] float swing_elapsedTime, swing_lerpTime;
    [SerializeField] Transform SwingAttachPointTransform;
    [SerializeField] DistanceJoint2D DJ_2D;
    [SerializeField] float swingbarOffset;
    [SerializeField] float swing_distance;
    [SerializeField] float SwingJumpForce;
    [SerializeField] GameObject SwingBarObject;

    [Header("Mask Shift")]

    [SerializeField] GameObject[] Masks;
    [SerializeField] List<Vector2> initialMaskPositions = new List<Vector2>();
    [SerializeField] List<Vector2> targetMaskPositions = new List<Vector2>();
    [SerializeField] Transform[] InitialTranformPositions, TargetTransformPositions;
    [SerializeField] GameObject BlendInBG;
    [SerializeField] float MS_ElapsedTime, MS_LerpTime;
    [SerializeField] bool isClicking_MS_Tab, is_changing_masks, do_once;
    [SerializeField] float MS_initialScaleFixed, MS_TargetScaleFixed;
    float MS_initialScale, MS_TargetScale;
    Vector2[] mask_final_positions = {Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};
    [SerializeField] float initialAlphaFixed, FinalAlphaFixed;
    float initialAlpha, FinalAlpha;

    [Header("Dashing")]

    [SerializeField] float DashForce;
    [SerializeField] float DashForceDynamic;
    [SerializeField] float DashElapsedTime, DashResetTime;
    [SerializeField] bool isDashing;
    [SerializeField] bool ApplyDash;

    [Header("Mask Obtain")]

    [SerializeField] bool is_insideMaskBounds, is_obtaining_Mask, canRevealMaskUI, canRevealActiveMask;
    [SerializeField] bool maskRevealFinish;
    [SerializeField] float time_1, time_2;
    [SerializeField] float time_to_wait_before_mr, time_to_wait_after_mr;
    [SerializeField] Color MaskStartRevealColor, MaskEndRevealColor;
    [SerializeField] float MaskRevealElapsedTime, MaskRevealTime;
    [SerializeField] int maskToReveal;
    [SerializeField] GameObject CurrentMaskToReveal;

    [Header("Double Jump")]

    [SerializeField] int jumpIndex;
    [SerializeField] int maxJumps;
    [SerializeField] bool canDo_DoubleJump;

    [Header("Wall Jump")]

    [SerializeField] bool is_wall = false;
    [SerializeField] float wall_time, wall_friction_time;
    [SerializeField] bool canHoldWall = true;
    [SerializeField] Vector2 wall_jumpForce;

    [Header("Tutorial")]

    public bool is_showing_tutorial, canPressE;
    public GameObject TutorialObject;

    [Header("Death")]

    [SerializeField] bool is_dead;
    [SerializeField] float fadeIn_PlayerElapsedTime, fadeIn_Time;
    [SerializeField] Color PlayerStartColor, PlayerEndColor;
    [SerializeField] GameObject SafePositionObject;
    [SerializeField] float dead_time_1, dead_wait_time;
    [SerializeField] float dead_cam_elapsedTime, dead_cam_time;
    [SerializeField] Vector2 camStartPos, camEndPos;
    [SerializeField] float dead_time_2, spawn_wait_time;
    [SerializeField] float fadeOut_PlayerElapsedTime, fadeOut_Time;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialGravityScale = rb.gravityScale;
        SetMaskShiftInitials();
        MaskStartRevealColor = new Color(0, 0, 0, 1);
        MaskEndRevealColor = new Color(1, 1, 1, 1);

        animator = GetComponent<Animator>();
    }

    void SetMaskShiftInitials()
    {
        initialAlpha = initialAlphaFixed;
        FinalAlpha = FinalAlphaFixed;

        MS_initialScale = MS_initialScaleFixed;
        MS_TargetScale = MS_TargetScaleFixed;
        targetMaskPositions.Clear();
        initialMaskPositions.Clear();

        for(int i = 0; i < Masks.Length; i++)
        {
            initialMaskPositions.Add(InitialTranformPositions[i].position);
            targetMaskPositions.Add(TargetTransformPositions[i].position);
        }
    }

    private void Update()
    {
        if(!isClicking_MS_Tab && !isDashing && !is_obtaining_Mask)
        {
            HandleMovement();
            HandleInputs();
            HandleSwinging();
        }
        HandleMaskShifting();
        HandlePandP();
        HandleCamera();
        HandleDashing();
        HandleMaskObtaining();
        HandleDoubleJump();
        HandleWallJump();
        HandleCanHoldWall();
        HandleTutorialFadeOut();
        HandleDead();
    }

    private void LateUpdate()
    {
        if (is_dead) return;
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    void HandleDead()
    {
        if (!is_dead) return;
        float t = fadeIn_PlayerElapsedTime / fadeIn_Time;
        Color finalColor_1 = Color.Lerp(PlayerStartColor, PlayerEndColor, t);
        fadeIn_PlayerElapsedTime += Time.unscaledDeltaTime;
        GetComponent<SpriteRenderer>().color = finalColor_1;
        if(t <= 0)
        {
        }
        if(t >= 1)
        {
            dead_time_1 += Time.unscaledDeltaTime;
            transform.position = SafePositionObject.transform.GetChild(0).position;
            if(dead_time_1 > dead_wait_time)
            {
                float t_1 = dead_cam_elapsedTime / dead_cam_time;
                Vector3 finalPos = Vector3.Lerp(new Vector3(camStartPos.x, camStartPos.y, -10f), 
                    new Vector3(camEndPos.x, camEndPos.y, -10f), t_1);
                dead_cam_elapsedTime += Time.unscaledDeltaTime;

                Camera.main.transform.position = finalPos;
                if(t_1 >= 1)
                {
                    dead_time_2 += Time.unscaledDeltaTime;
                    if(dead_time_2 > spawn_wait_time)
                    {
                        float t_2 = fadeOut_PlayerElapsedTime/fadeOut_Time;
                        Color finalColor_2 = Color.Lerp(PlayerEndColor, PlayerStartColor, t_2);
                        fadeOut_PlayerElapsedTime += Time.unscaledDeltaTime;

                        GetComponent<SpriteRenderer>().color = finalColor_2;
                        if (t_2 >= 1)
                        {
                            is_dead = false;
                            fadeIn_PlayerElapsedTime = 0f;
                            dead_time_1 = 0f;
                            dead_cam_elapsedTime = 0f;
                            dead_time_2 = 0f;
                            fadeOut_PlayerElapsedTime = 0f;
                            rb.gravityScale = initialGravityScale;
                        }
                    }
                }
            }
        }
    }

    void HandleTutorialFadeOut()
    {
        if(canPressE)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                TutorialObject.GetComponent<TutorialBlock>().has_tutorial_given = true;
                canPressE = false;
                x = 0f;
            }
        }
    }

    void HandleWallJump()
    {
        if (!is_wall) return;
        wall_time += Time.deltaTime;
        if (wall_time >= wall_friction_time)
        {
            canHoldWall = false;
            rb.gravityScale = initialGravityScale;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int multiplier = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
                rb.AddForce(new Vector2(wall_jumpForce.x * multiplier, wall_jumpForce.y), ForceMode2D.Impulse);
                rb.gravityScale = initialGravityScale;
                space_clicked = false;
                wall_time = 0f;
            }
        }
    }

    void HandleCanHoldWall()
    {
        if (!canHoldWall)
        {
            if (isGrounded)
            {
                canHoldWall = true;
                wall_time = 0f;
                is_wall = false;
            }
        }
    }

    void HandleMaskObtaining()
    {
        if(is_insideMaskBounds && !is_obtaining_Mask)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                x = 0f;
                SetMaskShiftInitials();
                is_obtaining_Mask = true;
                if(CurrentMaskToReveal)
                {
                    CurrentMaskToReveal.GetComponent<Mask>().HasObtained(gameObject);
                }
            }
        }

        if(is_obtaining_Mask)
        {
            if(canRevealMaskUI)
            {
                MaskShiftLerp(true, true);
            }

            if(canRevealActiveMask)
            {
                float t = MaskRevealElapsedTime / MaskRevealTime;
                Color color_lerp = Color.Lerp(MaskStartRevealColor, MaskEndRevealColor, t);
                MaskRevealElapsedTime += Time.deltaTime;
                Masks[maskToReveal].GetComponent<Image>().color = color_lerp;

                if(t >= 1)
                {
                    maskRevealFinish = true;
                    canRevealActiveMask = false;
                    MS_ElapsedTime = 0f;
                    SwapMaskValues();
                }
            }

            if(maskRevealFinish)
            {
                time_2 += Time.deltaTime;
                if (time_2 >= time_to_wait_after_mr)
                {
                    MaskShiftLerp(false, true);
                    canRevealMaskUI = false;
                }
            }
        }
    }

    void HandlePandP()
    {
        RaycastHit2D ray_1 = Physics2D.Raycast(transform.position, Vector2.left, checkLength, mo_layer);
        RaycastHit2D ray_2 = Physics2D.Raycast(transform.position, Vector2.right, checkLength, mo_layer);
        bool is_in_contact = ray_1.collider != null || ray_2.collider != null;    
        Ref_Obj = is_in_contact ? (ray_1.collider != null ? ray_1.collider.gameObject : ray_2.collider.gameObject) : Ref_Obj;
        if (is_in_contact)
        {
            isForward = ray_2.collider != null;
            if(Input.GetMouseButtonDown(1))
            {
                //animator.SetBool("push_idle", true);
                isHoldingSomething = true;
                Ref_Obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                FixedJoint2D joint = Ref_Obj.AddComponent<FixedJoint2D>();
                joint.connectedBody = rb;
                joint.enableCollision = false;
            }
            if(Input.GetMouseButton(1))
            {
                //animator.SetBool("canPush", rb.linearVelocityX != 0);
            }
            if (Input.GetMouseButtonUp(1))
            {
                animator.SetBool("push_idle", false);
                animator.SetBool("canPush", false);
                isHoldingSomething = false;
                main_force_value = 0f;
                if(Ref_Obj)
                {
                    Ref_Obj.GetComponent<Rigidbody2D>().linearVelocityX = 0f;
                    FixedJoint2D joint = Ref_Obj.GetComponent<FixedJoint2D>();
                    if (joint) Destroy(joint);
                }
                Ref_Obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
        } else
        {
            animator.SetBool("canPush", false);
            isHoldingSomething = false;
            if(Ref_Obj)
            {
                Ref_Obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            Ref_Obj = null;
        }

        if(isHoldingSomething)
        {
            bool isKey = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) isKey = false;
            if (isKey)
            {
                int x = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);
                animator.SetBool("canPush", (isForward ? (x > 0f ? true : false) : (x < 0f ? true : false)));
                animator.SetBool("canPull", (isForward ? (x < 0f ? true : false) : (x > 0f ? true : false)));

                //animator.SetBool
                float t = pp_elapsedTime /
                    (Input.GetKey(KeyCode.A) ?
                    (isForward ? pull_force_changeTime : push_force_changeTime) :
                    (isForward ? push_force_changeTime : pull_force_changeTime));
                main_force_value = Mathf.Lerp(
                    neutral_force_value, 
                    (isForward ? (x < 0 ? pullForce : x > 0 ? pushForce : 0) : (x < 0 ? -pushForce : x > 0 ? -pullForce : 0)), 
                    t
                );
                pp_elapsedTime += Time.deltaTime;
            } else
            {
                animator.SetBool("canPush", false);
                animator.SetBool("canPull", false);
                pp_elapsedTime = 0f;
                main_force_value = 0f;
            }
        }

    }

    void HandleMovement()
    {
        if(!isHoldingSomething)
        {
            x = Input.GetAxis("Horizontal");
            animator.SetBool("isrunning", x != 0f);
        }
        if(x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            Canvas.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        } 

        if(x > 0f)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            Canvas.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        if(Input.GetKeyDown(KeyCode.Space) && !is_wall)
        {   
            if(canDo_DoubleJump)
            {
                jumpIndex++;
            }
            space_clicked = 
                ((isGrounded && !isHoldingSomething) || is_swinging) || 
                (canDo_DoubleJump ? jumpIndex <= maxJumps : false);
        }
    }

    void HandleDoubleJump()
    {
        canDo_DoubleJump = RealmGameManager.instance.currentRealm == 3;
    }

    void HandleDashing()
    {
        if (isHoldingSomething || isClicking_MS_Tab || is_swinging || RealmGameManager.instance.currentRealm != 1) return;
        int x = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);
        if(Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && x != 0)
        {
            if(x < 0)
            {
                DashForce = -DashForce;
            }
            isDashing = true;
        }

        if(isDashing)
        {
            float t = DashElapsedTime / DashResetTime;
            DashForceDynamic = Mathf.Lerp(DashForce, 0, t);
            DashElapsedTime += Time.deltaTime;
            if(t >= 1)
            {
                DashElapsedTime = 0f;
                isDashing = false;
                DashForce = Mathf.Abs(DashForce);
            }
        }
    }

    void HandleInputs()
    {
        if(!isHoldingSomething)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                RealmGameManager.instance.ChangeRealm(0);
            } else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                RealmGameManager.instance.ChangeRealm(1);
            } else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                RealmGameManager.instance.ChangeRealm(2);
            }
        }
    }

    void HandleCamera()
    {
        if (!is_cameraMoving) return;
        float t = CameraElapsedTime / MoveTime;

        Vector2 newPos = Vector2.Lerp(Cam_startPos, Cam_endPos, t);
        float newSize = Mathf.Lerp(Cam_startSize, Cam_endSize, t);

        Camera.main.transform.position = new Vector3(newPos.x, newPos.y, -10f);
        Camera.main.orthographicSize = newSize;

        CameraElapsedTime += Time.deltaTime;

        if (t >= 1)
        {
            CameraElapsedTime = 0f;
            is_cameraMoving = false;
        }
    }

    void HandleSwinging()
    {
        if(is_insideSwingBounds)
        {
            if(Input.GetMouseButtonDown(1))
            {
                canSwing = true;
                swing_player_startPos = transform.position;
                swing_bar_pos = new Vector2(SwingBarObject.transform.position.x, SwingBarObject.transform.position.y - swingbarOffset);
                rb.gravityScale = 0f;
            }
        }

        if(canSwing)
        {
            float t = swing_elapsedTime / swing_lerpTime;
            Vector3 newPlayerPos = Vector3.Lerp(swing_player_startPos, swing_bar_pos, t);
            swing_elapsedTime += Time.deltaTime;
            transform.position = newPlayerPos;
            if (t >= 1)
            {
                is_swinging = true;
                is_insideSwingBounds = false;
                canSwing = false;
                swing_elapsedTime = 0f;
                DJ_2D = transform.AddComponent<DistanceJoint2D>();
                DJ_2D.connectedAnchor = new Vector2(swing_bar_pos.x, swing_bar_pos.y + swingbarOffset);
                DJ_2D.autoConfigureDistance = false;
                DJ_2D.distance = swing_distance;
                DJ_2D.enableCollision = true;
                rb.gravityScale = initialGravityScale;
                rb.linearVelocity = new Vector2(rb.linearVelocityX / 3, rb.linearVelocityY / 3);
            }
        }

        if(is_swinging)
        {
            Vector2 distance = (SwingBarObject.transform.position - transform.position).normalized;
            float z_deg = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0f, 0f, z_deg);
        }
    }

    void HandleMaskShifting()
    {
        if (is_obtaining_Mask) return;
        if(Input.GetMouseButtonDown(2) && !is_changing_masks)
        {
            RealmGameManager.instance.nextRealm = 0;
            RealmGameManager.instance.ConfirmRealm();
            SetMaskShiftInitials();
            isClicking_MS_Tab = true;
            //x = 0f;
            Time.timeScale = 0.1f;
        }

        if(Input.GetMouseButtonUp(2) && isClicking_MS_Tab)
        {
            Time.timeScale = 1f;
            x = 0f;
            if (RealmGameManager.instance.nextRealm != -1)
            {
                if(RealmGameManager.instance.currentRealm != RealmGameManager.instance.nextRealm)
                {
                    RealmGameManager.instance.ConfirmRealm();
                }
            } else
            {
                if(RealmGameManager.instance.prev_realm != 0)
                {
                    RealmGameManager.instance.nextRealm = RealmGameManager.instance.prev_realm;
                    RealmGameManager.instance.ConfirmRealm();
                }
            }
            MS_ElapsedTime = 0f;
            isClicking_MS_Tab = false;
            is_changing_masks = true;
            MS_initialScale = Masks[0].transform.localScale.x;
            initialAlpha = BlendInBG.GetComponent<SpriteRenderer>().color.a;
            if(!do_once)
            {
                SwapMaskValues();
            }
            for(int i = 0; i < Masks.Length;i++)
            {
                initialMaskPositions[i] = Masks[i].transform.position;
            }
        }

        if (isClicking_MS_Tab)
        {
            if (!do_once)
            {
                MaskShiftLerp(true);
            }
        }
        else
        {
            if (is_changing_masks)
            {
                MaskShiftLerp(false);
            }
        }
    }

    void MaskShiftLerp(bool val, bool is_showing = false)
    {
        float t = MS_ElapsedTime / MS_LerpTime;
        float newScale = Mathf.Lerp(MS_initialScale, MS_TargetScale, t);
        Color newAlphaColor = Color.Lerp(new Color(0, 0, 0, initialAlpha), new Color(0, 0, 0, FinalAlpha), t);
        for (int i = 0; i < Masks.Length; i++)
        {
            Vector2 NewPosition = Vector2.Lerp(initialMaskPositions[i], targetMaskPositions[i], t);
            mask_final_positions[i] = NewPosition;
        }
        MS_ElapsedTime += Time.unscaledDeltaTime;
        for (int i = 0; i < Masks.Length; i++)
        {
            Masks[i].transform.position = mask_final_positions[i];
            Masks[i].transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        BlendInBG.GetComponent<SpriteRenderer>().color = newAlphaColor;
        if (t >= 1)
        {
            if(!val)
            {
                for (int i = 0; i < Masks.Length; i++)
                {
                    if (Masks[i].gameObject.name[1] - '0' <= RealmGameManager.instance.masks_unlocked)
                    {
                        Masks[i].GetComponent<Image>().color = Color.white;
                    }
                }
            }
            if(!is_showing)
            {
                is_changing_masks = val;
                do_once = val;
                MS_ElapsedTime = 0f;
                SwapMaskValues();
            } else
            {
                if(val)
                {
                    Destroy(CurrentMaskToReveal);
                    time_1 += Time.unscaledDeltaTime;
                    if(time_1 >= time_to_wait_before_mr)
                    {
                        canRevealActiveMask = true;
                    }
                } else
                {
                    time_1 = 0f;
                    time_2 = 0f;
                    MaskRevealElapsedTime = 0f;
                    canRevealMaskUI = false;
                    canRevealActiveMask = false;
                    is_insideMaskBounds = false;
                    is_obtaining_Mask = false;
                    maskRevealFinish = false;
                    MS_ElapsedTime = 0f;
                    RealmGameManager.instance.masks_unlocked++;
                }
            }
        }
    }

    void SwapMaskValues()
    {
        MS_initialScale = MS_TargetScale;
        MS_TargetScale = MS_initialScaleFixed;

        initialAlpha = FinalAlpha;
        FinalAlpha = initialAlphaFixed;

        for (int i = 0; i < Masks.Length; i++)
        {
            Vector2 tempPos = initialMaskPositions[i];
            initialMaskPositions[i] = targetMaskPositions[i];
            targetMaskPositions[i] = tempPos;
        }
    }

    public void SetCanReveal(bool val)
    {
        canRevealMaskUI = val;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(JumpTransform.position, Jump_CircleRadius, JumpLayer);
        if(!canSwing && !is_swinging && !isDashing && !is_wall)
        {
            if (!isHoldingSomething)
            {
                rb.linearVelocityX = x * speed;
            }
            else
            {
                rb.linearVelocityX = main_force_value;
                Ref_Obj.GetComponent<Rigidbody2D>().linearVelocityX = rb.linearVelocityX;
            }

            if(space_clicked)
            {
                rb.linearVelocityY = jumpForce;
                space_clicked = false;
            }
            if (isGrounded)
            {
                if (jumpIndex >= maxJumps) { jumpIndex = 0; }
            }

            if(Is_Trampoline)
            {
                rb.linearVelocityY = TrampolineForce;
                Is_Trampoline = false;
            }
        }

        if (is_swinging)
        {
            rb.AddForce(Vector2.right * swingForce * x, ForceMode2D.Force);
            if(space_clicked)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                space_clicked = false;
                is_insideSwingBounds = false;
                is_swinging = false;
                rb.gravityScale = initialGravityScale;
                rb.AddForceY(SwingJumpForce, ForceMode2D.Impulse);
                Destroy(DJ_2D);
            }
        }

        if(isDashing)
        {
            rb.linearVelocityX = DashForceDynamic;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "tramp")
        {
            Is_Trampoline = true;
        }

        if (collision.gameObject.tag == "CamZones")
        {
            is_cameraMoving = true;
            CameraElapsedTime = 0f;
            Cam_startPos = Camera.main.transform.position;
            Cam_endPos = collision.transform.position;

            Cam_startSize = Camera.main.orthographicSize;
            Cam_endSize = collision.transform.GetComponent<CamData>().camSize;
        }

        if(collision.gameObject.tag == "hinge")
        {
            is_insideSwingBounds = true;
            SwingBarObject = collision.transform.gameObject;
            collision.transform.GetChild(0).gameObject.SetActive(true);
        }

        if(collision.gameObject.tag == "GameMask")
        {
            is_insideMaskBounds = true;
            collision.gameObject.GetComponent<Mask>().ResetValues(true);
            CurrentMaskToReveal = collision.gameObject;
            maskToReveal = collision.gameObject.GetComponent<Mask>().maskIndex;
        }

        if(collision.gameObject.tag == "TutorialBlock")
        {
            if(!collision.gameObject.GetComponent<TutorialBlock>().is_mini_tutorial)
            {
                is_showing_tutorial = true;
            }
            collision.gameObject.GetComponent<TutorialBlock>().is_showing_tutorial = true;
            collision.gameObject.GetComponent<TutorialBlock>().player = gameObject;
            TutorialObject = collision.gameObject;
        }

        if(collision.gameObject.tag == "Death")
        {
            rb.linearVelocity = new Vector2(0f, 0f);
            rb.gravityScale = 0f;
            is_dead = true;
            camStartPos = Camera.main.transform.position;
            camEndPos = SafePositionObject.transform.GetChild(0).transform.position;
            PlayerStartColor = GetComponent<SpriteRenderer>().color;
            PlayerEndColor = GetComponent<SpriteRenderer>().color;
            PlayerStartColor.a = 1;
            PlayerEndColor.a = 0;
        }

        if(collision.gameObject.tag == "safePoint")
        {
            SafePositionObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "hinge")
        {
            is_insideSwingBounds = false;
            is_swinging = false;
            rb.gravityScale = initialGravityScale;
            Destroy(DJ_2D);
            SwingBarObject = null;
            collision.transform.GetChild(0).gameObject.SetActive(false);
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }

        if(collision.gameObject.tag == "GameMask")
        {
            is_insideMaskBounds = false;
            if(!is_obtaining_Mask)
            {
                collision.gameObject.GetComponent<Mask>().ResetValues(false);
                CurrentMaskToReveal = null;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "wall" && canHoldWall)
        {
            is_wall = true;
            rb.linearVelocityY = 0f;
            rb.gravityScale = 0f;
            if (jumpIndex >= maxJumps) { jumpIndex = 0; }        
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "wall" && is_wall)
        {
            StartCoroutine(SetIsWall());
        }
    }

    IEnumerator SetIsWall()
    {
        yield return new WaitForSeconds(0.2f);
        is_wall = false;
    }
}
