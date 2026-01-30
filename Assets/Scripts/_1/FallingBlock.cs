using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    [SerializeField]
    float downSpeed;

    [SerializeField]
    float normalSpeed, slowSpeed;

    [SerializeField]
    public float DestroyPosition;

    [SerializeField]
    bool is_affected_by_sad = true;

    [SerializeField]
    public bool is_slow = false;

    private void Start()
    {
        downSpeed = is_slow ? slowSpeed : normalSpeed;
    }

    private void Update()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.GetChild(1).transform.position, 0.01f);
        if(col != null)
        {
            if(col.gameObject != gameObject)
            {
                if(col.gameObject.tag == "FallingBlock")
                {
                    Destroy(col.gameObject);
                }
            }
        }
        downSpeed = is_slow ? slowSpeed : normalSpeed;
        transform.Translate(0f, downSpeed * Time.deltaTime, 0f);
        if(transform.position.y <= DestroyPosition)
        {
            Destroy(gameObject);
        }
    }
}
