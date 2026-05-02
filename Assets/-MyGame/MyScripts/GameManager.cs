using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat.Demo;
//using Unity.Services.Authentication;

namespace com.mani.muzamil.amjad
{

    public class GameManager : MonoBehaviourPunCallbacks
    {

        [ShowOnly] public bool isRunINBackGround;

        public GameObject DummyCardPrefab;
        public GameObject playerPrefab, exitPanel;

        public Transform PlayerTable;

        //public Sprite[] playerProfileImage;
        public Collections PlayerProfileImages;
        public FramesCollections playerProfileFrameImage;

        public RectTransform positionAvailabilityFull;
        public RectTransform shakeAnimationWhenPositionAvailFul;
        public CardsContainer AllCards;
        public PositionAvailability[] position_availability;
        public List<PlayerInfo> playersList;

        public RectTransform DistributerCardPosition;

        public int TotalPlayers;
        public int[] CardsIndexes;

        public GameObject[] Particles;

        public RectTransform SupportingCardPos;
        [ShowOnly]
        public CardProperty SupportingCard;

        public RectTransform XpShowAdding;

        [ShowOnly]
        public int SupportingCardIndex;

        public int myLocalSeat
        {
            get { return privateLocalSeat; }
            set
            {
                privateLocalSeat = value;
                PositionsManager.Instance.AssignMyLocalPositionWithAllOtherClients();
            }
        }



        [SerializeField]
        private int privateLocalSeat;


        public TMP_Text TurnTxt;

