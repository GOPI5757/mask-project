using UnityEngine;
using UnityEngine.Rendering;

public class BG_Move : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float bg_endPoint;
    [SerializeField] float bg_size;
    [SerializeField] public GameObject AnotherBG;

    void Start()
    {

    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime, 0f, 0f);
        if(transform.position.x < bg_endPoint)
        {
            transform.position= new Vector3(AnotherBG.transform.position.x + bg_size, transform.position.y, transform.position.z);
            for(int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i).gameObject != gameObject)
                {
                    transform.parent.GetChild(i).gameObject.GetComponent<BG_Move>().AnotherBG = this.gameObject;
                }
            }
        }
    }
}
