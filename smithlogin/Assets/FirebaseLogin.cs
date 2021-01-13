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
    public GameObject mSignupPannel;
    bool LoginState = false;

    Firebase.Auth.FirebaseAuth auth = null;
    public Text Logtext;
    // Start is called before the first frame update
    public void InittializeAccount()
    {
        //auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }
    public void LoginButton()
    {
        if (mSignupPannel.activeSelf == false)
        {
            mSignupPannel.SetActive(true);
        }
    }

    public void LoginCloseButton()
    {
        
        if (mSignupPannel.activeSelf == true)
        {
            
            mSignupPannel.SetActive(false);
        }
    }

    public void EmailLogin()
    {
        auth.SignInWithEmailAndPasswordAsync(mEmailInputField.text, mPasswordInputField.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            Logtext.text = ("Email Login");

        });
        if (auth != null)
        {
            LoginCloseButton();

        } }

        public void EmailSignUp()
    {
        
        auth.CreateUserWithEmailAndPasswordAsync(mEmailInputField.text, mPasswordInputField.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            Logtext.text = ("회원가입 완료");
            LoginCloseButton();
        });
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

        });
        if (auth != null)
        {
            LoginCloseButton();
            
        }
    }

    public void GoogleLoginBtnOnClick()
    {
        GooglePlayServiceInitialize();

        Social.localUser.Authenticate(success =>
        {
            if (success == false) return;

            StartCoroutine(coLogin());
        });
        if (auth != null)
        {
            LoginCloseButton();


        }
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
        LoginState = false;
    }
}
