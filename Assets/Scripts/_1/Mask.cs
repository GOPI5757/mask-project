using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mask : MonoBehaviour
{
    [SerializeField] public bool has_obtained = false;
    [SerializeField] public int maskIndex;

    [SerializeField] float elapsedTime, showTime;

    [SerializeField] float y_offset;

    [SerializeField] float start_yFixed, end_yFixed;
    float lerp_y_a, lerp_y_b;

    [SerializeField] float scaleTime;

    [SerializeField] Color color_withAlpha, color_withoutAlpha;
    Color lerp_color_a, lerp_color_b;

    GameObject Player;

    [SerializeField] Transform E_UI_Element;


    void Start()
    {
        start_yFixed = E_UI_Element.localPosition.y;
        color_withAlpha = E_UI_Element.gameObject.GetComponent<TMP_Text>().color;
        color_withAlpha.a = 1;
        color_withoutAlpha = E_UI_Element.gameObject.GetComponent<TMP_Text>().color;
        color_withoutAlpha.a = 0;
        end_yFixed = start_yFixed + y_offset;
        ResetValues(false);
    }

    void Update()
    {
        if(!has_obtained)
        {
            float t = elapsedTime / showTime;
            float new_y = Mathf.Lerp(lerp_y_a, lerp_y_b, t);
            Color newColor = Color.Lerp(lerp_color_a, lerp_color_b, t);
            elapsedTime += Time.deltaTime;
            E_UI_Element.transform.localPosition = new Vector3(0f, new_y, 0f);
            E_UI_Element.gameObject.GetComponent<TMP_Text>().color = newColor;
        } else
        {
            float t = elapsedTime / scaleTime;
            float new_Scale = Mathf.Lerp(1, 0, t);
            transform.localScale = Vector3.one * new_Scale;
            elapsedTime += Time.deltaTime;

            if(t >= 1)
            {
                Player.GetComponent<RealmPlayer>().SetCanReveal(true);
            }
        }

    }

    public void ResetValues(bool val)
    {
        elapsedTime = 0f;
        lerp_y_a = E_UI_Element.localPosition.y;
        lerp_y_b = val ? end_yFixed : start_yFixed;

        lerp_color_a = E_UI_Element.gameObject.GetComponent<TMP_Text>().color;
        lerp_color_b = val ? color_withAlpha : color_withoutAlpha;
    }

    public void HasObtained(GameObject player_obj)
    {
        has_obtained = true;
        elapsedTime = 0f;
        Player = player_obj;
    }
}
