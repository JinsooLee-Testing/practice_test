using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GoogleLoginManager : MonoBehaviour
{

    void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.Activate();
    }


    public void googleLogon()
    {
        Social.localUser.Authenticate(success => {
            if (success)
            {
                Debug.Log("Authentication successful");
                string userInfo = "Username: " + Social.localUser.userName +
                    "\nUser ID: " + Social.localUser.id +
                    "\nIsUnderage: " + Social.localUser.underage;
                Debug.Log(userInfo);
            }
            else
                Debug.Log("Authentication failed");
        });
    }


    public void OnLogOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
        Debug.Log("Logout");
    }
}

