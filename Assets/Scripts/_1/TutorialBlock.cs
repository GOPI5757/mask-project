using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBlock : MonoBehaviour
{
    [SerializeField] public Texture TutorialImage;
    [SerializeField] public string TutorialTitle;
    [SerializeField] public string TutorialMessage;

    [SerializeField] float font_size;

    [SerializeField] GameObject MajorTutorial, MiniTutorial;

    [SerializeField] public bool is_mini_tutorial;

    [SerializeField] public bool has_tutorial_given;

    [SerializeField] float Time_1, stayTime, destroyTime;

    [Header("Major")]

    [SerializeField] float Time_2 = 0f, E_Appear_Time = 3f;
    [SerializeField] float x_offset, Speed;

    [SerializeField] Vector2 startPos_E, endPos_E;

    public bool is_showing_tutorial;
    public GameObject player;

    void Start()
    {
        startPos_E = MajorTutorial.transform.GetChild(3).transform.position;
        //endPos_E = MajorTutorial.transform.GetChild(3).transform.position - (Vector3.right * x_offset);
    }

    void Update()
    {
        if (is_mini_tutorial)
        {
            if (is_showing_tutorial)
            {
                MiniTutorial.SetActive(true);
                MiniTutorial.transform.GetChild(0).GetComponent<TMP_Text>().text = TutorialMessage;
                MiniTutorial.transform.GetChild(0).GetComponent<TMP_Text>().fontSize = font_size;
                Time_1 += Time.deltaTime;
                if(!has_tutorial_given)
                {
                    if(Time_1 >= stayTime)
                    {                       
                        MiniTutorial.GetComponent<Animator>().SetBool("flag", true);
                        has_tutorial_given = true;
                    }
                }
            }
            if(Time_1 >= stayTime + destroyTime)
            {
                MiniTutorial.GetComponent<Animator>().SetBool("flag", false);
                MiniTutorial.SetActive(false);
                Destroy(gameObject);
            }
        } else
        {
            if(is_showing_tutorial)
            {
                MajorTutorial.SetActive(true);
                MajorTutorial.transform.GetChild(0).GetComponent<RawImage>().texture = TutorialImage;
                MajorTutorial.transform.GetChild(1).GetComponent<TMP_Text>().text = TutorialTitle;
                MajorTutorial.transform.GetChild(2).GetComponent<TMP_Text>().text = TutorialMessage;

                MajorTutorial.transform.GetChild(2).GetComponent<TMP_Text>().fontSize = font_size;
                Time.timeScale = 0f;
                if (!player.GetComponent<RealmPlayer>().canPressE)
                {
                    Time_2 += Time.unscaledDeltaTime;
                }
                if(Time_2 >= E_Appear_Time)
                {
                    Time_2 = 0f;
                    endPos_E = MajorTutorial.transform.GetChild(3).transform.position - (Vector3.right * x_offset);
                    player.GetComponent<RealmPlayer>().canPressE = true;
                }

                if(player.GetComponent<RealmPlayer>().canPressE)
                {
                    Vector2 FinalPos = Vector2.MoveTowards(MajorTutorial.transform.GetChild(3).transform.position, endPos_E, Time.unscaledDeltaTime * Speed);
                    MajorTutorial.transform.GetChild(3).transform.position = FinalPos;
                }

                if (has_tutorial_given)
                {
                    Time.timeScale = 1f;
                    MajorTutorial.GetComponent<Animator>().SetBool("flag", true);
                    Time_1 += Time.unscaledDeltaTime;
                    if (Time_1 >= destroyTime)
                    {
                        MajorTutorial.GetComponent<Animator>().SetBool("flag", false);
                        MajorTutorial.SetActive(false);
                        MajorTutorial.transform.GetChild(3).transform.position = startPos_E;
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
