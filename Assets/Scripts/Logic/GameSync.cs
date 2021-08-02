using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using PhotonPlayer = Photon.Realtime.Player;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using RyanGQ.RunOrDie.Player;

namespace RyanGQ.RunOrDie.Logic
{
    public class GameSync : MonoBehaviourPunCallbacks, IPunObservable
    {
        public BulletPool Pool;
        public string ReaperID;
        public Dictionary<string, PlayerSync> Players = new Dictionary<string, PlayerSync>();

        private void Awake()
        {
            GameManager.Singleton.Sync = this;
        }

        private void Start()
        {
            GameManager.Singleton.WaitingCard.SetActive(false);
            if(PhotonNetwork.IsMasterClient)
            {
                ReaperID = PhotonNetwork.CurrentRoom.Players.ElementAt(Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount)).Value.UserId;
                int nextSpawn = 0;
                foreach (PhotonPlayer player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    player.SetCustomProperties(new Hashtable()
                        {
                            ["Ready"] = "No"
                        }
                    );
                    Vector3 position;
                    Quaternion rotation;
                    if(player.UserId == ReaperID)
                    {
                        position = GameManager.Singleton.ReaperSpawn.position;
                        rotation = GameManager.Singleton.ReaperSpawn.rotation;
                    }
                    else
                    {
                        position = GameManager.Singleton.GhostSpawns[nextSpawn].position;
                        rotation = GameManager.Singleton.GhostSpawns[nextSpawn].rotation;
                        nextSpawn = nextSpawn == GameManager.Singleton.GhostSpawns.Length - 1 ? 0 : (nextSpawn + 1);
                    }
                    photonView.RPC("SpawnRPC", player, ReaperID, position, rotation);
                }

            }
        }

        public PlayerSync GetPlayer(string nickname)
        {
            return Players.Values.First(p => p.photonView.Owner.NickName.ToLower() == nickname.ToLower());
        }

        [PunRPC]
        private void FireBulletRPC(Vector3 position, Vector3 direction, string player)
        {
            Pool.FireBullet(position, direction, player);
        }

        [PunRPC]
        private void SpawnRPC(string reaperId, Vector3 position, Quaternion rotation)
        {
            ReaperID = reaperId;
            PlayerController player;
            player = PhotonNetwork.Instantiate("Player", position, rotation).GetComponent<PlayerController>();
            player.enabled = true;
            player.Camera.Enable();
            GameManager.Singleton.LobbyCamera.SetActive(false);
        }

        [PunRPC]
        private void GameStartedRPC()
        {
            GameManager.Singleton.Intro.Skip();
            if (GameManager.Singleton.Player.Sync.IsReaper)
                GameManager.Singleton.ReaperHelpText.SetActive(true);
            else
                GameManager.Singleton.GhostHelpText.SetActive(true);
        }

        [PunRPC]
        private void GameEndedRPC(byte reasonCode)
        {
            GameEndReason reason = (GameEndReason)reasonCode;
            GameManager.Singleton.Crosshair.gameObject.SetActive(false);
            GameManager.Singleton.WaitingCard.SetActive(true);
            switch(reason)
            {
                case GameEndReason.ReaperDead:
                    GameManager.Singleton.WaitingText.text = "Ghosts won!";
                    break;
                case GameEndReason.AllGhostsDead:
                    GameManager.Singleton.WaitingText.text = "The reaper won!";
                    break;
            }
            StartCoroutine("EndScreenCoroutine");
        }

        private IEnumerator EndScreenCoroutine()
        {
            yield return new WaitForSeconds(3f);
            GameManager.Singleton.WaitingCard.SetActive(true);
            int time = 7;
            while (time > 0)
            {
                GameManager.Singleton.WaitingText.text = "Main menu in " + time;
                yield return new WaitForSeconds(1f);
                time--;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable()
                {
                    ["Started"] = "No"
                });
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable properties)
        {
            if (properties.ContainsKey("Started") && (string)properties["Started"] == "No")
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.LoadLevel(0);
        }

        public List<PlayerSync> Ghosts => Players.Values.Where(p => !p.IsReaper).ToList();
        public PlayerSync Reaper => Players.Values.FirstOrDefault(p => p.IsReaper);

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
}