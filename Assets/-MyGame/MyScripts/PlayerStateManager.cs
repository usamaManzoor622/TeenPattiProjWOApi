using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using System.Net.NetworkInformation;

namespace com.mani.muzamil.amjad
{
    public class PlayerStateManager : MonoBehaviourPunCallbacks
    {

        public List<PlayerInfo> PlayingList = new List<PlayerInfo>();

        private static PlayerStateManager _instance;
        public GameObject waitForNextRound;
        public GameObject taptoSitHere;
        public GameObject Amountobject;

        public static PlayerStateManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<PlayerStateManager>();
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance == null)
                _instance = this;
        }


        public void ArrayToListInPlayingList(int[] viewIds)
        {
            PlayingList.Clear();

            for (int i = 0; i < viewIds.Length; i++)
            {
                for (int j = 0; j < GameManager.Instance.playersList.Count; j++)
                {
                    if (GameManager.Instance.playersList[j].photonView.ViewID == viewIds[i])
                    {
                        PlayingList.Add(GameManager.Instance.playersList[j]);
                        break;
                    }
                }
            }
        }


        public void UpdatePlayingList()
        {

            //if (PhotonNetwork.IsMasterClient)
            //{
            // For Current Players Playing List Sorted                
            // photonView.RPC("SetNewPlayerListForGamePlay", RpcTarget.All);

            SetNewPlayerListForGamePlay();

            int[] temp = new int[PlayingList.Count];
            for (int i = 0; i < PlayingList.Count; i++)
            {
                temp[i] = PlayingList[i].photonView.ViewID;
            }

            // For New Player Who is Out of Game 
            // Will Get Playing List from this Property of Room
            if (PhotonNetwork.IsMasterClient)
                if (PhotonNetwork.IsConnectedAndReady)
                    LocalSettings.GetCurrentRoom.SetPlayingList(LocalSettings.playingListToArray, temp);
            //Debug.LogError("Array Length is + " + PlayingList.ToArray().Length);
            // }
        }

        //[PunRPC]
        public void SetNewPlayerListForGamePlay()
        {

            //            List<PlayerInfo> SortedList = GameManager.Instance.playersList.Where(o => o.getCurrentPlayerState().currentState == PlayerState.STATE.AbleToJoin).OrderBy(o => o.myNetworkSeat).ToList();
            if (MatchHandler.IsLuckyWar() && RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
            {
                List<PlayerInfo> SortedList = GameManager.Instance.playersList.Where(o => o.TieBetWinLw).OrderBy(o => o.myNetworkSeat).ToList();
                Debug.Log("Sorted List is " + SortedList);
                PlayingList = new List<PlayerInfo>();
                PlayingList = SortedList;
            }
            else
            {
                List<PlayerInfo> SortedList = GameManager.Instance.playersList.Where(o => o.player.GetPlayerStateProperty(LocalSettings.playerState) == PlayerState.STATE.AbleToJoin).OrderBy(o => o.myNetworkSeat).ToList();
                Debug.Log("Sorted List is " + SortedList);
                PlayingList = new List<PlayerInfo>();
                PlayingList = SortedList;
            }

        }



        //public void ShareNewListToOthers()
        //{
        //    //photonView.RPC("shareList", RpcTarget.All);
        //    //this is now handling from Player Current State Script
        //}


        public void UpdateListOnPlayerPack()
        {
            photonView.RPC("UpdateListOnPackedOnNetwork", RpcTarget.All);
        }

        //public void UpdateListOnPlayerOnStandUp()
        //{
        //    photonView.RPC("UpdateListOnPackedOnNetwork", RpcTarget.All);
        //}

        [PunRPC]
        public void UpdateListOnPackedOnNetwork()
        {
            for (int i = 0; i < PlayingList.Count; i++)
            {
                if (PlayingList[i].getCurrentPlayerState().currentState == PlayerState.STATE.Packed || PlayingList[i].getCurrentPlayerState().currentState == PlayerState.STATE.OutOfTable)
                {


                    PlayingList.RemoveAt(i);
                }
            }
            //PlayerWonGame();
        }

        public void RemainingPlayerWonGameAutomatic()
        {

            if (PlayingList.Count == 0)
            {
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
                {
                    UIManager.Instance.GetMyPlayerInfo().IAmWinner(true);
                    if (!MatchHandler.IsPoker())
                    {
                        UIManager.Instance.TotalWinsAmount += Pot.instance.potSize;
                        UIManager.Instance.TotalWinHands++;
                        GameManager.Instance.AddXPToMyPlayer(true);
                        UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
                        if (MatchHandler.IsTeenPatti())
                        {
                            if (UIManager.Instance.GetMyPlayerInfo().photonView.IsMine)
                            {
                                GameManager.Instance.PlayerTotalChipsUpdate(Pot.instance.potSize);
                                

                            }
                        }

                    }
                    else if (MatchHandler.IsPoker())
                    {
                        LocalSettings.SetPokerBuyInChips(PokerActionPanel.Instance.TotalPotAmount());
                        //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, PokerActionPanel.Instance.TotalPotAmount().ToString());
                        UIManager.Instance.TotalWinsAmount += PokerActionPanel.Instance.TotalPotAmount();
                        UIManager.Instance.TotalWinHands++;
                        GameManager.Instance.AddXPToMyPlayer(true);
                        UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
                        UIManager.Instance.GetMyPlayerInfo().player.SetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey, LocalSettings.GetPokerBuyInChips());
                        PokerActionPanel.Instance.ActionPanelPoker.SetActive(false);

                    }

                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                    SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);


                }
                //Before
                UIManager.Instance.GetMyPlayerInfo().FillerImage.gameObject.SetActive(false);
                UIManager.Instance.GetMyPlayerInfo().FillerImage.gameObject.SetActive(false);
                UIManager.Instance.ActionTable.SetActive(false);
                UpdateBuyInChipsForPoker(UIManager.Instance.GetMyPlayerInfo());
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(WaitBeforeReset(1));
                }
            }
            else if (PlayingList.Count <= 1)
            {


                ///Before
                if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
                {
                    PlayingList[0].IAmWinner(true);

                    if (PlayingList[0].player.IsLocal)
                    {
                        if (!MatchHandler.IsPoker())
                        {
                            UIManager.Instance.TotalWinsAmount += Pot.instance.potSize;
                            UIManager.Instance.TotalWinHands++;
                            GameManager.Instance.AddXPToMyPlayer(true);
                            UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
                            if (MatchHandler.IsTeenPatti())
                            {

                                GameManager.Instance.PlayerTotalChipsUpdate(Pot.instance.potSize);
                               


                            }

                        }
                        else if (MatchHandler.IsPoker())
                        {
                            LocalSettings.SetPokerBuyInChips(PokerActionPanel.Instance.TotalPotAmount());
                            //GoldWinLoose.Instance.SendGold(GoldWinLoose.Trans.win, PokerActionPanel.Instance.TotalPotAmount().ToString());
                            UIManager.Instance.TotalWinsAmount += PokerActionPanel.Instance.TotalPotAmount();
                            UIManager.Instance.TotalWinHands++;
                            GameManager.Instance.AddXPToMyPlayer(true);
                            UIManager.Instance.UpdateTheWinAmount(LocalSettings.totalcashWinLossKey, LocalSettings.TotalHandsKey, LocalSettings.WinHandsKey);
                            PlayingList[0].player.SetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey, LocalSettings.GetPokerBuyInChips());
                            PokerActionPanel.Instance.ActionPanelPoker.SetActive(false);


                        }
                        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.WinFinal, false);
                        SoundManager.Instance.PlayAudioClip(SoundManager.AllSounds.ChipsCollect, false);
                    }

                }
                ///After
                // if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying)
                //    UIManager.Instance.GetMyPlayerInfo().IAmWinner(true);

                //Before
                PlayingList[0].FillerImage.gameObject.SetActive(false);
                PlayingList[0].FillerImage.gameObject.SetActive(false);
                UIManager.Instance.ActionTable.SetActive(false);
                UpdateBuyInChipsForPoker(PlayingList[0]);
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(WaitBeforeReset(1));
                }
                //PlayerstateChangeForNextRound();

            }
            else
            {
                //PlayerTurnManager.Instance.GoToNextTurn();
            }
        }



        public void UpdateBuyInChipsForPoker(PlayerInfo pInfo)
        {
            StartCoroutine(UploadBuyInChips(pInfo, 1));
        }

        IEnumerator UploadBuyInChips(PlayerInfo pInfo, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (MatchHandler.IsPoker())
                pInfo.PokerTotalCashTxt.text = LocalSettings.Rs(pInfo.player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey));
            // Debug.LogError("3.....Check Here For Poker Cash....." + pInfo.PokerTotalCashTxt.text);
            //Debug.LogError("playeName....." + pInfo.player.NickName + "........Pocker Check BetAmount...." + pInfo.player.GetCustomBigIntegerData(LocalSettings.PlayerPokerTableCashKey));

        }


        IEnumerator WaitBeforeReset(float waitTime)
        {

            yield return new WaitForSeconds(waitTime);
            if (RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.GameIsPlaying || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.CardDistributing || RoomStateManager.Instance.CurrentRoomState == RoomState.STATE.ABFirstTurn)
            {
                RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ShowingResults);
                Debug.Log("Here is your error 3......." + RoomStateManager.Instance.CurrentRoomState);
                //PlayerStateManager.Instance.PlayingList.Clear();
            }
        }



        public PlayerInfo ReturnPlayer(int index)
        {
            for (int j = 0; j < PlayingList.Count; j++)
            {
                if (PlayingList[j] != null)
                {
                    if (PhotonNetwork.PlayerList[index].NickName == PlayingList[j].GetComponent<PhotonView>().Controller.NickName)
                        return PlayingList[j].GetComponent<PlayerInfo>();
                }
            }
            return null;
        }


        void UpdateAllListsOnPlayerleft(string ActorNumber)
        {

            UpdatePlayerListOnPlayerLeft(ActorNumber);
            UpdatePlayingListOnPlayerLeft(ActorNumber);
        }

        void UpdatePlayerListOnPlayerLeft(string ActorNumber)
        {
            for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
            {
                if (GameManager.Instance.playersList[i] == null)
                    GameManager.Instance.playersList.RemoveAt(i);
                else if (GameManager.Instance.playersList[i].player.NickName == ActorNumber)
                    GameManager.Instance.playersList.RemoveAt(i);
            }
        }

        void UpdatePlayingListOnPlayerLeft(string ActorNumber)
        {
            for (int i = 0; i < PlayingList.Count; i++)
            {
                if (PlayingList[i] == null)
                {
                    PlayingList.RemoveAt(i);
                    //GiveTurnToNext();
                }
                else if (PlayingList[i].player.NickName == ActorNumber)
                {
                    if (PlayingList[i].getCurrentPlayerState().currentState == PlayerState.STATE.ExecutingTurn)
                    {
                        int a = i;
                        a++;
                        if (a >= PlayingList.Count)
                        {
                            a = 0;
                        }
                        int CheckNumber = 0;
                        if (MatchHandler.IsPoker())
                        {
                            for (int k = 0; k < PlayingList.Count; k++)
                            {
                                if (!PlayingList[k].isBetPlacedPocker)
                                {
                                    CheckNumber++;
                                    // Debug.LogError("value of CheckNumber in Player State manager.....1" + CheckNumber);
                                }

                            }

                            if (CheckNumber <= 1 && PlayingList.Count > 2)
                            {
                                Debug.LogError("value of CheckNumber in Player State manager.....2" + CheckNumber);
                                RoomStateManager.Instance.UpdateCurrentRoomState(RoomState.STATE.ABFirstTurn);
                                GiveTurnToNext(PlayingList[i]);
                                //PokerActionPanel.Instance.GiveTurnToNextPlayerPocker();
                            }
                            else
                            {
                                GiveTurnToNext(PlayingList[i]);
                            }




                        }
                        // Debug.Log("Player Left Room Called " + PlayingList[a].player.NickName + PlayingList[a].getCurrentPlayerState().currentState);
                        if (MatchHandler.IsTeenPatti())
                            if (PlayingList[a].getCurrentPlayerState().currentState != PlayerState.STATE.RecieverSideShow)
                            {
                                // Debug.Log("Player Left Room Called " + PlayingList[a].player.NickName + PlayingList[a].getCurrentPlayerState().currentState);

                                GiveTurnToNext(PlayingList[i]);
                            }

                    }
                    else
                    {
                        if (PlayingList[i].IsSideShow && PlayingList[i].currentPlayerStateRef.currentState == PlayerState.STATE.RecieverSideShow)
                        {
                            Game_Play.Instance.OnClickCancelSideShowBtn();
                            // Debug.Log("Player Left Room Called " + PlayingList[i].player.NickName);
                        }
                        else
                        {
                            //Debug.LogError("Player Left Room Called " + PlayingList[i].player.NickName);
                            if (PlayingList[i].IsSideShow && PlayingList[i].currentPlayerStateRef.currentState == PlayerState.STATE.SenderSideShow)
                            {
                                // Debug.LogError("Player Left Room Called " + PlayingList[i].player.NickName);
                                Game_Play.Instance.OnClickCancelSideShowBtn();
                                PlayingList[i].AllSideShowPanelsFalse(false);

                            }
                        }

                    }
                    PlayingList.RemoveAt(i);
                }
            }

        }


        void GiveTurnToNext(PlayerInfo plyerInfo)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int nextPlayerInt = PlayingList.IndexOf(plyerInfo);
                //if (!dontIcrease)
                //{
                nextPlayerInt++;
                //}
                //dontIcrease = false;
                if (nextPlayerInt >= PlayingList.Count)
                    nextPlayerInt = 0;

                //Debug.LogError("Current Player Index: " + nextPlayerInt);
                PlayingList[nextPlayerInt].currentPlayerStateRef.UpdateCurrentPlayerState(PlayerState.STATE.ExecutingTurn);
            }
        }


        //public void UpdatePlayerIndexFromPlayingList()
        //{
        //    for (int i = 0; i < PlayingList.Count; i++)
        //    {
        //        PlayingList[i].PlayerIndexInPlayingList = i;
        //    }
        //}





        public bool CheckIfAllPlayersPlayedMaxChaals()
        {
            foreach (PlayerInfo plyrinfo in PlayingList)
            {
                if (plyrinfo.MyChaalsPlayedCounter < UIManager.Instance.TotalChals)
                    return false;
            }
            return true;
        }

        public void ShowCardsOfAllPlayers()
        {
            for (int i = 0; i < PlayingList.Count; i++)
            {
                if (PlayingList[i].photonView.IsMine)
                {
                    PlayingList[i].ShowCardsFromBlind();
                    PlayingList[i].IsSeen = true;
                    PlayingList[i].player.SetCustomBoolData("is_seen", true);
                }

                PlayingList[i].GiveSeenAlertToAll();
            }
        }


        public void AllPlayersGameCompleted()
        {
            photonView.RPC("UpDateAllPlayersGameCompleted", RpcTarget.All);
        }
        [PunRPC]
        public void UpDateAllPlayersGameCompleted()
        {
            GameResultsManager.Instance.isGameCompleted = true;
            for (int i = 0; i < PlayingList.Count; i++)
            {
                PlayingList[i].FillerImage.gameObject.SetActive(false);
                PlayingList[i].getCurrentPlayerState().currentState = PlayerState.STATE.Watching;
                PlayingList[i].IsSideShow = false;
            }
        }

        public void SideShowAndShowbtn()
        {
            if (PlayingList.Count == 2)
            {
                UIManager.Instance.showBtn.gameObject.SetActive(true);
                UIManager.Instance.sideShowBtn.gameObject.SetActive(false);
                if (UIManager.Instance.GetMyPlayerInfo().IsSeen)
                {
                    UIManager.Instance.showBtn.interactable = true;
                }
            }
            else
            {
                UIManager.Instance.showBtn.gameObject.SetActive(false);
                UIManager.Instance.sideShowBtn.gameObject.SetActive(true);
                SideShowPrev();

            }

        }

        public int SideShowPrev()
        {
            int myPlayerInt = PlayingList.IndexOf(UIManager.Instance.GetMyPlayerInfo());
            if (UIManager.Instance.GetMyPlayerInfo().IsSeen)
            {
                myPlayerInt--;
                if (myPlayerInt < 0)
                {
                    myPlayerInt = PlayingList.Count - 1;
                }



                if (PlayingList[myPlayerInt].IsSeen)
                {
                    UIManager.Instance.sideShowBtn.interactable = true;
                }

            }
            return myPlayerInt;
        }
        public int SideShowNext()
        {
            int myPlayerInt = PlayingList.IndexOf(UIManager.Instance.GetMyPlayerInfo());

            myPlayerInt++;
            if (myPlayerInt >= PlayingList.Count)
            {
                myPlayerInt = 0;
            }
            return myPlayerInt;
        }
        public void GetPlayerCardStatus()
        {
            photonView.RPC("PlayerCardStatus", RpcTarget.All);
        }
        [PunRPC]
        public void PlayerCardStatus()
        {
            for (int i = 0; i < PlayingList.Count; i++)
            {
                if (PlayingList[i].photonView.IsMine)
                {
                    PlayingList[i].SeenIndicator.SetActive(false);
                    PlayingList[i].BlindIndicator.SetActive(false);
                    // ShowBtn.SetActive(true);
                }
                else
                {

                    PlayingList[i].ShowBtn.SetActive(false);
                    PlayingList[i].SeenIndicator.SetActive(false);
                    PlayingList[i].BlindIndicator.SetActive(true);

                }
            }

        }




        #region PhotonNetworkCallbacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {

            //if (UIManager.Instance.GetMyPlayerInfo().IsSideShow && UIManager.Instance.GetMyPlayerCurrentState().currentState == PlayerState.STATE.SideShow)
            //{
            //    UIManager.Instance.GetMyPlayerInfo().GivesideShowAlertToAll(false);

            //}

            if (!otherPlayer.IsMasterClient)
            {
                //NetCode.CheckPlayerConnectivity();
                UpdateAllListsOnPlayerleft(otherPlayer.NickName);
                if (MatchHandler.IsTeenPatti() || MatchHandler.IsPoker())
                    RemainingPlayerWonGameAutomatic();
            }
            else
            {
                // Debug.LogError("Old master client notice ______________________");
            }

            if (MatchHandler.IsTeenPatti())
                SideShowAndShowbtn();

            GameManager.Instance.UpdateWatchingPlayers();
        }
        //public void PlayerstateChangeForNextRound()
        //{
        //    if (photonView.IsMine)
        //    {
        //        for (int i = 0; i < GameManager.Instance.playersList.Count; i++)
        //        {
        //            GameManager.Instance.playersList[i].currentStateRef.UpdateCurrentState(PlayerState.STATE.AbleToJoin);

        //        }
        //    }
        //}




        bool dontIcrease;

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            //Debug.LogError("New master client is now: " + newMasterClient.NickName);
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    for (int i = 0; i < PlayingList.Count; i++)
            //    {
            //        if (PlayingList[i].getCurrentState().currentState == PlayerState.STATE.ExecutingTurn)
            //        {
            //            //dontIcrease = true;
            //            GiveTurnToNext(PlayingList[i]);

            //        }
            //    }
            //}
        }


        public override void OnLeftRoom()
        {
            //  Debug.LogError("I have left the Room");

            base.OnLeftRoom();
        }

        #endregion



        // For Background Player



    }
}