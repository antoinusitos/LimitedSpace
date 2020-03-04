using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager     myInstance = null;

    [SerializeField]
    private GameObject              myEndSoundObject = null;

    private void Awake()
    {
        myInstance = this;
    }

    public static SoundManager GetInstance()
    {
        return myInstance;
    }

    public void PlayEndSound()
    {
        myEndSoundObject.SetActive(true);
    }
}
