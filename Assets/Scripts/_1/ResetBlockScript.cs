using System.Collections;
using UnityEngine;

public class ResetBlockScript : MonoBehaviour
{
    [SerializeField] Vector2 blockStartPos;

    public bool hasCollided = false;

    void Start()
    {
        StartCoroutine(GetStartPos());
    }

    IEnumerator GetStartPos()
    {
        yield return new WaitForSeconds(1f);
        blockStartPos = transform.position;
    }


    void Update()
    {
        if(hasCollided)
        {
            transform.position = blockStartPos;
            hasCollided = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "reset_block")
        {
            hasCollided = true;
        }
    }
}
