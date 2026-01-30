using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed;

    Camera cam;

    Rigidbody2D rb;

    [SerializeField]
    float Cam_startPosX, Cam_nextPosX;

    [SerializeField]
    float elapsedTime, moveTime;

    [SerializeField]
    bool isMoving;

    [SerializeField]
    LayerMask jumpLayer;

    [SerializeField]
    bool isGrounded;

    [SerializeField]
    bool shouldJump = false;

    [SerializeField]
    float jumpForce;

    [SerializeField]
    bool isSwinging = false;

    [SerializeField]
    float swingForce;

    HingeJoint2D HingeJoint;
    DistanceJoint2D DistanceJoint;

    float x;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        HingeJoint = GetComponent<HingeJoint2D>();
        DistanceJoint = GetComponent<DistanceJoint2D>();
        cam = Camera.main;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if(!isMoving)
        {
        }

        x = Input.GetAxis("Horizontal");

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            shouldJump = true;
        }
 
        //rb.AddForce(Vector2.right * x * speed, ForceMode2D.Force);
        //MoveCamera();
    }

    private void FixedUpdate()
    {
        if(!isSwinging)
        {
            rb.linearVelocityX = x * speed;
        } else
        {
            rb.AddForce(Vector2.right * x * swingForce, ForceMode2D.Force);
        }

        isGrounded = Physics2D.OverlapCircle(transform.GetChild(1).transform.position, 0.1f, jumpLayer);
        if(isGrounded && shouldJump)
        {
            shouldJump = false;
            rb.linearVelocityY = jumpForce;
        }
    }

    void MoveCamera()
    {
        if (!isMoving) return;
        rb.linearVelocity = Vector3.zero;
        x = 0f;
        float t = elapsedTime / moveTime;
        float newX = Mathf.Lerp(Cam_startPosX, Cam_nextPosX, t);
        cam.transform.position = new Vector3(newX, cam.transform.position.y, cam.transform.position.z);
        elapsedTime += Time.deltaTime;
        if(t >= 1)
        {
            elapsedTime = 0f;
            isMoving = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Cam_startPosX = cam.transform.position.x;
            Cam_nextPosX = collision.transform.position.x;
            isMoving = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "hinge")
        {
            isSwinging = true;
            rb.constraints = RigidbodyConstraints2D.None;
            //HingeJoint.enabled = true;
            //HingeJoint.connectedBody = collision.attachedRigidbody;

            DistanceJoint.enabled = true;
            DistanceJoint.connectedBody = collision.attachedRigidbody;
        }
    }
}
