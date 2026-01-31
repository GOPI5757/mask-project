using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    Sprite[] Mask_Textures_Color, Mask_Textures_NoColor;

    [SerializeField] Color NormalColor, HoverColor;
    [SerializeField] Color[] font_colors;

    [SerializeField] List<Color> initial_font_colors = new List<Color>();

    [SerializeField] GameObject[] Buttons;

    [SerializeField] Image CloseScreen;
    [SerializeField] float closeElapsedTime, closeTime, time_1, wait_time;
    [SerializeField] bool shouldClose;

    private void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            initial_font_colors.Add(Buttons[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color);
            
        }
    }

    private void Update()
    {
        if(shouldClose)
        {
            float t = closeElapsedTime / closeTime;
            Color newColor = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), t);
            CloseScreen.color = newColor;
            closeElapsedTime += Time.deltaTime;
            if(t >= 1)
            {
                time_1 += Time.deltaTime;
                if(time_1 > wait_time)
                {
                    SceneManager.LoadScene(1);
                }
            }
        }   
    }

    public void HoverIn(int index)
    {
        if (shouldClose) return;
        Buttons[index].GetComponent<Image>().sprite = Mask_Textures_NoColor[index];
        Buttons[index].GetComponent<Image>().color = NormalColor;
        Buttons[index].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color = font_colors[index];
    }

    public void HoverOut(int index)
    {
        Buttons[index].GetComponent<Image>().sprite = Mask_Textures_Color[index];
        Buttons[index].GetComponent<Image>().color = HoverColor;
        Buttons[index].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color = initial_font_colors[index];
    }

    public void PlayGame()
    {
        shouldClose = true;
        CloseScreen.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
