using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField] GameObject MenuPanel, CreditsPanel, SettingsPanel;

    [SerializeField] GameObject Audio_Panel, Controls_Panel;
    [SerializeField] Image AudioButtonImage, ControlsButtonImage;
    [SerializeField] Color NormalButtonColor, clickButtonColor;

    [SerializeField] Scrollbar controls_Scrollbar;

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
        Buttons[0].GetComponent<Image>().sprite = Mask_Textures_Color[0];
        Buttons[0].GetComponent<Image>().color = HoverColor;
        Buttons[0].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color = initial_font_colors[0];

    }

    public void SettingsButton()
    {
        SettingsPanel.SetActive(true);
        MenuPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        Audio_Panel.SetActive(true);
        Controls_Panel.SetActive(false);

        AudioButtonImage.color = clickButtonColor;
        ControlsButtonImage.color = NormalButtonColor;
        Buttons[1].GetComponent<Image>().sprite = Mask_Textures_Color[2];
        Buttons[1].GetComponent<Image>().color = HoverColor;
        Buttons[1].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color = initial_font_colors[1];
    }

    public void CreditsButton()
    {
        MenuPanel.SetActive(false);
        CreditsPanel.SetActive(true);
        Buttons[2].GetComponent<Image>().sprite = Mask_Textures_Color[2];
        Buttons[2].GetComponent<Image>().color = HoverColor;
        Buttons[2].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().color = initial_font_colors[2];
    }

    public void BackButton()
    {
        MenuPanel.SetActive(true);
        CreditsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AudioPanelButton()
    {
        Audio_Panel.SetActive(true);
        Controls_Panel.SetActive(false);
        AudioButtonImage.color = clickButtonColor;
        ControlsButtonImage.color = NormalButtonColor;

    }

    public void ControlPanelButton()
    {
        Audio_Panel.SetActive(false);
        Controls_Panel.SetActive(true);
        AudioButtonImage.color = NormalButtonColor;
        ControlsButtonImage.color = clickButtonColor;

        controls_Scrollbar.value = 1f;
    }
}
