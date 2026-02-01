using UnityEngine;

public class TransferData : MonoBehaviour
{
    public int masks_Unlocked;
    public int currentRealm, nextRealm, prevRealm;

    public static TransferData instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  
        }
    }
}
