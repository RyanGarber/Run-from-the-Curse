using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using PhotonPlayer = Photon.Realtime.Player;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace RyanGQ.RunOrDie.UI
{
    /// <summary>
    /// Monolith script for the main menu.
    /// </summary>
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        [Header("Overlays")]
        [SerializeField] CanvasGroup NicknameGroup;
        [SerializeField] CanvasGroup OptionsGroup;
        
        [Header("Sections")]
        [SerializeField] GameObject Main;
        [SerializeField] GameObject Match;

        [Header("Miscellaneous")]
        [SerializeField] GameObject PlayerList;
        [SerializeField] GameObject PlayerListRow;
        [SerializeField] Text Countdown;

        private Dictionary<RectTransform, Text[]> _playerListRows = new Dictionary<RectTransform, Text[]>();
        private int _nicknameAlpha = 0;
        private int _optionsAlpha = 0;

        /// <summary>
        /// Loads previous nickname and connects to Photon.
        /// </summary>
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            // Load nickname and connect.
            if(PhotonNetwork.CurrentRoom != null)
            {
                Match.SetActive(true);
                UpdatePlayerList();
            }
            else
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.ConnectUsingSettings();
                if (PlayerPrefs.HasKey("Nickname"))
                {
                    PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname");
                    _nicknameAlpha = 0;
                }
                else
                {
                    _nicknameAlpha = 1;
                }
                Main.SetActive(true);
            }
        }

        /// <summary>
        /// Handles transitions and input.
        /// </summary>
        private void Update()
        {
            NicknameGroup.alpha = Mathf.MoveTowards(NicknameGroup.alpha, _nicknameAlpha, Time.deltaTime * 5f);
            NicknameGroup.gameObject.SetActive(NicknameGroup.alpha > float.Epsilon);
            OptionsGroup.alpha = Mathf.MoveTowards(OptionsGroup.alpha, _optionsAlpha, Time.deltaTime * 5f);
            OptionsGroup.gameObject.SetActive(OptionsGroup.alpha > float.Epsilon);

            if (Input.GetKeyDown(KeyCode.Escape) && _optionsAlpha == 1)
                _optionsAlpha = 0;
        }

        public override void OnJoinedRoom()
        {
            Main.SetActive(false);
            Match.SetActive(true);
            UpdatePlayerList();
        }

        public override void OnPlayerEnteredRoom(PhotonPlayer player)
        {
            UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(PhotonPlayer player)
        {
            UpdatePlayerList();
        }

        public override void OnPlayerPropertiesUpdate(PhotonPlayer player, Hashtable properties)
        {
            UpdatePlayerList();
        }

        public override void OnLeftRoom()
        {
            Match.SetActive(false);
            Main.SetActive(true);
        }

        private void UpdatePlayerList()
        {
            while(_playerListRows.Count > PhotonNetwork.CurrentRoom.PlayerCount)
            {
                RectTransform row = _playerListRows.Keys.Last();
                Destroy(row.gameObject);
                _playerListRows.Remove(row);
            }
            while(_playerListRows.Count < PhotonNetwork.CurrentRoom.PlayerCount)
            {
                RectTransform row = Instantiate(PlayerListRow, PlayerList.transform).GetComponent<RectTransform>();
                row.gameObject.SetActive(true);
                _playerListRows.Add(row, new Text[] {
                    row.Find("Name").GetComponent<Text>(),
                    row.Find("Ready").GetComponent<Text>()
                });
            }
            int readyCount = 0;
            for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                PhotonPlayer player = PhotonNetwork.CurrentRoom.Players.Values.ElementAt(i);
                bool ready = player.CustomProperties.ContainsKey("Ready") && (string)player.CustomProperties["Ready"] == "Yes";
                if (ready)
                    readyCount++;
                _playerListRows.ElementAt(i).Value[0].text = player.NickName;
                _playerListRows.ElementAt(i).Value[1].text = ready ? "Ready" : "No";
                _playerListRows.ElementAt(i).Key.anchoredPosition = new Vector2(100f, 65f - (i * 20f));
            }
            if((float) readyCount / PhotonNetwork.CurrentRoom.PlayerCount >= 0.66f)
            {
                Countdown.text = "Starting soon";
                StartCoroutine("MatchCountdownCoroutine");
            }
            else
            {
                Countdown.text = "Waiting for ready players";
                StopCoroutine("MatchCountdownCoroutine");
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable properties)
        {
            if (properties.ContainsKey("Started"))
            {
                if ((string) properties["Started"] == "Yes")
                {
                    PhotonNetwork.LoadLevel(1);
                }
            }
        }

        private IEnumerator MatchCountdownCoroutine()
        {
            int time = 7;
            while(time > 0)
            {
                yield return new WaitForSeconds(1f);
                time--;
                Countdown.text = time > 0 ? "Starting in " + time : "Waiting for host";
            }
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
                {
                    ["Started"] = "Yes"
                });
        }

        public void OnReadyUp()
        {
            bool ready = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready")
                      && (string) PhotonNetwork.LocalPlayer.CustomProperties["Ready"] == "Yes";
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
            {
                ["Ready"] = ready ? "No" : "Yes"
            });
        }

        public void OnOpenOptions()
        {
            _optionsAlpha = 1;
        }

        public void OnLeaveMatch()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnFindMatch()
        {
            PhotonNetwork.JoinRandomOrCreateRoom(
                new Hashtable()
                {
                    ["Started"] = "No"
                },
                12,
                MatchmakingMode.FillRoom,
                null,
                null,
                null,
                new RoomOptions()
                {
                    PublishUserId = true,
                    MaxPlayers = 12,
                    CustomRoomProperties =
                        new Hashtable()
                        {
                            ["Started"] = "No"
                        },
                    CustomRoomPropertiesForLobby =
                        new string[]
                        {
                            "Started"
                        }
                }
            );
        }

        public void OnNicknameSubmit(string nickname)
        {
            PlayerPrefs.SetString("Nickname", nickname);
            PhotonNetwork.NickName = nickname;
            _nicknameAlpha = 0;
        }

        public void OnQuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}