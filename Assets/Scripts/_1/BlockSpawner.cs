using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField]
    public float TimeBetweenSpawn;

    [SerializeField]
    float normalTime, SlowTime;

    [SerializeField] GameObject PrefabToSpawn;

    [SerializeField]
    bool is_affected_by_sad = true;

    [SerializeField]
    public bool is_slow = false;

    [SerializeField]
    public float time_1, Delay;

    [SerializeField]
    public float normalDelay, SlowDelay;

    [SerializeField]
    public float main_time;

    [SerializeField]
    public bool is_opp = false;

    [SerializeField]
    bool canStart;

    void Start()
    {
        TimeBetweenSpawn = is_slow ? SlowTime : normalTime;
    }

    private void Update()
    {
        TimeBetweenSpawn = is_slow ? SlowTime : normalTime;
        if (!canStart)
        {
            time_1 += Time.deltaTime;
            if (time_1 >= Delay)
            {
                canStart = true;
                time_1 = 0f;
            }
        }

        if(canStart)
        {
            main_time += Time.deltaTime;
            if(main_time >= TimeBetweenSpawn)
            {
                //main_time = 0f - (is_slow ? SlowDelay : 0f);
                main_time = 0f;
                GameObject Object = Instantiate(PrefabToSpawn, transform.position, Quaternion.identity);
                Object.GetComponent<FallingBlock>().DestroyPosition = transform.GetChild(0).transform.position.y;
                Object.GetComponent<FallingBlock>().is_slow = RealmGameManager.instance.is_slow_Toggled;
            }
        }
    }

    IEnumerator SpawnEnumerator()
    {
        while(canStart)
        {
            yield return new WaitForSeconds(TimeBetweenSpawn);
        }
    }
}
