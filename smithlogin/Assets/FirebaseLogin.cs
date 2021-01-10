using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
public class FirebaseLogin : MonoBehaviour
{
    // 이메일 InputField
    [SerializeField]
    InputField mEmailInputField;
    // 비밀번호 InputField
    [SerializeField]
    InputField mPasswordInputField;
    // 결과를 알려줄 텍스트

    Firebase.Auth.FirebaseAuth auth = null;
    public Text Logtext;
    // Start is called before the first frame update
    public void InittializeAccount()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // E-mail Signup    
    public void SignUp()
    {
        // 회원가입 버튼은 인풋 필드가 비어있지 않을 때 작동한다.
        if (mEmailInputField.text.Length != 0 && mPasswordInputField.text.Length != 0)
        {
            auth.CreateUserWithEmailAndPasswordAsync(mEmailInputField.text, mPasswordInputField.text).ContinueWith(
                task =>
                {
                    if (!task.IsCanceled && !task.IsFaulted)
                    {
                        Logtext.text = "회원가입 성공";
                    }
                    else
                    {
                        Logtext.text = "회원가입 실패";
                    }
                });
        }
    }

    // E-mail Sign in
    public void SignIn()
    {
        // 로그인 버튼은 인풋 필드가 비어있지 않을 때 작동한다.
        if (mEmailInputField.text.Length != 0 && mPasswordInputField.text.Length != 0)
        {
            auth.SignInWithEmailAndPasswordAsync(mEmailInputField.text, mPasswordInputField.text).ContinueWith(
                task =>
                {
                    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        Firebase.Auth.FirebaseUser newUser = task.Result;
                        Logtext.text = "로그인 성공";
                    }
                    else
                    {
                        Logtext.text = "로그인 실패";
                    }
                });
        }
    }
        // Guest Login
        public void GuestLoginButton()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Logtext.text = "SignInAnonymouslyAsync was canceled.";
                                return;
            }
            if (task.IsFaulted)
            {
                Logtext.text = "SignInAnonymouslyAsync encountered an error: " + task.Exception;
               
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Logtext.text = newUser.DisplayName + newUser.UserId;
            
        });
    }

    public void GoogleLoginBtnOnClick()
    {
        GooglePlayServiceInitialize();

        Social.localUser.Authenticate(success =>
        {
            if (success == false) return;

            StartCoroutine(coLogin());
        });

    }

    void GooglePlayServiceInitialize()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    IEnumerator coLogin()
    {
        while (System.String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;

        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        string accessToken = null;



        Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Logtext.text= newUser.DisplayName+ newUser.UserId;
            

        });
    }

    public void LogOut()
    {
        auth.SignOut();
        Logtext.text = "logout";
    }
}
