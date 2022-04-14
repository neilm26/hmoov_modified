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
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        FeedbackUser("Searching....");
        BoltMatchmaking.JoinSession(HeadlessServerManager.RoomID());
    }

    public override void Connected(BoltConnection connection)
    {
        FeedbackUser("Connected!");
    }
}
