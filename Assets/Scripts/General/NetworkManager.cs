using UnityEngine;
using UdpKit;
using System;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;

public class NetworkManager : Photon.Bolt.GlobalEventListener
{
    [SerializeField]
    private UnityEngine.UI.Text feedback;

    public void FeedbackUser(string text)
    {
        feedback.text = text;
    }

    public void Connect()
    {
        FeedbackUser("Attempting to Connect....");
        BoltLauncher.StartClient();
        Debug.Log("ran Connect");
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        FeedbackUser("Searching....");
        BoltMatchmaking.JoinSession(HeadlessServerManager.RoomID());
        Debug.Log("ran SessionListUpdated");
    }

    public override void Connected(BoltConnection connection)
    {
        FeedbackUser("Connected!");
        Debug.Log("ran Connected");
    }
}
