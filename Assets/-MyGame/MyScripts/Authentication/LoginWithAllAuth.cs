using com.mani.muzamil.amjad;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class LoginWithAllAuth : MonoBehaviour
{
    public TMP_Text ProgressTxt;

    public GameObject AuthPanel;
    public GameObject WaitPanel;
    public GameObject fBBtnOfPlayerInfo;
    public TMP_Text AllInfo;
    public Image Picc;

    public static int AuthType = 0;
    public static string TokenID;
    public static string DeviceID;
    public static string EmailID;
    public static string UserName;
    public static string Status;
    public static Image ProfilePic;

    #region Creating Instance
    private static LoginWithAllAuth _instance;
    public static LoginWithAllAuth Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<LoginWithAllAuth>();
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        AuthType = 0;
        TokenID = "";
        DeviceID = "";
        EmailID = "";
        UserName = "";
        Status = "";
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //WaitPanel.SetActive(false);

        AuthType = LocalSettings.GetAuthType();
        //AuthType = 0;
        if (AuthType == 0)
        {
            AuthPanel.SetActive(true);
            fBBtnOfPlayerInfo.SetActive(true);
        }
        else
        {
            fBBtnOfPlayerInfo.SetActive(false);
            AuthPanel.SetActive(false);
           
        }

    }


    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private string GenerateRandomString(int length)
    {
        string randomString = "";

        for (int i = 0; i < length; i++)
        {
            // Generate a random index to pick a character from the Characters string
            int randomIndex = Random.Range(0, Characters.Length);
            randomString += Characters[randomIndex];
        }

        return randomString;
    }

    public void GetTokenIDAndOtherThing(int AuthTypeScr, string TokenIdFromAuth, string EmailIdSrc, string UserNameSrc, Image UserProfilePicSrc)
    {
        ProgressTxt.text = "Authenticating User...";
        GoogleAuth.Instance.ShowGoogleMassage("Authenticating User...");
        if (UserNameSrc != "" && UserNameSrc != null)
            UserName = UserNameSrc;
        AuthType = AuthTypeScr;
        if (TokenIdFromAuth != "")
        {
            TokenID = TokenIdFromAuth;
            LocalSettings.SetTokenID(TokenID);
        }
        //else if (AuthTypeScr == 1)
        //{
        //    TokenID = GenerateRandomString(Random.Range(10, 15));
        //    LocalSettings.SetTokenID(TokenID);
        //}

        //DeviceID = RestAPI.Instance.MyDeviceId;
        DeviceID = (AuthTypeScr == 1) ? SystemInfo.deviceUniqueIdentifier.ToString() : "";
        Debug.LogError("Amjad first:___________________________________ " + UserName + "     Src name: " + UserNameSrc);
        switch (AuthType)
        {
            case 1:
                // Guest User
                Status = "guest";
                AuthenticateUser();
                break;
            case 2:
                // Google User
                Status = "player";
                if (EmailIdSrc != "")
                    EmailID = EmailIdSrc;
                if (UserNameSrc != "")
                    UserName = UserNameSrc;
                if (UserProfilePicSrc != null)
                {

                    ProfilePic = UserProfilePicSrc;
                }

                GoogleAuth.Instance.ShowGoogleMassage("bhai sb");
                AuthenticateUser();
                break;
            case 3:
                // Facebook User
                Debug.LogError("<color=cayan>Amjad 2nd:___________________________________ " + UserName + "     Src name: " + UserNameSrc + "</Color>");
                Status = "player";
                if (UserNameSrc != "")
                {
                    UserName = UserNameSrc;
                }
                if (UserProfilePicSrc)
                    ProfilePic = UserProfilePicSrc;
                if (EmailIdSrc != "")
                    EmailID = EmailIdSrc;
                AuthenticateUser();
                break;
        }


    }


    void AuthenticateUser()
    {
        switch (AuthType)
        {
            case 1:
                // Guest 
                GetDataFromRestAPI();
                break;
            case 2:
                // Google
                GoogleAuth.Instance.ShowGoogleMassage("after calling Google auth ");
                GetDataFromRestAPI();
                break;
            case 3:
                // Facebook
                if (TokenID != "" && UserName != "" && ProfilePic.sprite != null)
                {
                    GetDataFromRestAPI();

                    Debug.LogError("<color=green>Facebook Authenticated</color>");
                }
                else
                {
                    string picname = "null";
                    if (ProfilePic)
                        picname = ProfilePic.name;
                    Debug.LogError("<color=red>missing: tokenid: " + TokenID + "    User name: " + UserName + "     PIc: " + picname + "</ color > ");
                }
                break;
        }
        if (AuthType != 0)
            LocalSettings.SetAuthType(AuthType);
    }

    public void GetDataFromRestAPI()
    {
        Debug.LogError("<color=yellow>API Called ____________________________</color>");
        GoogleAuth.Instance.ShowGoogleMassage("<color=yellow>API Called ____________________________</color>");
        if (ProfilePic != null)
            Picc.sprite = ProfilePic.sprite;
        else
            Picc.sprite = UpdateAvatar.Instance.AvatorSpritesSquare.Sprites[0].Sprites;
        Image ReadyProfileImage = Picc;

        AuthPanel.SetActive(false);
        WaitPanel.SetActive(false);
    }
    public void ShowError(string errorMsg)
    {
        AllInfo.text = errorMsg;
        WaitPanel.SetActive(false);
    }
}