        #region Creating Instance
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<GameManager>();
                return _instance;
            }
        }
        #endregion
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            //ActiveMyPanel(exitPanel.name);
            if (!PhotonNetwork.IsConnected)
                return;

            Application.runInBackground = true;

            if (MatchHandler.IsTeenPatti())
            {
                LocalSettings.SetMinPlayers(2);

            }
            else if (MatchHandler.IsAndarBahar())
            {
                LocalSettings.SetMinPlayers(1);
                //Debug.LogError("Game Starting Table Setting");
                AndarBaharPositionsManager.Instance.AdjustABTableThings();
            }
            else if (MatchHandler.isWingoLottary())
            {
                LocalSettings.SetMinPlayers(1);
            }
            else if (MatchHandler.IsPoker())
            {
                LocalSettings.SetMinPlayers(2);
            }
            else if (MatchHandler.IsLuckyWar())
            {
                LocalSettings.SetMinPlayers(1);
                LuckyWarManager.Instance.AdjustWLTableThings();
            }
            else if (MatchHandler.isDragonTiger())
            {
                LocalSettings.SetMinPlayers(1);
            }

            if (!MatchHandler.IsPoker())
                StartingThings();
            else
            {
                if (LocalSettings.GetTotalChips() >= (LocalSettings.GetStartingMinAmountPoker()))
                {

                    PokerTableAmount.Instance.OnJoinNowBtnClick();
                    PokerManager.Instance.BuyInCashPanel.SetActive(false);

                }
                else
                {
                    PokerManager.Instance.SetStartingAmount();
                    StartingThings();
                }

            }

            LocalSettings.GetSetRoomID = PhotonNetwork.CurrentRoom.Name.ToString();
            LocalSettings.GetSetGameName = MatchHandler.CurrentMatch.ToString();
            LocalSettings.GetSetTableName = NetworkSettings.Instance.RoomName;

        }


        public void StartingThings()
        {
            //if (UIManager.Instance.GetMyPlayerInfo() == null)
            if (ReferenceEquals(UIManager.Instance.GetMyPlayerInfo(), null))
                StartPlayerInstantiateNow();
            else
            {
                PositionsManager.Instance.SitHere(PokerManager.Instance.sitPosAfterReset);
                Invoke(nameof(RefreshAfterSomeTime), 0.5f);
                SitHereBtnStatus(false);
            }

            StartCoroutine(CheckAllPlayersConnected());
            PlayerTotalChipsUpdate(0);
            SitHereBtnStatus(false);
            Chating.Instance.StartingChatRef();
            UpdateWatchingPlayers();
        }


        void RefreshAfterSomeTime()
        {
            PositionsManager.Instance.AssignMyLocalPositionWithAllOtherClients();
        }


        public void SitHereBtnStatus(bool val)
        {
            foreach (var item in position_availability)
            {
                item.sitHere.SetActive(val);
                item.sitHere.transform.parent.GetComponent<Image>().enabled = !item.sitHere.activeSelf;
            }
        }
        public void ActiveMyPanel(string name)
        {
            exitPanel.SetActive(exitPanel.name.Equals(name));
        }

        public void StartPlayerInstantiateNow()
        {
            Debug.Log("Not Ready");
            if (PhotonNetwork.IsConnectedAndReady)
            {
                Debug.Log("Is Ready");
                GameObject NewPlayerInstantiated = PhotonNetwork.Instantiate(playerPrefab.name, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, 0);
                NewPlayerInstantiated.name = NewPlayerInstantiated.GetComponent<PhotonView>().Controller.NickName;
            }
        }
        // Assign Real Cards to All Players
        #region Assign Real Cards to All Players
        public void AssignCardsToAllPlayers()
        {
            // Transfering all cards to temporary list
            List<GameObject> TempRealCardsList = new List<GameObject>();
            for (int i = 0; i < AllCards.Card.Length; i++)
            {
                TempRealCardsList.Add(AllCards.Card[i].gameObject);
            }

            // Generating list of random selected cards according to number of players
            List<GameObject> SelectedRandomCardsList = new List<GameObject>();
            //for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount * 6; i++)
            int addSupportingCard = 0;
            if (MatchHandler.CurrentMatch == MatchHandler.MATCH.HUukm)
                addSupportingCard = 1;
            for (int i = 0; i < (PlayerStateManager.Instance.PlayingList.Count * 3) + addSupportingCard; i++)
            {
                int randomNumber = UnityEngine.Random.Range(0, TempRealCardsList.Count);
                SelectedRandomCardsList.Add(TempRealCardsList[randomNumber].gameObject);
                TempRealCardsList.RemoveAt(randomNumber);
            }
            //if (MatchHandler.CurrentMatch == MatchHandler.MATCH.HUKM)
            //{

            //    GameObject supCard = Instantiate(SelectedRandomCardsList[SelectedRandomCardsList.Count - 1], PlayerTable);
            //    SelectedRandomCardsList.RemoveAt(SelectedRandomCardsList.Count - 1);
            //    SupportingCard = supCard.GetComponent<CardProperty>();
            //    SetPosAndRect(SupportingCard.gameObject, SupportingCardPos, PlayerTable);
            //    SupportingCard.gameObject.SetActive(false);
            //}
            // Getting indexes of Selected cards for players
            CardsIndexes = new int[SelectedRandomCardsList.Count];
            for (int i = 0; i < SelectedRandomCardsList.Count; i++)
                CardsIndexes[i] = SelectedRandomCardsList[i].GetComponent<CardProperty>().CardIndexInArray;


            //  Debug.LogError("Master called NumberOf Times: ");
            if (UIManager.Instance.GetMyPlayerInfo() != null)
                UIManager.Instance.GetMyPlayerInfo().SendPlayerCardsArray(CardsIndexes);
        }

        #endregion

        /// Dummy cards distribution ////       
        #region Dummy Cards Generation and distribution
        List<GameObject> DummyCardsList;

        GameObject[] TempPlayersListForDummyCards;
        public void InstantiateDummyPlayerCard(int NumberOfPlayers)
        {
            AssigningPlayersToListForDummyCards();
            DummyCardsList = new List<GameObject>();
            int totalCardsToGenerate = NumberOfPlayers * 3;
            for (int i = 0; i < totalCardsToGenerate; i++)
            {
                GameObject card = Instantiate(DummyCardPrefab);
                DummyCardsList.Add(card);
                LocalSettings.SetPosAndRect(card, DistributerCardPosition.transform.GetChild(0).gameObject.GetComponent<RectTransform>(), DistributerCardPosition);
            }

            StartCoroutine(DistributeDummyCards());
        }
        void AssigningPlayersToListForDummyCards()
        {
            TempPlayersListForDummyCards = new GameObject[position_availability.Length];
            UpdateCard(0, 2);
            UpdateCard(1, 1);
            UpdateCard(2, 0);
            UpdateCard(3, 4);
            UpdateCard(4, 3);
        }
        void UpdateCard(int IndexDummyCardsAry, int indexReserved)
        {
            if (position_availability[indexReserved].is_reserved)
            {
                if (position_availability[indexReserved].is_reserved.activeInHierarchy)
                    TempPlayersListForDummyCards[IndexDummyCardsAry] = position_availability[indexReserved].is_reserved;
                else
                    TempPlayersListForDummyCards[IndexDummyCardsAry] = null;
            }
        }
        IEnumerator DistributeDummyCards()
        {
            yield return new WaitForSeconds(0.1f);
            float delay = 0.4f * (PlayerStateManager.Instance.PlayingList.Count * 3);
            Invoke(nameof(ChageRoomStateToGamePlaying), delay);
            posIndexTemp = 0;
            PlyerCrdDummyIndex = 0;
            {
                foreach (GameObject card in DummyCardsList)
                {
                    yield return new WaitForSeconds(0.4f);
                    GameObject dumPos = GetPosForDummyCard();
                    //Debug.LogError("DummyCard index: " + PlyerCrdDummyIndex);
                    if (PlyerCrdDummyIndex <= dumPos.GetComponent<PlayerInfo>().PlayerDummyCardsToShowParent.transform.childCount - 2)
                    {
                        Transform pos = dumPos.GetComponent<PlayerInfo>().PlayerDummyCardsToShowParent.transform.GetChild(PlyerCrdDummyIndex).transform;
                        GameObject childObj = dumPos.GetComponent<PlayerInfo>().PlayerDummyCardsToShowParent.transform.GetChild(PlyerCrdDummyIndex).gameObject;
                        StartCoroutine(PlayAnimation(card.transform, pos, childObj));
                        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.CardFlip, false);
                        //Debug.LogError("card distribute sound is playing");
                    }
                }

                PlayerStateManager psm = PlayerStateManager.Instance;
                foreach (PlayerInfo plyrinfo in psm.PlayingList)
                {
                    plyrinfo.PlayerDummyCardsToShowParent.transform.GetChild(0).gameObject.SetActive(true);
                    plyrinfo.PlayerDummyCardsToShowParent.transform.GetChild(1).gameObject.SetActive(true);
                    plyrinfo.PlayerDummyCardsToShowParent.transform.GetChild(2).gameObject.SetActive(true);
                    plyrinfo.PlayerDummyCardsToShowParent.SetActive(true);

                    plyrinfo.ShowBtn.SetActive(plyrinfo.photonView.IsMine);

                }
            }

            Invoke(nameof(RemoveRemainingDummyCards), 0.4f);
        }

        void ChageRoomStateToGamePlaying()
        {
            if (PlayerStateManager.Instance.PlayingList.Count > 1)
                RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.GameIsPlaying);
            else
                RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ShowingResults);
        }
        void RemoveRemainingDummyCards()
        {
            for (int i = DummyCardsList.Count - 1; i >= 0; i--)
                Destroy(DummyCardsList[i]);
            DummyCardsList.Clear();
            if (SupportingCard)
                SupportingCard.gameObject.SetActive(true);
        }

        int posIndexTemp = 0;
        int PlyerCrdDummyIndex = 0;
        // return target pos
        GameObject GetPosForDummyCard()
        {
            if (posIndexTemp >= TempPlayersListForDummyCards.Length)
            {
                posIndexTemp = 0;
                PlyerCrdDummyIndex++;
            }
            if (CheckAvailability(posIndexTemp))
            {
                GameObject pos = TempPlayersListForDummyCards[posIndexTemp];
                posIndexTemp++;
                return pos;
            }
            else
            {
                posIndexTemp++;
                return GetPosForDummyCard();
            }
        }

        bool CheckAvailability(int value)
        {
            return TempPlayersListForDummyCards[value] != null;
        }
        //public void PlayAnimation(Transform ObjToAnimate, Transform targetPosition, GameObject ObjToActivate)
        public IEnumerator PlayAnimation(Transform ObjToAnimate, Transform targetPosition, GameObject ObjToActivate)
        {
            GameObject objectToActivate = ObjToActivate;
            yield return new WaitForSeconds(0);
            if (targetPosition != null)
                ObjToAnimate.DOMove(targetPosition.position, 0.4f, false).OnComplete(() => OnCompleteAnim(ObjToAnimate.gameObject, objectToActivate));
            float RotationOffSet = targetPosition.eulerAngles.z;
            if (ObjToAnimate != null)
            {
                ObjToAnimate.DOLocalRotate(new UnityEngine.Vector3(0, 0, 360 + RotationOffSet), 0.4f, RotateMode.FastBeyond360);
                ObjToAnimate.DOScale(new UnityEngine.Vector3(1.5f, 1.5f, 1.5f), 0.4f);
            }
        }




        void OnCompleteAnim(GameObject AnimatedObj, GameObject objectToActivate)
        {
            objectToActivate.SetActive(true);
            AnimatedObj.SetActive(false);
        }
        #endregion

        public int CountPlayers()
        {
            return playersList.Count;
        }

        void SetOtherPlayersPositioning(PhotonView photonView)
        {
            if (!photonView.IsMine)
            {
                //SetPosAndRect(photonView.gameObject, GetAvailableTransform(photonView.gameObject), PlayerTable);
            }

        }

        void SetAllPlayersPositions()
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                if (!playersList[i].seated)
                {
                    SetOtherPlayersPositioning(playersList[i].GetComponent<PhotonView>());
                    playersList[i].seated = true;
                }
            }
        }

        private IEnumerator CheckAllPlayersConnected()
        {
            if (PhotonNetwork.CurrentRoom == null)
                yield break;

            yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.PlayerCount == playersList.Count);
            SetAllPlayersPositions();

            PositionsManager.Instance.AssignMyLocalPositionWithAllOtherClients();
            if (playersList[0].photonView.IsMine && playersList[0].getCurrentPlayerState().currentState == PlayerState.STATE.OutOfGame)
            {
                if (MatchHandler.IsPoker() && PlayerStateManager.Instance.PlayingList.Count != 0)
                {
                    yield return null;
                }
                else
                    PlayerStateManager.Instance.ArrayToListInPlayingList(LocalSettings.GetCurrentRoom.GetPlayingList(LocalSettings.playingListToArray));
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
                {
                    foreach (var item in PlayerStateManager.Instance.PlayingList)
                    {
                        if (MatchHandler.IsTeenPatti())
                        {
                            item.PlayerDummyCardsToShowParent.SetActive(true);
                            item.PlayerDummyCardsToShowParent.transform.GetChild(0).gameObject.SetActive(true);
                            item.PlayerDummyCardsToShowParent.transform.GetChild(1).gameObject.SetActive(true);
                            item.PlayerDummyCardsToShowParent.transform.GetChild(2).gameObject.SetActive(true);
                        }
                        else if (MatchHandler.IsAndarBahar())
                        {
                            item.ABBettingSection.SetActive(true);
                            item.AbTurn = item.player.GetCustomBoolData(LocalSettings.AbturnKey);
                            // Debug.LogError("  " + item.AbTurn);
                        }
                        else if (MatchHandler.IsLuckyWar())
                        {
                            item.LWBettingSection.SetActive(true);
                            item.LWTurn = item.player.GetCustomBoolData(LocalSettings.LWturnKey);

                            // Debug.LogError("  " + item.AbTurn);
                        }
                    }
                    if (MatchHandler.IsAndarBahar())
                    {
                        PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.ABTunDurationSave);
                    }
                    else if (MatchHandler.IsLuckyWar())
                    {
                        PlayerTurnManager.Instance.turnManager.TurnDuration = LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.LWTunDurationSave);
                        if (CheckAllPlayersTurn())
                        {
                            if (LuckyWarManager.Instance.LWFirstCard == null)
                            {

                                LuckyWarManager.Instance.CardAssignToPlayerWhoOutOfGame(LocalSettings.GetCurrentRoom.GetCustomRoomData(LocalSettings.LwFirstCardKey));
                            }
                            foreach (PlayerInfo item in PlayerStateManager.Instance.PlayingList)
                            {
                                if (item.PlayerLWCard == null)
                                {
                                    Transform parent = item.LWDummyCardPrent.transform;
                                    GameObject pCard = Instantiate(AllCards.Card[item.player.GetCustomData(LocalSettings.LwPlayerCardKey)].gameObject);
                                    LuckyWarManager.Instance.ObjectsToDestroy.Add(pCard);
                                    LocalSettings.SetPosAndRect(pCard, parent.GetChild(0).GetComponent<RectTransform>(), parent);
                                    item.PlayerLWCard = pCard.GetComponent<CardProperty>();
                                    pCard.SetActive(true);
                                }
                            }

                        }

                    }
                }


            }


            //playersList.Clear();
            //while (CountPlayers() < (int)PhotonNetwork.CurrentRoom.PlayerCount)
            //{
            //    Debug.Log("Waiting For Other Players in Room");
            //    yield return new WaitForSeconds(1f);
            //}

            //if (CountPlayers() >= (int)PhotonNetwork.CurrentRoom.PlayerCount)
            //{
            //    yield return null;
            //}
            //else
            //{
            //    PlayersDestroy();
            //    StartCoroutine(CheckAllPlayersConnected());
            //}

            //SetAllPlayersPositions();

        }

        public bool CheckAllPlayersTurn()
        {
            foreach (PlayerInfo item in PlayerStateManager.Instance.PlayingList)
            {
                if (item.LWTurn)
                    return false;
            }
            return true;
        }
        //void ResetAvailablePositions()
        //{
        //    for (int i = 0; i < position_availability.Length; i++)
        //    {
        //        if (i != 0)
        //            position_availability[i].is_reserved = null;
        //    }
        //}


        public void AddingPlayer(PlayerInfo info)
        {
            playersList.Add(info);
            SetAllPlayersPositions();

            UpdateWatchingPlayers();
        }


        public override void OnJoinedRoom()
        {
            Debug.Log("Another Player Joined");

        }


      

        public void UpdateAllTextsOfCash(BigInteger cash)
        {
            //All Texts Data
            Debug.Log("Updated TExt is " + cash);
        }



        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("Player " + newPlayer.NickName + " has joined the room.");
            StartCoroutine(CheckAllPlayersConnected());
        }


        void PlayersDestroy()
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                Destroy(playersList[i]);
            }
        }

        public void PlayerTotalChipsUpdate(BigInteger chips)
        {
            LocalSettings.SetTotalChips(chips);
            if (chips < 0)
            {
                // Minus To server Chips
                // RestAPILuqman.Instance.SubtractChips(chips);
            }
            else if (chips > 0)
            {
                // Add To Server Chips
                // RestAPILuqman.Instance.AddChips(chips);
            }
            // Debug.LogError("Check Winner sound" + SoundManager.AllSounds.Reward.name + "....Chips   " + chips);

            // SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.Reward, false);
            UIManager.Instance.PlayerTotalChipsTxt.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            GameStartManager.Instance.ResetWaiting();
            StartCoroutine(CheckAllPlayersConnected());
            //Debug.Log("Other Player named " + otherPlayer.NickName + " left this Room");
            base.OnPlayerLeftRoom(otherPlayer);
            
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            Debug.Log("Player properties updated: " + changedProps.ToStringFull() + targetPlayer.NickName);
            if (changedProps.ContainsKey(LocalSettings.networkPosition))
            {
                PositionsManager.Instance.AssignMyLocalPositionWithAllOtherClients();
                Debug.Log("updated Network Position");
                
            }
        }

        public void ShowParticle(RectTransform obj)
        {
            GameObject particle = Instantiate(Particles[UnityEngine.Random.Range(0, GameManager.Instance.Particles.Length)]);
            particle.transform.position = obj.transform.position;
            LocalSettings.SetPosAndRect(particle, obj, PlayerTable);
            particle.SetActive(true);
        }

        // For Extra Players

        public void WhenSettingFull()
        {
            if (playersList.Count <= 1)
                return;
            //if (playersList.Count < position_availability.Length)
            //    return;
            // yield return new WaitForSeconds(0);
            if (MatchHandler.isWingoLottary())
                LocalSettings.SetPosAndRect(playersList[0].gameObject, position_availability[0].Pos, WingoManager.Instance.WingoPositions[0].transform.parent);
            else if (MatchHandler.isDragonTiger())
                LocalSettings.SetPosAndRect(playersList[0].gameObject, position_availability[0].Pos, DragonTigerManager.Instance.DTPlayerSittingPositions[0].transform.parent);

            List<PlayerInfo> playerInfos = new List<PlayerInfo>();
            playerInfos = playersList.Where(mineInfo => !mineInfo.photonView.IsMine).OrderByDescending(Info => Info.player.GetCustomBigIntegerData(LocalSettings.totalcashWinLossKey)).ToList();

            for (int i = 1; i <= playerInfos.Count; i++)
            {
                if (i == position_availability.Length)
                {
                    if (playerInfos.Count >= position_availability.Length - 1)
                        for (int j = i - 1; j < playerInfos.Count; j++)
                        {
                            LocalSettings.SetPosAndRect(playersList[j].gameObject, positionAvailabilityFull, positionAvailabilityFull.parent);
                            playerInfos[j].player_name.transform.parent.gameObject.SetActive(false);
                            playerInfos[j].playerTotalCash.transform.parent.gameObject.SetActive(false);

                        }
                }
                else
                {
                    if (MatchHandler.isWingoLottary())
                        LocalSettings.SetPosAndRect(playerInfos[i - 1].gameObject, position_availability[i].Pos, WingoManager.Instance.WingoPositions[i].parent);
                    else if (MatchHandler.isDragonTiger())
                        LocalSettings.SetPosAndRect(playerInfos[i - 1].gameObject, position_availability[i].Pos, DragonTigerManager.Instance.DTPlayerSittingPositions[i].parent);
                    playerInfos[i - 1].player_name.transform.parent.gameObject.SetActive(true);
                    playerInfos[i - 1].playerTotalCash.transform.parent.gameObject.SetActive(true);
                }
            }

        }
        //dsfklsdlkjfsdklj
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (NetworkSettings.roomInfos != null)
                NetworkSettings.roomInfos.Clear();
            NetworkSettings.roomInfos = roomList;
            Debug.LogError("Room Name: " + NetworkSettings.roomInfos.Count + " | Player Count: " + "/");
            foreach (RoomInfo roomInfo in NetworkSettings.roomInfos)
            {
                if (roomInfo == null)
                {
                    NetworkSettings.roomInfos.Remove(roomInfo);
                }
                Debug.LogError(roomInfo.CustomProperties +
                 "   Room Name: " + roomInfo.Name + " | Player Count: " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers);
            }
        }

        public void UpdateWatchingPlayers()
        {
            int watchingPlayers = 0;
            foreach (PlayerInfo pInfo in playersList)
            {
                if (pInfo.getCurrentPlayerState().currentState == PlayerState.STATE.OutOfTable || !pInfo.gameObject.activeSelf)
                {
                    watchingPlayers++;
                    // Debug.LogError("Check playerListCount" + playersList.Count +"   " + watchingPlayers);
                }
            }
            // Debug.LogError("Check playerListCount" + playersList.Count + "   " + watchingPlayers);
            UIManager.Instance.watchingGamePlayer.text = "Watching: " + watchingPlayers;
        }


        #region When application goes to background
        // Player In background In mobile Game
        private void OnApplicationPause(bool pause)
        {
            bool istrue = PlayerStateManager.Instance.PlayingList.Exists(playerInfo => playerInfo.photonView.ViewID == UIManager.Instance.GetMyPlayerInfo().photonView.ViewID);

            if (MatchHandler.IsPoker() && istrue)
            {
                playersList[0].playerBackGround(pause);
                PhotonNetwork.SendAllOutgoingCommands();

            }

            if (pause)
            {
                if (MatchHandler.IsPoker())
                    isRunINBackGround = pause;
                // GameOnBackGround = true;
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }
                    if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
                    {
                        return;
                    }

                    PhotonNetwork.SetMasterClient(PhotonNetwork.MasterClient.GetNext());
                    PhotonNetwork.SendAllOutgoingCommands();
                }
            }
        }
        #endregion

        #region Update xp for current player

        public void AddXPToMyPlayer(bool isWin)
        {
            int xpToadd = isWin ? 99 : 66;
            LocalSettings.SetPendingXP(xpToadd);
            XpShowAdding.GetChild(0).GetComponent<TMP_Text>().text = "XP\n+" + xpToadd;
            XpShowAdding.gameObject.SetActive(true);
            XPLevelupdate.Instance.UpdateXP();

            int levelNumber = UIManager.Instance.GetMyPlayerInfo().player.GetCustomData(LocalSettings.PlayerTotalLevelKey);

            if (XPLevelCalculator.Instance.CurrentLevel > levelNumber)
            {
                BigInteger reward = LocalSettings.levelUpRewardAmount * XPLevelCalculator.Instance.CurrentLevel;
              
                LocalSettings.SetTotalChips(reward);
                UIManager.Instance.PlayerTotalCashText.text = LocalSettings.Rs(LocalSettings.GetTotalChips());
                UIManager.Instance.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.MyTotalCashKey, LocalSettings.GetTotalChips());
                UIManager.Instance.levelUpRewardText.text = LocalSettings.Rs(reward);
                UIManager.Instance.levelUpText.text = ("Level Up\n" + XPLevelCalculator.Instance.CurrentLevel).ToString();
                UIManager.Instance.GetMyPlayerInfo().player.SetCustomData(LocalSettings.PlayerTotalLevelKey, XPLevelCalculator.Instance.CurrentLevel);
                Debug.LogError("Adding Reward Accrorng to Level Increase" + reward);
                UIManager.Instance.levelUpPanel.SetActive(true);
                SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
            }
        }


        #endregion
    }




    [System.Serializable]
    public class PositionAvailability
    {
        public RectTransform Pos;
        public GameObject is_reserved;
        [ShowOnly]
        public int networkSeat;
        public GameObject sitHere;
    }


}