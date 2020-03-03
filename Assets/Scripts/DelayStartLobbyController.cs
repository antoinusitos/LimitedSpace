using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DelayStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject  myOfflineButton;
    [SerializeField]
    private GameObject  myDelayStartButton;
    [SerializeField]
    private GameObject  myDelayCancelButton;
    [SerializeField]
    private int         myRoomSize;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        myDelayStartButton.SetActive(true);
        myOfflineButton.SetActive(false);
    }

    public void DelayStart()
    {
        GameInstance.GetInstance().SetPlayerName();
        myDelayStartButton.SetActive(false);
        myDelayCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Delay Start");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    private void CreateRoom()
    {
        Debug.Log("Creating room now");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)myRoomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOptions);
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a room... trying again !");
        CreateRoom();
    }

    public void DelayCancel()
    {
        myDelayStartButton.SetActive(true);
        myDelayCancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
}
