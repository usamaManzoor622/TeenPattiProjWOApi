using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using WebSocketSharp;

public class Facebookauth : MonoBehaviour
{

    FirebaseAuth auth;

    public TMP_Text IdText;
    public TMP_Text NameText;

    public Image ProfileImage;

    [ShowOnly]
    public string userName;
    [ShowOnly]
    public string tokenId;
    [ShowOnly]
    public string EmailID;
    #region Creating Instance
    private static Facebookauth _instance;
    public static Facebookauth Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<Facebookauth>();
            return _instance;
        }
    }

    #endregion

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
       
    }
    private void InitCallBack()
    {
      //  if (!FB.IsInitialized)
        //{
        //    FB.ActivateApp();
        //}
        //else
        {
            IdText.text = "Failed to initialize";
            //Debug.Log("Failed to initialize");
        }
    }
    private void OnHideUnity(bool isgameshown)
    {
        if (!isgameshown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Facebook_Login()
    {
        LoginWithAllAuth.Instance.WaitPanel.SetActive(true);
        var permission = new List<string>() { "public_profile", "email" };
      //  FB.LogInWithReadPermissions(permission, AuthCallBack);
    }

    public void FaceBook_LogOut()
    {
       // FB.LogOut();
    }

    private void RetrieveFriendsList()
    {
        Debug.Log("Friends List Calling");
        // Call the FriendsCallBack method to retrieve the friends listvar permission = new List<string>() { "public_profile", "email" };
        var permission = new List<string>() { "user_friends", "email" };
        //FB.API("/me/friends", HttpMethod.GET, FriendsCallBack);
    }

    //private void FriendsCallBack(IGraphResult result)
    //{
    //    if (result.Error == null)
    //    {
    //        var friendsData = result.ResultDictionary["data"] as List<object>;
    //        Debug.Log(friendsData.Count);
    //        foreach (var friend in friendsData)
    //        {
    //            var friendData = friend as Dictionary<string, object>;
    //            var friendId = friendData["id"].ToString();
    //            var friendName = friendData["name"].ToString();
    //            Debug.Log("Friend ID: " + friendId + ", Name: " + friendName);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Failed to retrieve friends list: " + result.Error);
    //    }
    //}

    private void RetrieveProfilePicture()
    {
        var parameters = new Dictionary<string, string>()
    {
        { "type", "large" },
        { "redirect", "false" }
    };

        //FB.API("me/picture", HttpMethod.GET, ProfilePictureCallBack1, parameters);
    }

//    private void AuthCallBack(ILoginResult result)
//    {
//        if (FB.IsLoggedIn)
//        {
//            var aToken = AccessToken.CurrentAccessToken;
//            Debug.Log(AccessToken.CurrentAccessToken.ExpirationTime);
//            RetrieveProfilePicture();
//            IdText.text = (aToken.UserId);
//            //   LoginWithAllAuth.Instance.GetTokenIDAndOtherThing(3, aToken.UserId, "", "", null);
//            tokenId = aToken.UserId;
//            IdText.gameObject.SetActive(true);
//            Debug.Log(aToken.UserId);

//            string accesstoken;
//            string[] data;
//            string acc;
//            string[] some;
//            data = result.RawResult.Split(',');
//            for (int i = 0; i < data.Length; i++)
//            {
//                Debug.Log(i + "Data is " + data[i]);
//            }
//#if UNITY_EDITOR
//            //Debug.Log("this is raw access " + result.RawResult);
//            //DebugText.text = "this is raw access " + result.RawResult;
//            //data = result.RawResult.Split(',');
//            //Debug.Log("this is access" + data[3]);
//            //DebugText.text = "this is raw access Data3 " + data[3];
//            //acc = data[3];
//            //some = acc.Split('"');
//            //DebugText.text = "this is raw access some3 " + some[3];
//            //Debug.Log("this is access " + some[3]);
//            //accesstoken = some[3];
//#elif UNITY_ANDROID
//            //Debug.Log("this is raw access "+result.RawResult);
//            //DebugText.text = "this is overall raw access "+result.RawResult;
//            //data = result.RawResult.Split(',');
//            //Debug.Log("this is access"+data[0]);
//            //DebugText.text = "this is raw access data 0 " + data[0];
//            // acc = data[0];
//            // some = acc.Split('"');
//            //Debug.Log("this is access " + some[3]);
//            //DebugText.text = "this is raw access some 3 " + some[3];


//             //accesstoken = some[3];
//#endif
//            authwithfirebase(aToken.TokenString);
//            Invoke(nameof(RetrieveFriendsList), 2f);
//        }
//        else
//        {
//            Debug.Log("User Cancelled login");
//            LoginWithAllAuth.Instance.ShowError("User Cancelled login");
//        }
//    }
    Firebase.Auth.FirebaseUser newuserdata;
    public void authwithfirebase(string accesstoken)
    {
        auth = FirebaseAuth.DefaultInstance;
        Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accesstoken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("singin encountered error" + task.Exception);
                LoginWithAllAuth.Instance.ShowError("singin encountered error" + task.Exception);
            }
            Firebase.Auth.FirebaseUser newuser = task.Result;
            // string userName = newuser.DisplayName.ToString();
            // newuserdata = newuser;
            userName = newuser.DisplayName;
            EmailID = newuser.Email;
            // setname(newuserdata);
            //setname(task.Result);
            //LoginWithAllAuth.UserName = userName;
            //LoginWithAllAuth.Instance.GetTokenIDAndOtherThing(3, "", "", userName, null);
            //NameText.text = newuser.DisplayName.ToString();
            //NameText.gameObject.SetActive(true);

            //NameText.autoSizeTextContainer= true;
        });
    }
    bool isRepeated;
    public void setname(Firebase.Auth.FirebaseUser newuser)
    {
        if (newuser != null)
        {
            string nameis = newuser.DisplayName.ToString();
            // userName = nameis;
            EmailID = newuser.Email;
            //LoginWithAllAuth.Instance.GetTokenIDAndOtherThing(3, "", newuser.Email, nameis, null);
            callingName(newuser);
        }


    }


    void callingName(FirebaseUser newuser)
    {
        StartCoroutine(showName(newuser));
    }
    IEnumerator showName(FirebaseUser newuser)
    {
        yield return new WaitForSeconds(0.75f);
        Debug.LogError("Amjad Full name is:: " + newuser.DisplayName);
        string nameis = newuser.DisplayName.ToString();
        //  LoginWithAllAuth.Instance.GetTokenIDAndOtherThing(3, "", newuser.Email, nameis, null);
    }
    //private void ProfilePictureCallBack(IGraphResult result)
    //{
    //    if (result.Error == null && result.ResultDictionary != null)
    //    {
    //        Debug.Log("Download Result is " + result);
    //        Debug.Log("Profile Picture Download");

    //        if (result.ResultDictionary.TryGetValue("picture", out object pictureObject) && pictureObject is Dictionary<string, object> pictureData)
    //        {
    //            Debug.Log("Hereeee");
    //            if (pictureData.TryGetValue("data", out object dataObject) && dataObject is Dictionary<string, object> data)
    //            {
    //                if (data.TryGetValue("url", out object urlObject) && urlObject is string url)
    //                {
    //                    StartCoroutine(LoadProfilePicture(url));
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Failed to retrieve profile picture: " + result.Error);
    //    }
    //}

    //private void ProfilePictureCallBack1(IGraphResult result)
    //{
    //    if (result.Error == null && !string.IsNullOrEmpty(result.RawResult))
    //    {
    //        string rawResult = result.RawResult;
    //        string pattern = "\"url\":\"([^\"]+)\"";
    //        Match match = Regex.Match(rawResult, pattern);

    //        if (match.Success && match.Groups.Count > 1)
    //        {
    //            string url = match.Groups[1].Value;

    //            // Remove backslashes from the URL
    //            url = url.Replace("\\", "");

    //            // Validate the URL format
    //            if (Uri.TryCreate(url, UriKind.Absolute, out Uri profilePictureUri))
    //            {
    //                StartCoroutine(LoadProfilePicture(profilePictureUri.ToString()));
    //                // Use the retrieved URL
    //                Debug.Log("Profile Picture URL: " + url);
    //            }
    //            else
    //            {
    //                Debug.Log("Invalid profile picture URL: " + url);
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Failed to extract profile picture URL from RawResult.");
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Failed to retrieve profile picture: " + result.Error);
    //    }
    //}




    private IEnumerator LoadProfilePicture(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log("Calling Download Picture Data");
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Access the downloaded profile picture texture
                Texture2D profilePicture = DownloadHandlerTexture.GetContent(webRequest);

                // Do whatever you want with the profile picture
                // For example, you can assign it to a UI Image component
                // assuming you have a reference to the Image component in your script
                if (profilePicture != null)
                {
                    // Assuming you have a reference to the Image component in your script                    
                    if (ProfileImage != null)
                    {
                        ProfileImage.sprite = Sprite.Create(profilePicture, new Rect(0, 0, profilePicture.width, profilePicture.height), Vector2.zero);

                        ProfileImage.gameObject.SetActive(true);
                        //  setname(newuserdata);

                    }
                    yield return new WaitUntil(() => !userName.IsNullOrEmpty());
                    LoginWithAllAuth.Instance.GetTokenIDAndOtherThing(3, tokenId, EmailID, userName.ToString(), ProfileImage);
                }
            }
            else
            {
                Debug.Log("Failed to download profile picture: " + webRequest.error);
                LoginWithAllAuth.Instance.ShowError("Failed to download profile picture: " + webRequest.error);
            }
        }
    }
}
