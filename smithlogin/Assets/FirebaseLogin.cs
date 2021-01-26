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

    LoginState mLoginState;
    
    private bool signedIn = false;

    Firebase.Auth.FirebaseAuth auth = null;
    Firebase.Auth.FirebaseUser user = null;
    public GameObject mInitTrue;
    //public Image mInitFalse;

    // Start is called before the first frame update
    public void InittializeAccount()
    {
        if (auth.CurrentUser == null)
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
            mLoginState = LoginState.NoLogin;

        }
        else
        {
            user = auth.CurrentUser;
            Debug.Log("init_" + auth.CurrentUser.UserId);
            Debug.Log("Provider :" + user.ProviderData);

        }
    }
    void Start()
    {
        InittializeAccount();
    }
    public void LoginButton()
    {
        if (authSignIn() == true)
        {
            if ((mSignupPannel.activeSelf == false))
            {
                mSignupPannel.SetActive(true);

            }
        }
        else Debug.Log("Init을 안했거나 이미 로그인되어있습니다");
    
        
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null) 

            user = auth.CurrentUser;

            
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
    
            mSignupPannel.SetActive(false);
        
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

            if(task.IsCompleted)
            {
                task.Wait();
                mLoginState = LoginState.Email;
                LoginCloseButton();
            }
            
        });
         }

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
            
        }
    }

    // Guest Login
    public void GuestLoginButton()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("error");
            }

            Debug.Log("Login Ok" + auth.CurrentUser.UserId);
            mLoginState = LoginState.Guest;
            Debug.Log(mLoginState);
            mSignupPannel.SetActive(false);
            Debug.Log("1");
            /*if (task.IsCanceled)
            {
                // "SignInAnonymouslyAsync was canceled.";
                                return;
            }
            if (task.IsFaulted)
            {
               // "SignInAnonymouslyAsync encountered an error: " 
               
                return;
            }*/


        });

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
        if (auth == null)
        {
            Debug.Log("Not Setting");
        }
        user = auth.CurrentUser;
        switch(mLoginState)
        {
            case LoginState.NoLogin :

                break;
            case LoginState.Guest:
                 if (mSettingPannel.activeSelf == false)
                {
                    mSettingPannel.SetActive(true);
                }
                mAccount.text = user.Email;
                mUserID.text = user.UserId;
                break;
            case LoginState.Google:
                if (mSettingPannel.activeSelf == false)
                {
                    mSettingPannel.SetActive(true);
                }
                mAccount.text = user.Email;
                mUserID.text = user.UserId;
                break;
            case LoginState.Email:
                if (mSettingPannel.activeSelf == false)
                {
                    mSettingPannel.SetActive(true);
                }
                mAccount.text = user.Email;
                mUserID.text = user.UserId;
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
     
        auth.SignOut();
        mLoginState = LoginState.NoLogin;
        if (auth == null)
        {
            Debug.Log("auth == null");
        }
        else Debug.Log("Auth != null");
        auth = null;
    }
}
