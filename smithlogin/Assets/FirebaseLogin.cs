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
           
            

        });
    }

    public void SettingButon()
    {
        user = auth.CurrentUser;
        if (user != null)
        {
            foreach (var profile in user.ProviderData)
            {
                // Id of the provider (ex: google.com)
                string providerId = profile.ProviderId;

                // UID specific to the provider
                string uid = profile.UserId;

                // Name, email address, and profile photo Url
                string name = profile.DisplayName;
                string email = profile.Email;
                System.Uri photoUrl = profile.PhotoUrl;
            }
        }
    }

    public void LogOut()
    {
        auth.SignOut();
        Logtext.text = "logout";
        
    }
}
