using UnityEngine;
using UdpKit;
using System;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;

public class NetworkManager : Photon.Bolt.GlobalEventListener
{
    [SerializeField]
    private UnityEngine.UI.Text feedback;
    [SerializeField]
    private UnityEngine.UI.InputField username;

    private void Awake()
    {
        username.text = AppManager.Current.Username;
    }

    public void FeedbackUser(string text)
    {
        feedback.text = text;
    }

    public void Connect()
    {
        if (username.text != "")
        {
            AppManager.Current.Username = username.text;
            BoltLauncher.StartClient();
            FeedbackUser("Connecting ...");
        }
        else
        {
            FeedbackUser("Please enter a name");
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        FeedbackUser("Searching....");
        BoltMatchmaking.JoinRandomSession();
        //BoltMatchmaking.JoinSession(HeadlessServerManager.RoomID());
        //Debug.Log("ran SessionListUpdated");
    }

    public override void Connected(BoltConnection connection)
    {
        FeedbackUser("Connected!");
        //Debug.Log("ran Connected");
    }
}
