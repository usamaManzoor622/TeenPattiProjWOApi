using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace com.mani.muzamil.amjad
{
    public class Menu_Manager : MonoBehaviourPunCallbacks
    {

        private static Menu_Manager instance;
        public static Menu_Manager Instance
        {
            get
            {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<Menu_Manager>();
                return instance;
            }
        }
        public Collections teenPattiVariationSprites;

        public GameObject shopPanel;

        public Image variationNameSprite;
        public GameObject[] lightArrow;
        public GameObject[] DTVRPanel;
        public GameObject[] BackPackBtn;
        public GameObject[] STCPanel;

        public GameObject tPVariationsMode;
        public GameObject selectModeClassicMufflisHumk;
        public GameObject selectModeAb;
        public GameObject selectModeWL;
        public GameObject selectModeLw;
        public GameObject selectModeDL;
        public GameObject SelectModePoker;

        public TMP_Text[] TotalChipsTxt;
        public TMP_InputField SetChips;
        public GameObject Namesaver;

        // For TeenPattiBetMode
        public GameObject teenPattiBetMode;
        [ShowOnly]

        public List<Button> TeenPattiModeMiniBetAmountBtn;
        BigInteger x100;
        BigInteger x1000;
        BigInteger x10000;
        BigInteger x50000;
        BigInteger x100000;
        BigInteger x500000;
        BigInteger x1000000;
        // For TeenPattiBetMode
        public GameObject pokerBetMode;
        [ShowOnly]
        public List<Button> pokerMiniBetAmountBtn;
        BigInteger maxP500;
        BigInteger maxP5000;
        BigInteger maxP20000;
        BigInteger maxP100000;
        BigInteger maxP500000;
        BigInteger maxP1000000;
        BigInteger maxP2000000;
        BigInteger maxP10000000;
        BigInteger maxP20000000;

        BigInteger minP250;
        BigInteger minP2500;
        BigInteger minP10000;
        BigInteger minP50000;
        BigInteger minP100000;
        BigInteger minP250000;
        BigInteger minP500000;

        // Room Summary For Win or Loss
        public GameObject Silverpanel;
        public GameObject Goldenpanel;
        public TMP_Text winAmountText;
        public TMP_Text loseAmountText;
        public TMP_Text loseInGame;
        public TMP_Text timerText;
        public GameObject playerInfoPanel;
        public GameObject quitePanel;


        // Player Detail 
        public TMP_Text PlayerName;
        public TMP_Text PlayerID;
        public TMP_Text TotalChips;
        public TMP_Text TotalXP;


        public GameObject goldCollectedSuccessfullyPanel;
        public TMP_Text goldCollectedTxt;
        public Texture2D ProfileImageTexture;

        public DailyReward dailyReward;
        private void Awake()
        {
            instance = this;
            if (!PlayerPrefs.HasKey(LocalSettings.TotalChips))
            {
                LocalSettings.SetTotalChips(100000);
                //FindObjectOfType<GetAPICash>().UpdateThisText(LocalSettings.GetTotalChips());
                PhotonNetwork.LocalPlayer.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
            }

        }


        // Start is called before the first frame update
        void Start()
        {

            UpDateTotalChipsTxt();
            //if (!PlayerPrefs.HasKey("name"))
            //    Namesaver.SetActive(true);

            HandleBGMusic();
            GetListFromTeenPattiModeMiniBetAmountBtn();
            SilverOrGoldenPanelActive();

            Invoke(nameof(ClearBuyInAmount), 3f);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void ClearBuyInAmount()
        {
            if (LocalSettings.GetPokerBuyInChips() > 0)
            {
                BigInteger amount = LocalSettings.GetPokerBuyInChips();

                LocalSettings.SetPokerBuyInChips(-amount);
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape) && ModePanel())
            {
                quitePanel.SetActive(true);
            }
        }

        void SilverOrGoldenPanelActive()
        {
            if (LocalSettings.isSwitchRoom)
                return;
            string WinOrLoseString = LocalSettings.winOrLoseAmount.ToString();
            timerText.text = LocalSettings.TimeCount(LocalSettings.GamePlayTimeCount);
            if (LocalSettings.winOrLoseAmount > 0)
            {

                winAmountText.text = "+" + LocalSettings.Rs(WinOrLoseString);
                Goldenpanel.SetActive(true);
                timerText.transform.parent.gameObject.SetActive(true);

                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.MainMenuGoldWin, false);
            }
            else if (LocalSettings.winOrLoseAmount < 0)
            {
                string totalAmount = WinOrLoseString;
                string aa = "-";
                WinOrLoseString = totalAmount.Replace(aa, "");
                loseAmountText.text = "-" + LocalSettings.Rs(WinOrLoseString);
                loseInGame.text = MatchHandler.IsTeenPatti() ? "Oops, you have lost " + LocalSettings.Rs(WinOrLoseString) + " in " + "Teen patti " + MatchHandler.CurrentMatch + " game try again or return \nto the main menu to continue your adventure;"
                    : "Oops, you have lost " + LocalSettings.Rs(WinOrLoseString) + " in " + MatchHandler.CurrentMatch + " game try again or return \nto the main menu to continue your adventure;";
                timerText.transform.parent.gameObject.SetActive(true);
                Silverpanel.SetActive(true);

                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.MainMenuGoldLoose, false);
            }
            else
            {
                timerText.transform.parent.gameObject.SetActive(false);
                Silverpanel.SetActive(false);
                Goldenpanel.SetActive(false);
            }

            //  Debug.LogError("Win or Lose Amont  " + LocalSettings.winOrLoseAmount);
            LocalSettings.GamePlayTimeCount = 0;
            LocalSettings.winOrLoseAmount = 0;
        }





        void GetListFromTeenPattiModeMiniBetAmountBtn()
        {
            // For Teen Patti Section
            for (int i = 0; i < teenPattiBetMode.transform.childCount; i++)
            {
                TeenPattiModeMiniBetAmountBtn.Add(teenPattiBetMode.transform.GetChild(i).GetComponent<Button>());
                // teenPattiBetMode.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
            x100 = LocalSettings.TeenPattiLevelMultiplier * 100;
            x1000 = LocalSettings.TeenPattiLevelMultiplier * 1000;
            x10000 = LocalSettings.TeenPattiLevelMultiplier * 10000;
            x50000 = new BigInteger(LocalSettings.TeenPattiLevelMultiplier) * 50000;
            x100000 = new BigInteger(LocalSettings.TeenPattiLevelMultiplier) * 100000;
            x500000 = new BigInteger(LocalSettings.TeenPattiLevelMultiplier) * 500000;
            x1000000 = new BigInteger(LocalSettings.TeenPattiLevelMultiplier) * 1000000;

            // For Poker Section
            for (int i = 0; i < pokerBetMode.transform.childCount; i++)
            {
                pokerMiniBetAmountBtn.Add(pokerBetMode.transform.GetChild(i).GetComponent<Button>());
                // teenPattiBetMode.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
            maxP500 = LocalSettings.PokerMultiplayerMax * 500;
            maxP5000 = LocalSettings.PokerMultiplayerMax * 5000;
            maxP20000 = LocalSettings.PokerMultiplayerMax * 20000;
            maxP100000 = new BigInteger(LocalSettings.PokerMultiplayerMax) * 100000;
            maxP500000 = new BigInteger(LocalSettings.PokerMultiplayerMax) * 500000;
            maxP1000000 = new BigInteger(LocalSettings.PokerMultiplayerMax) * 1000000;
            maxP2000000 = 50000000;
            maxP10000000 = 250000000;
            maxP20000000 = 500000000;

            minP250 = 0;
            minP2500 = LocalSettings.PokerMultiplayermin * 2500;
            minP10000 = LocalSettings.PokerMultiplayermin * 10000;
            minP50000 = LocalSettings.PokerMultiplayermin * 50000;
            minP250000 = LocalSettings.PokerMultiplayermin * 250000;
            minP500000 = new BigInteger(LocalSettings.PokerMultiplayermin) * 500000;



            // p2000000 = new BigInteger(LocalSettings.PokerMultiplayerMax) * 2000000;
            // p10000000 = new BigInteger(LocalSettings.PokerMultiplayerMax) * 10000000;
            // p20000000 = new BigInteger(LocalSettings.PokerMultiplayerMax) * 20000000;

            //  Debug.LogError(LocalSettings.Rs(minP250) + " " + LocalSettings.Rs(minP2500) + " " + LocalSettings.Rs(minP10000) + " " + LocalSettings.Rs(minP50000) + " " + LocalSettings.Rs(minP250000) + " " + LocalSettings.Rs(maxP20000000));

        }

        AudioSource BGMusicAS;
        public void HandleBGMusic()
        {
            if (BGMusicAS)
            {
                BGMusicAS.Stop();
                BGMusicAS = null;
            }
            //if (LocalSettings.GetSoundEffect())
            //    BGMusicAS = SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.BGMusic, true, true);
            //else if (BGMusicAS)
            //{
            //    BGMusicAS.Stop();
            //    BGMusicAS = null;
            //}

        }

        public void SelectTable(int EnumNumber)
        {
            MatchHandler.CurrentMatch = (MatchHandler.MATCH)EnumNumber;
            // Instance.currentRoomFilter = (RoomFilters)EnumNumber;



            //if (MatchHandler.IsTeenPatti())            
            //    // StartCoroutine(WaitForLoadRoom(EnumNumber));
            if (MatchHandler.IsTeenPatti())
                teenPattiMode();
            else if (MatchHandler.IsAndarBahar())
                ActiveMyPanel(selectModeAb.name);
            else if (MatchHandler.isWingoLottary())
                ActiveMyPanel(selectModeWL.name);
            else if (MatchHandler.IsLuckyWar())
                ActiveMyPanel(selectModeLw.name);
            else if (MatchHandler.isDragonTiger())
                ActiveMyPanel(selectModeDL.name);
            else if (MatchHandler.IsPoker())
                CheckTotalChipsToEnterPoker();
            //   StartCoroutine(WaitForLoadRoom(EnumNumber));

            //StartCoroutine(WaitForLoadRoom(EnumNumber));


        }

        void teenPattiMode()
        {

            switch (MatchHandler.CurrentMatch)
            {
                case MatchHandler.MATCH.Classic:
                    variationNameSprite.sprite = teenPattiVariationSprites.Sprites[0];
                    break;
                case MatchHandler.MATCH.Mufflis:
                    variationNameSprite.sprite = teenPattiVariationSprites.Sprites[1];
                    break;
                case MatchHandler.MATCH.HUukm:
                    variationNameSprite.sprite = teenPattiVariationSprites.Sprites[2];
                    break;
            }
            CheckTotalChipsToEnter3pattiRoom();
        }

        void ActiveMyPanel(string panelName)
        {
            selectModeClassicMufflisHumk.SetActive(panelName.Equals(selectModeClassicMufflisHumk.name));
            selectModeAb.SetActive(panelName.Equals(selectModeAb.name));
            selectModeWL.SetActive(panelName.Equals(selectModeWL.name));
            selectModeLw.SetActive(panelName.Equals(selectModeLw.name));
            selectModeDL.SetActive(panelName.Equals(selectModeDL.name));
            SelectModePoker.SetActive(panelName.Equals(SelectModePoker.name));

        }

        public void LoadAndStartGame(string EnumNumber)
        {
            if (LocalSettings.isPasswordRequired && !LocalSettings.IsPasswordChecked)
            {
               
                return;
            }
            BigInteger betAmount = LocalSettings.StringToBigInteger(EnumNumber);
            LocalSettings.Poker_Max_Entry_Fee = SetPokerMaxEntryFee(betAmount);
            //  Debug.LogError("Entry Fee...." + LocalSettings.Rs(LocalSettings.Poker_Max_Entry_Fee));
            StartCoroutine(WaitForLoadRoom(betAmount));
        }


        BigInteger SetPokerMaxEntryFee(BigInteger Fee)
        {
            BigInteger pokerFee = 0;
            string minFee = "";
            string feeString = Fee.ToString();
            switch (feeString)
            {
                case "500":
                    pokerFee = maxP500;
                    minFee = LocalSettings.Rs(minP250);
                    break;
                case "5000":
                    pokerFee = maxP5000;
                    minFee = LocalSettings.Rs(minP2500);
                    break;
                case "20000":
                    pokerFee = maxP20000;
                    minFee = LocalSettings.Rs(minP10000);
                    break;
                case "100000":
                    pokerFee = maxP100000;
                    minFee = LocalSettings.Rs(minP500000);
                    break;
                case "500000":
                    pokerFee = maxP500000;
                    minFee = LocalSettings.Rs(minP250000);
                    break;
                case "1000000":
                    pokerFee = maxP1000000;
                    minFee = LocalSettings.Rs(minP500000);
                    break;
                case "2000000":
                    pokerFee = new BigInteger(LocalSettings.PokerMultiplayerMax) * 2000000;
                    LocalSettings.pokerEntryFeeString = LocalSettings.Rs(maxP2000000) + "++";
                    break;
                case "10000000":
                    pokerFee = new BigInteger(LocalSettings.PokerMultiplayerMax) * 10000000;
                    LocalSettings.pokerEntryFeeString = LocalSettings.Rs(maxP10000000) + "++";
                    break;
                case "20000000":
                    pokerFee = new BigInteger(LocalSettings.PokerMultiplayerMax) * 20000000;
                    LocalSettings.pokerEntryFeeString = LocalSettings.Rs(maxP20000000) + "++";
                    break;

            }
            //  Debug.LogError("Check Here" + LocalSettings.pokerEntryFeeString);

            //if (Fee == 500)
            //    return pokerFee = maxP500;
            //if (Fee == 5000)
            //    return pokerFee = maxP5000;
            //if (Fee == 20000)
            //    return pokerFee = maxP20000;
            //if (Fee == 100000)
            //    return pokerFee = maxP100000;
            //if (Fee == 500000)
            //    return pokerFee = maxP500000;
            //if (Fee == 1000000)
            //    return pokerFee = maxP1000000;
            //if (Fee == 2000000)
            //    return pokerFee = new BigInteger(LocalSettings.PokerMultiplayerMax) * 2000000;
            //if (Fee == 10000000)
            //    return pokerFee = new BigInteger(LocalSettings.PokerMultiplayerMax) * 10000000;
            //if (Fee == 20000000)
            //    return pokerFee = new BigInteger(LocalSettings.PokerMultiplayerMax) * 20000000;
            if (Fee <= 1000000)
                LocalSettings.pokerEntryFeeString = minFee + "-" + LocalSettings.Rs(pokerFee);
            return pokerFee;
        }

        IEnumerator WaitForLoadRoom(BigInteger EnumNumber)
        {
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
            NetworkSettings.Instance.JoinRandomRoom(EnumNumber);
        }

        void CheckTotalChipsToEnter3pattiRoom()
        {
            foreach (Button item in TeenPattiModeMiniBetAmountBtn)
            {
                item.interactable = false;
            }

            if (LocalSettings.GetTotalChips() >= 100 && LocalSettings.GetTotalChips() <= x100)
                TeenPattiModeMiniBetAmountBtn[0].interactable = true;
            if (LocalSettings.GetTotalChips() >= (x100 / 2) && LocalSettings.GetTotalChips() <= x1000)
                TeenPattiModeMiniBetAmountBtn[1].interactable = true;
            if (LocalSettings.GetTotalChips() >= (x1000 / 2) && LocalSettings.GetTotalChips() <= x10000)
                TeenPattiModeMiniBetAmountBtn[2].interactable = true;
            if (LocalSettings.GetTotalChips() >= (x10000 / 2))
                TeenPattiModeMiniBetAmountBtn[3].interactable = true;
            if (LocalSettings.GetTotalChips() >= (x50000 / 2))
                TeenPattiModeMiniBetAmountBtn[4].interactable = true;
            if (LocalSettings.GetTotalChips() >= (x100000 / 2))
                TeenPattiModeMiniBetAmountBtn[5].interactable = true;
            if (LocalSettings.GetTotalChips() >= (x500000 / 2))
                TeenPattiModeMiniBetAmountBtn[6].interactable = true;
            if (LocalSettings.GetTotalChips() >= (x100000 / 2))
                TeenPattiModeMiniBetAmountBtn[7].interactable = true;

            if (LocalSettings.GetTotalChips() < 100)
                shopPanel.SetActive(true);
            else
                ActiveMyPanel(selectModeClassicMufflisHumk.name);

        }

        void CheckTotalChipsToEnterPoker()
        {
            foreach (Button item in pokerMiniBetAmountBtn)
            {
                item.interactable = false;
            }

            if (LocalSettings.GetTotalChips() >= minP250 && LocalSettings.GetTotalChips() <= maxP500)
                pokerMiniBetAmountBtn[0].interactable = true;
            if (LocalSettings.GetTotalChips() >= minP2500 && LocalSettings.GetTotalChips() <= maxP5000)
                pokerMiniBetAmountBtn[1].interactable = true;
            if (LocalSettings.GetTotalChips() >= minP10000 && LocalSettings.GetTotalChips() <= maxP20000)
                pokerMiniBetAmountBtn[2].interactable = true;
            if (LocalSettings.GetTotalChips() >= minP250000 && LocalSettings.GetTotalChips() <= maxP500000)
                pokerMiniBetAmountBtn[3].interactable = true;
            if (LocalSettings.GetTotalChips() >= minP500000 && LocalSettings.GetTotalChips() <= maxP1000000)
                pokerMiniBetAmountBtn[4].interactable = true;
            if (LocalSettings.GetTotalChips() >= minP250000 && LocalSettings.GetTotalChips() <= maxP1000000)
                pokerMiniBetAmountBtn[5].interactable = true;
            if (LocalSettings.GetTotalChips() >= maxP2000000)
                pokerMiniBetAmountBtn[6].interactable = true;
            if (LocalSettings.GetTotalChips() >= maxP10000000)
                pokerMiniBetAmountBtn[7].interactable = true;
            if (LocalSettings.GetTotalChips() >= maxP20000000)
                pokerMiniBetAmountBtn[8].interactable = true;



            if (LocalSettings.GetTotalChips() < 500)
                shopPanel.SetActive(true);
            else
                ActiveMyPanel(SelectModePoker.name);

        }


        public void DTVRBtn(int i)
        {
            foreach (GameObject item in DTVRPanel)
            {
                item.SetActive(false);
            }
            foreach (GameObject item in lightArrow)
            {
                item.SetActive(false);
            }
            foreach (GameObject item in BackPackBtn)
            {
                item.SetActive(false);
            }
            foreach (GameObject item in BackPackBtn)
            {
                item.SetActive(false);
            }
            foreach (GameObject item in STCPanel)
            {
                item.SetActive(false);
            }

            if (i < 3)
            {
                STCPanel[i].SetActive(true);
                BackPackBtn[i].SetActive(true);
                DTVRPanel[i].SetActive(true);
                lightArrow[i].SetActive(true);
            }
        }
        public void UpDateTotalChipsTxt()
        {
            foreach (TMP_Text chipsTxt in TotalChipsTxt)
                chipsTxt.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
        }
        public void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GetCashAndSaveOnServer()
        {
            BigInteger chipsToAdd = 0;
            string amount = SetChips.text;
            if (amount != "")
            {
                chipsToAdd = BigInteger.Parse(amount.Trim());
                if (chipsToAdd > 0)
                    LocalSettings.SetTotalChips(chipsToAdd);
                else
                {
                    LocalSettings.SetTotalChips(100000);
                    chipsToAdd = 100000;
                }
            }
            //else
            //{
            //    LocalSettings.SetTotalChips(100000);
            //    chipsToAdd = 100000;
            //}
            //RestAPI.Instance.AddChips(100000);
            Debug.Log("Amount adding: " + chipsToAdd + "  \n Total Chips: " + LocalSettings.GetTotalChips());
            //  FindObjectOfType<GetAPICash>().UpdateThisText(LocalSettings.GetTotalChips());
            PhotonNetwork.LocalPlayer.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());

        }

        public bool ModePanel()
        {
            return !tPVariationsMode.activeSelf && !selectModeAb.activeSelf && !selectModeWL.activeSelf && !selectModeClassicMufflisHumk.activeSelf && !selectModeLw.activeSelf && !selectModeDL.activeSelf && !SelectModePoker.activeSelf;
        }


        public void OnProfileBtnClick()
        {


            if (ModePanel())
                playerInfoPanel.gameObject.SetActive(true);

        }

        public void LogOut()
        {
            //switch (LoginWithAllAuth.AuthType)
            //{
            //    case 2:
            //        GoogleAuth.Instance.GoogleSigout();
            //        break;
            //    case 3:
            //        Facebookauth.Instance.FaceBook_LogOut();
            //        break;

            //}
            //GuestAuth.Instance.GuestSingOut();
        }
        public void GameQuit()
        {
            Application.Quit();
        }

    
       

        public Sprite ConvertTexture2DToSprite(Texture2D tex)
        {
            if (ProfileImageTexture)
            {
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), UnityEngine.Vector2.one);
                return sprite;
            }
            else return null;
        }



        public void GoldCollectedPanel(BigInteger collectedAmount)
        {
            goldCollectedTxt.text = LocalSettings.Rs(collectedAmount);
            goldCollectedSuccessfullyPanel.SetActive(true);
            SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.DailyReward, false);
        }
    }
}