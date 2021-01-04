using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;

public class GoogleManager : MonoBehaviour
{
    public Text LogText;


    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().RequestServerAuthCode(false).Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
    }


    public void LogIn()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success) LogText.text = Social.localUser.id + " \n " + Social.localUser.userName;
            else LogText.text = "구글 로그인 실패";
        });
    }


    public void LogOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
         LogText.text = "구글 로그아웃";
            
       
    }
}