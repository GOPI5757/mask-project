using UnityEngine;

public class BackgroundBlending : MonoBehaviour
{
    [SerializeField] GameObject[] background_textures;

    [SerializeField] int _1_oil, _2_oil;
    [SerializeField] public int index_1, index_2;
    [SerializeField] Color c_1, c_2;

    [SerializeField] float elapsedTime, lerpTime, time_1, waitTime;

    void Start()
    {
        c_1.a = 1;
        c_2.a = 0;
        background_textures[index_1].GetComponent<SpriteRenderer>().sortingOrder = _1_oil;
        background_textures[index_2].GetComponent<SpriteRenderer>().sortingOrder = _2_oil;

        background_textures[index_1].GetComponent<SpriteRenderer>().color = c_1;
        background_textures[index_2].GetComponent<SpriteRenderer>().color = c_2;
    }

    void Update()
    {
        if (RealmGameManager.instance.HasChanged)
        {
            elapsedTime = 0f;
            time_1 = 0f;
            return;
        }
        float t = elapsedTime / lerpTime;
        Color newColor_1 = Color.Lerp(c_1, c_2, t);
        Color newColor_2 = Color.Lerp(c_2, c_1, t);

        background_textures[index_1].GetComponent<SpriteRenderer>().color = newColor_1;
        background_textures[index_2].GetComponent<SpriteRenderer>().color = newColor_2;

        elapsedTime += Time.deltaTime;
        if(t >= 1)
        {
            time_1 += Time.deltaTime;
            if(time_1 >= waitTime)
            {
                index_1 = ChangeIndex(index_1);
                index_2 = ChangeIndex(index_2);
                time_1 = 0f;
                elapsedTime = 0f;
            }
        }
    }

    int ChangeIndex(int value)
    {
        if(++value >= background_textures.Length)
        {
            value = 0;
        }
        return value;
    }
}
