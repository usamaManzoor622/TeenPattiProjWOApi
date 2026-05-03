
using UnityEngine;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;
using System;
using Firebase;
using Google;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using Firebase.Auth;
using Google;
using System.Net.Http;
using System.Collections;

public class GoogleAuth : MonoBehaviour
{
    public string googleWebApi = "638497931596-p9ifjk25ktrv11if4l9in5obsrvv4i6p.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    //  UI of Users
    public TMP_Text userNameTxt, UserEmailTxt, googleText;
    public Image userProfilePic;
    public string imageUrl;
    public GameObject LoginScreen, profileScreen;



    public void ShowGoogleMassage(string massage)
    {
        googleText.text = massage;
        }
    string userName, TokenID, EmailAddress;
    #region Creating Instance
    private static GoogleAuth _instance;
    public static GoogleAuth Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GoogleAuth>();
            return _instance;
        }
    }
    #endregion
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        configuration = new GoogleSignInConfiguration
        { WebClientId = googleWebApi, /*RequestEmail = true,*/ RequestIdToken = true };

    }

    private void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // On SignIn Button Click
    public void GoogleSinInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        ShowGoogleMassage("Autencation Start .......");
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
    }

    public void GoogleSigout()
    {
        GoogleSignIn.DefaultInstance.SignOut();
      
      
    }
    public void SignInWithGoogle() { GoogleSinInClick(); }


    void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Fault");
            ShowGoogleMassage("Fault .......");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Login Cancel");
            ShowGoogleMassage("Login Cancel .......");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    ShowGoogleMassage("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync ecountered as error: " + task.Exception);
                    ShowGoogleMassage("SignInWithCredentialAsync ecountered as error: " + task.Exception);
                    return;
                }


                user = auth.CurrentUser;

                userName = user.DisplayName;
                TokenID = user.UserId;
                EmailAddress = user.Email;

                userNameTxt.text = user.DisplayName;
                UserEmailTxt.text = user.UserId;

                ShowGoogleMassage("Authentication Sucessfully");
                LoginScreen.SetActive(false);
                profileScreen.SetActive(true);

                StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));

            });
        }
    }

    string CheckImageUrl(string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Debug.LogError("cehck Your url" + url);
            ShowGoogleMassage("cehck Your url" + url);
            return url;
        }
        Debug.LogError("cehck Your url" + imageUrl);
        ShowGoogleMassage("cehck Your url" + imageUrl);
        return imageUrl;
    }

    IEnumerator LoadImage(string imageUrl)
    {
        WWW wWW = new WWW(imageUrl);
        ShowGoogleMassage("wait for url of image");
        yield return wWW;

        userProfilePic.sprite = Sprite.Create(wWW.texture, new Rect(0, 0, wWW.texture.width, wWW.texture.height), new Vector2(0, 0));
        ShowGoogleMassage("picture implementation Successfully");
        LoginWithAllAuth.Instance.GetTokenIDAndOtherThing(2, TokenID, EmailAddress, userName, userProfilePic);
    }





}
