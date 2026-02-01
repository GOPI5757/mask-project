using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[System.Serializable]
struct BlockColorData
{
    public Color BlockColor_1;
    public Color BlockColor_2;
}

[System.Serializable]
struct BG_Tex_Data
{
    public Sprite[] color_sprites;
}

public class RealmGameManager : MonoBehaviour
{
    [Header("Realm")]

    [SerializeField] public int prev_realm;
    [SerializeField] public int currentRealm = 0, nextRealm;
    [SerializeField] public bool HasChanged = false;

    [Header("Realm Changes")]

    [SerializeField] Color[] CameraColors;

    [SerializeField]
    BlockColorData[] bc_datas;

    [SerializeField] float elapsedTime, transitionTime;

    [SerializeField]
    GameObject[] MainObjects;

    [SerializeField] float hoverMaskSize;

    [Header("Background")]
    
    [SerializeField]
    GameObject[] Background_Textues;
    [SerializeField] public float BG_ElapsedTime;

    [SerializeField] BG_Tex_Data[] BGT_Datas;

    [SerializeField] bool is_fading = true;

    [Header("Mask")]

    [SerializeField] public int masks_unlocked;

    [Header("Slow motion Toggle")]

    [SerializeField] float blur_value_low, blur_value_high;
    [SerializeField] Color normalColor, blurColor;
    [SerializeField] float smt_elapsedTime, smt_transitionTime;

    [SerializeField] bool is_toggling_slowMode;
    [SerializeField] bool is_toggling_backwards;

    [SerializeField]
    public bool is_slow_Toggled;

    [SerializeField]
    Volume Global_Volume;

    [SerializeField] DepthOfField depthOf_field;
    [SerializeField] ColorAdjustments color_Adjustment;

    [SerializeField] bool isPaused;

    [SerializeField] GameObject PauseMenu;

