using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WallPlayer : MonoBehaviour
{
    [Header("Movement")]

    float x;

    [SerializeField]
    float speed;

    Rigidbody2D rb;

    [Header("Jump")]

    [SerializeField]
    float JumpForce;

    [SerializeField]
    LayerMask jumpLayer;

    [SerializeField]
    Transform jump_transform;

    [SerializeField]
    float jumpRadius;

    [SerializeField]
    bool isGrounded, spaceClicked;

    [Header("Camera")]

    [SerializeField]
    bool is_cameraMoving = false;

    [SerializeField]
    Vector2 Cam_startPos, Cam_endPos;

    [SerializeField]
    float Cam_startSize, Cam_endSize;

    [SerializeField]
    float CameraElapsedTime, MoveTime;

    [Header("Recording")]

    [SerializeField]
    List<Vector3> Record_Positions = new List<Vector3>();

    [SerializeField]
    float record_time, record_interval = 0.1f;

    [SerializeField]
    bool isshowing_recording = false;

    [SerializeField]
    GameObject RecodPlayer;

    [SerializeField]
    GameObject Spawned_RP;

    [Header("Wall Jump")]

    [SerializeField]
    bool is_wall = false;

    [SerializeField]
    float wall_time, wall_friction_time;

    [SerializeField]
    bool canHoldWall = true;

    [SerializeField]
    float initialGravityScale;

    [SerializeField]
    Vector2 wall_jumpForce;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if(!isshowing_recording && !is_wall)
        {
            x = Input.GetAxis("Horizontal");
        }
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            spaceClicked = true;
        }

        HandleCamera();
        HandleRecord();
        HandleWallJump();
        HandleCanHoldWall();
    }

    private void FixedUpdate()
    {
        if (!is_wall)
        {
            rb.linearVelocityX = x * speed;
        }

        isGrounded = Physics2D.OverlapCircle(jump_transform.position, jumpRadius, jumpLayer);

        if(isGrounded && spaceClicked)
        {
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            spaceClicked = false;
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

        if(t >= 1)
        {
            CameraElapsedTime = 0f;
            is_cameraMoving = false;
        }
    }

    void HandleWallJump()
    {
        if (!is_wall) return;
        wall_time += Time.deltaTime;
        if(wall_time >= wall_friction_time)
        {
            canHoldWall = false;
            rb.gravityScale = initialGravityScale;
        } else
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                int multiplier = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
                rb.AddForce(new Vector2(wall_jumpForce.x * multiplier, wall_jumpForce.y), ForceMode2D.Impulse);
                wall_time = 0f;
            }
        }
    }

    void HandleCanHoldWall()
    {
        if(!canHoldWall)
        {
            if(isGrounded)
            {
                canHoldWall = true;
                wall_time = 0f;
                is_wall = false;
            }
        }
    }

    void HandleRecord()
    {
        if(!isshowing_recording)
        {
            record_time += Time.deltaTime;
            if (record_time > record_interval)
            {
                Record_Positions.Add(transform.position);
                record_time = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isshowing_recording)
        {
            isshowing_recording = true;
            Spawned_RP = Instantiate(RecodPlayer, Record_Positions[0], Quaternion.identity);
            StartCoroutine(SetRP_positions());
        }
    }

    IEnumerator SetRP_positions()
    {
        for(int i = 0; i < Record_Positions.Count; i++)
        {
            yield return new WaitForSeconds(record_interval);
            Spawned_RP.transform.position = Record_Positions[i];
            if(i == Record_Positions.Count - 1)
            {
                isshowing_recording = false;
                Record_Positions.Clear();
                Destroy(Spawned_RP);
                Spawned_RP = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "CamZones")
        {
            is_cameraMoving = true;
            CameraElapsedTime = 0f;
            Cam_startPos = Camera.main.transform.position;
            Cam_endPos = collision.transform.position;

            Cam_startSize = Camera.main.orthographicSize;
            Cam_endSize = collision.transform.GetComponent<CamData>().camSize;
        }  
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "wall" && canHoldWall)
        {
            is_wall = true;
            rb.linearVelocityY = 0f;
            rb.gravityScale = 0f;
        }
    }
}
