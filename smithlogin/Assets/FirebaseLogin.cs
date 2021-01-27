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
    

    // 각각 팝업창 및 내부 인자 관리 
    public GameObject mSignupPanel;
    public GameObject mSettingPanel;
    public Image mLogPanel;
    public Text mLogText;
    public Text mAccount;
    public Text mUserID;

    // 각각 상태 제어 스테이트 
    enum LoginState : int {NoLogin,Google, Guest, Email};
    enum ButtonState : int { Initialize, Login, Logout }
    LoginState mLoginState;
    ButtonState mButtonState = ButtonState.Logout;    
    private bool signedIn = false;

    //파이어베이스 생성 
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    

    // Start is called before the first frame update
    public void InittializeAccount()
    {
        if (mButtonState == ButtonState.Logout)
        {
            mButtonState = ButtonState.Initialize;
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
            if(mLogPanel.gameObject.activeSelf == false)  mLogPanel.gameObject.SetActive(true);
            mLogPanel.color = Color.blue;
            mLogText.text = "Init";
        }
        else
        {
            if (mLogPanel.gameObject.activeSelf == false) mLogPanel.gameObject.SetActive(true);
            mLogPanel.color = Color.red;
            mLogText.text = "Init";
        }

    }

    void Start()
    {
        //InittializeAccount();
    }
    public void LoginButton()
    {
        if ((mSignupPanel.activeSelf == false) && (mButtonState == ButtonState.Initialize))
        {
            mLoginState = LoginState.NoLogin;
            mSignupPanel.SetActive(true);
        }
        else
        {
            if (mLogPanel.gameObject.activeSelf == false) mLogPanel.gameObject.SetActive(true);
            mLogPanel.color = Color.red;
            mLogText.text = "Login";
        }
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null) { }
            user = auth.CurrentUser;
        }
    }

    public void LoginCloseButton()
    {
        mSignupPanel.SetActive(false);
        if (mLoginState != LoginState.NoLogin)
        {
            if (mLogPanel.gameObject.activeSelf == false) mLogPanel.gameObject.SetActive(true);
            mLogPanel.color = Color.blue;
            mLogText.text = "Login";
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
        });
        if (auth != null)
        {
            mLoginState = LoginState.Email;
            mButtonState = ButtonState.Login;
            LoginCloseButton();
        }

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
            mButtonState = ButtonState.Login;
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
        });
        if (auth != null)
        {
            mLoginState = LoginState.Guest;
            mButtonState = ButtonState.Login;
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
        if(auth != null)
        {
            mLoginState = LoginState.Google;
            mButtonState = ButtonState.Login;
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
        if (mButtonState == ButtonState.Login)
        {
            user = auth.CurrentUser;
            if (mLogPanel.gameObject.activeSelf == false) mLogPanel.gameObject.SetActive(true);
            mLogPanel.color = Color.blue;
            mLogText.text = "Setting";
            switch (mLoginState)
            {
                case LoginState.NoLogin:

                    break;
                case LoginState.Guest:
                    if (mSettingPanel.activeSelf == false)
                    {
                        mSettingPanel.SetActive(true);
                    }
                    mAccount.text = " -(Guest) ";
                    mUserID.text = user.UserId;
                    break;
                case LoginState.Google:
                    if (mSettingPanel.activeSelf == false)
                    {
                        mSettingPanel.SetActive(true);
                    }
                    mAccount.text = " -(Google) ";
                    mUserID.text = user.UserId;
                    break;
                case LoginState.Email:
                    if (mSettingPanel.activeSelf == false)
                    {
                        mSettingPanel.SetActive(true);
                    }
                    mAccount.text = user.Email;
                    mUserID.text = user.UserId;
                    break;
                default:
                    mLogText.text = "exceptional error";
                    break;
            }
        }
        else
        {
            mLogPanel.color = Color.red;
            mLogText.text = "Setting";
        }
    }

    public void SettingCloseButton()
    {
        if (mSettingPanel.activeSelf == true)
        {
            mSettingPanel.SetActive(false);
        }
    }

    public void LogOut()
    {   
        if(auth != null)  auth.SignOut();

        if (mButtonState == ButtonState.Login)
        {
            if (mLogPanel.gameObject.activeSelf == false) mLogPanel.gameObject.SetActive(true);
            mLogPanel.color = Color.blue;
            mLogText.text = "Logout";
            mLoginState = LoginState.NoLogin;
            mButtonState = ButtonState.Logout;
        }
        else
        {
            if (mLogPanel.gameObject.activeSelf == false) mLogPanel.gameObject.SetActive(true);
            mLogPanel.color = Color.red;
            mLogText.text = "Logout";
        }
        

    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
