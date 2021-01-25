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
    public GameObject mSettingPannel;
    public Text mAccount;
    public Text mUserID;
    enum LoginState : int {NoLogin,Google, Guest, Email};
    LoginState mLoginState = 0;


    
    private bool signedIn = false;

    Firebase.Auth.FirebaseAuth auth = null;
    Firebase.Auth.FirebaseUser user = null;
    public Text Logtext;
    // Start is called before the first frame update
    public void InittializeAccount()
    {
        //auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        Logtext.text = "Init";
        mLoginState = 0;
    }
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        
       

    }
    public void LoginButton()
    {
        if (authSignIn() == true)
        {
            if ((mSignupPannel.activeSelf == false))
            {
                mSignupPannel.SetActive(true);
            }
        }else
        {
            Logtext.text = "이미 로그인 되어 있습니다";
        }
    

    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null) Logtext.text = user.UserId;

            user = auth.CurrentUser;

            if(signedIn) Logtext.text = user.UserId;
        }
    }



    bool authSignIn()
    {
        if (auth.CurrentUser == null)
        {
            return true;
        }
        else return false;
       
        
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

            Logtext.text = ("Email Login");
            
        });
        if (auth != null)
        {
            mLoginState = LoginState.Email;
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
        });
        if (auth != null)
        {
            mLoginState = LoginState.Email;
            LoginCloseButton();
            Logtext.text = "이메일 회원가입 완료";
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
            mLoginState = LoginState.Guest;
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
            mLoginState = LoginState.Google;
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
        });
    }


    public void SettingButon()
    {

       
        switch(mLoginState)
        {
            case LoginState.NoLogin :

                break;
            case LoginState.Guest:
                 if (mSettingPannel.activeSelf == false)
                {
                    mSettingPannel.SetActive(true);
                }
                mAccount.text = auth.CurrentUser.Email;
                mUserID.text = auth.CurrentUser.UserId;
                break;
            case LoginState.Google:
                if (mSettingPannel.activeSelf == false)
                {
                    mSettingPannel.SetActive(true);
                }
                mAccount.text = auth.CurrentUser.Email;
                mUserID.text = auth.CurrentUser.UserId;
                break;
            case LoginState.Email:
                if (mSettingPannel.activeSelf == false)
                {
                    mSettingPannel.SetActive(true);
                }
                mAccount.text = auth.CurrentUser.Email;
                mUserID.text = auth.CurrentUser.UserId;
                break;
            default:
                Debug.Log("exceptional error");
                break;
        }         


    }

    public void SettingCloseButton()
    {
        if (mSettingPannel.activeSelf == true)
        {

            mSettingPannel.SetActive(false);
        }
    }

    public void LogOut()
    {
        if (mLoginState != LoginState.NoLogin)
        {
            mLoginState = LoginState.NoLogin;
            
            Logtext.text = "logout";
        }
        auth.SignOut();
    }
}