    public static RealmGameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        if (TransferData.instance != null)
        {
            currentRealm = TransferData.instance.currentRealm;
            prev_realm = TransferData.instance.prevRealm;
            nextRealm = TransferData.instance.nextRealm;

            masks_unlocked = TransferData.instance.masks_Unlocked;
            Destroy(TransferData.instance.gameObject);
        }
    }

    private void Start()
    {
        //if(TransferData.instance != null)
        //{
        //    currentRealm = TransferData.instance.currentRealm;
        //    prev_realm = TransferData.instance.prevRealm;
        //    nextRealm = TransferData.instance.nextRealm;

        //    masks_unlocked = TransferData.instance.masks_Unlocked;
        //    Destroy(TransferData.instance.gameObject);
        //}

        Background_Textues = GameObject.FindGameObjectsWithTag("BG");
        Camera.main.backgroundColor = CameraColors[currentRealm];
        for (int i = 0; i < Background_Textues.Length; i++)
        {
            Background_Textues[i].GetComponent<SpriteRenderer>().sprite = BGT_Datas[currentRealm].color_sprites[(Background_Textues[i].gameObject.name[1] - '0') - 1];
        }

        Global_Volume.profile.TryGet<DepthOfField>(out depthOf_field);
        Global_Volume.profile.TryGet<ColorAdjustments>(out color_Adjustment);

        //Time.timeScale = 0.2f;
    }

    private void Update()
    {
        HandleChange();
        HandleSlowToggle();
        HandleSMT();
        HandlePause();
    }

    void HandlePause() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        PauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    void HandleChange()
    {
        if(HasChanged)
        {
            float t = elapsedTime / transitionTime;
            float t1 = BG_ElapsedTime / (transitionTime / 2);
            Color cam_color = Color.Lerp(
                CameraColors[currentRealm],
                CameraColors[nextRealm], 
                t
            );

            RealmData[] Interactables = FindObjectsByType<RealmData>(FindObjectsSortMode.None);

            for(int i = 0; i < Interactables.Length; i++)
            {
                Color int_color = Interactables[i].GetComponent<SpriteRenderer>().color;
                if (Interactables[i].supporting_realm == nextRealm)
                {
                    Color newColor = Color.Lerp(
                        new Color(int_color.r, int_color.g, int_color.b, 1),
                        new Color(int_color.r, int_color.g, int_color.b, 0),
                        t
                    );

                    Interactables[i].GetComponent<SpriteRenderer>().color = newColor;
                } else
                {
                    Color newColor = Color.Lerp(
                        new Color(int_color.r, int_color.g, int_color.b, 0),
                        new Color(int_color.r, int_color.g, int_color.b, 1),
                        t
                    );

                    Interactables[i].GetComponent<SpriteRenderer>().color = newColor;
                }
            }

            for (int i = 0; i < Background_Textues.Length; i++)
            {
                if(!is_fading)
                {
                    if ((Background_Textues[i].gameObject.name[1] - '0') - 1 == Background_Textues[i].transform.parent.gameObject.GetComponent<BackgroundBlending>().index_1)
                    {
                        Color a = Color.white;
                        a.a = 1;
                        Color b = Color.white;
                        b.a = 0;
                        Color BG_Color = Color.Lerp(b, a, t1);
                        Background_Textues[i].GetComponent<SpriteRenderer>().color = BG_Color;
                    }
                } else
                {
                    Color b = Color.white;
                    b.a = 0;
                    Color BG_Color = Color.Lerp(Background_Textues[i].GetComponent<SpriteRenderer>().color, b, t1);
                    Background_Textues[i].GetComponent<SpriteRenderer>().color = BG_Color;
                }
            }
            Camera.main.backgroundColor = cam_color;

            elapsedTime += Time.unscaledDeltaTime;
            BG_ElapsedTime += Time.unscaledDeltaTime;

            if (t >= 1)
            {
                HasChanged = false;
                elapsedTime = 0f;
                BG_ElapsedTime = 0f;

                for (int i = 0; i < Interactables.Length; i++)
                {
                    if (Interactables[i].supporting_realm == nextRealm)
                    {
                        Interactables[i].GetComponent<Collider2D>().enabled = false;
                        for(int j = 0; j < Interactables[i].transform.childCount; j++)
                        {
                            if(Interactables[i].transform.GetChild(j).transform.gameObject.GetComponent<Collider2D>())
                            {
                                Interactables[i].transform.GetChild(j).transform.gameObject.GetComponent<Collider2D>().enabled = false;
                            }
                        }
                    } else
                    {
                        Interactables[i].GetComponent<Collider2D>().enabled = true;
                        for (int j = 0; j < Interactables[i].transform.childCount; j++)
                        {
                            if (Interactables[i].transform.GetChild(j).transform.gameObject.GetComponent<Collider2D>())
                            {
                                Interactables[i].transform.GetChild(j).transform.gameObject.GetComponent<Collider2D>().enabled = true;
                            }
                        }
                    }
                }
                prev_realm = currentRealm;
                currentRealm = nextRealm;
                nextRealm = -1;

                if(currentRealm != 2)
                {
                    is_slow_Toggled = false;
                    depthOf_field.focalLength.value = blur_value_low;
                    color_Adjustment.colorFilter.value = normalColor;
                    BlockSpawner[] b_spawners = FindObjectsByType<BlockSpawner>(FindObjectsSortMode.None);
                    for (int i = 0; i < b_spawners.Length; i++)
                    {
                        b_spawners[i].is_slow = false;
                        //b_spawners[i].main_time = 0f + (b_spawners[i].is_opp ? b_spawners[i].TimeBetweenSpawn / 2 : 0);
                    }

                    FallingBlock[] falling_blocks = FindObjectsByType<FallingBlock>(FindObjectsSortMode.None);
                    for (int i = 0; i < falling_blocks.Length; i++)
                    {
                        falling_blocks[i].is_slow = false;
                    }
                }
                is_fading = true;
            }

            if(t1 >= 1)
            {
                BG_ElapsedTime = 0f;
                if(is_fading)
                {
                    is_fading = false;
                    for(int i = 0; i < Background_Textues.Length; i++)
                    {
                        if(nextRealm != -1)
                        {
                            Background_Textues[i].GetComponent<SpriteRenderer>().sprite = BGT_Datas[nextRealm].color_sprites[(Background_Textues[i].gameObject.name[1] - '0') - 1];
                        }
                    }
                }
            }
        }
    }

    void HandleSMT()
    {
        if (!is_toggling_slowMode) return;
        float t = smt_elapsedTime / smt_transitionTime;
        float newValue = Mathf.Lerp(
            is_slow_Toggled ? blur_value_low : blur_value_high, 
            is_slow_Toggled ? blur_value_high : blur_value_low, 
            t
        );

        Color newColor = Color.Lerp(
            is_slow_Toggled ? normalColor : blurColor,
            is_slow_Toggled ? blurColor : normalColor,
            t
        );
        smt_elapsedTime += Time.deltaTime;

        depthOf_field.focalLength.value = newValue;
        color_Adjustment.colorFilter.value = newColor;

        if(t >= 1)
        {
            is_toggling_slowMode = false;
            smt_elapsedTime = 0f;
        }
    }

    void HandleSlowToggle()
    {
        if (currentRealm != 2 || is_toggling_slowMode) return;
        if(Input.GetKeyDown(KeyCode.Q))
        {
            is_slow_Toggled = !is_slow_Toggled;
            is_toggling_slowMode = true;
            BlockSpawner[] b_spawners = FindObjectsByType<BlockSpawner>(FindObjectsSortMode.None);
            for(int i = 0; i < b_spawners.Length; i++)
            {
                b_spawners[i].is_slow = is_slow_Toggled;
                b_spawners[i].main_time = 0f + (b_spawners[i].is_opp ? b_spawners[i].TimeBetweenSpawn / 2 : 0);
            }

            FallingBlock[] falling_blocks = FindObjectsByType<FallingBlock>(FindObjectsSortMode.None);
            for(int i = 0; i < falling_blocks.Length; i++)
            {
                falling_blocks[i].is_slow = is_slow_Toggled;
            }
        }
    }

    public void ChangeRealm(int next_Realm)
    {
        if (HasChanged) return;
        nextRealm = next_Realm;
        HasChanged = true;
    }

    public void ConfirmRealm()
    {
        HasChanged = true;
    }

    public void HoverRealm(GameObject MaskBtn)
    {
        if (HasChanged) return;
        if(MaskBtn.gameObject.name[1] - '0' <= masks_unlocked)
        {
            nextRealm = (MaskBtn.gameObject.name[1] - '0');
            MaskBtn.GetComponent<Image>().color = Color.gray;
            MaskBtn.transform.localScale = new Vector3(hoverMaskSize, hoverMaskSize, hoverMaskSize);
        }
        else
        {
            nextRealm = -1; 
        }
    }

    public void HoverOutRealm(GameObject MaskBtn)
    {
        if (HasChanged) return;
        if(nextRealm <= masks_unlocked && nextRealm > -1)
        {
            MaskBtn.GetComponent<Image>().color = Color.white;
            MaskBtn.transform.localScale = Vector3.one;
        } 
        nextRealm = -1;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
    }

    public void SettingsGame()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
