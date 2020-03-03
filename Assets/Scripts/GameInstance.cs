using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameInstance : MonoBehaviour
{
    private static GameInstance     myInstance = null;

    [SerializeField]
    private InputField              myNameInputField = null;

    private string                  myName = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        myInstance = this;
    }

    public static GameInstance GetInstance()
    {
        return myInstance;
    }

    public void SetPlayerName()
    {
        myName = myNameInputField.text;
        PhotonNetwork.LocalPlayer.NickName = myName;
    }
}
