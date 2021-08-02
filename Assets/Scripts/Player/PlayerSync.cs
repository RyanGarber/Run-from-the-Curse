using UnityEngine;

using Photon.Pun;

using RyanGQ.RunOrDie.Logic;
using System.Collections;

namespace RyanGQ.RunOrDie.Player
{
    public class PlayerSync : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] GameObject GhostBody;
        [SerializeField] GameObject ReaperBody;
        public Animator Animator;
        public float Health = 100f;

        public bool IsReaper
        {
            get
            {
                return photonView.Owner.UserId == GameManager.Singleton.Sync.ReaperID;
            }
        }

        private void Start()
        {
            GameManager.Singleton.Sync.Players.Add(photonView.Owner.UserId, this);
            if (photonView.IsMine)
            {
                GameManager.Singleton.Crosshair.gameObject.SetActive(true);
            }

            if (IsReaper)
            {
                ReaperBody.SetActive(true);
                Animator = ReaperBody.GetComponent<Animator>();
                gameObject.SetLayerRecursively(LayerMask.NameToLayer("Reaper"));
            }
            else
            {
                GhostBody.SetActive(true);
                Animator = GhostBody.GetComponent<Animator>();
                gameObject.SetLayerRecursively(LayerMask.NameToLayer("Ghost"));
            }
        }

        [PunRPC]
        public void DamageRPC()
        {
            Health -= 20f;
            if(Health <= 0f)
            {
                DieRPC();
            }
        }

        [PunRPC]
        public void DieRPC()
        {
            if (photonView.IsMine)
            {
                GameManager.Singleton.Sync.Players.Remove(photonView.Owner.UserId);
                GameManager.Singleton.LobbyCamera.SetActive(true);
                GameManager.Singleton.Crosshair.gameObject.SetActive(false);
                GameManager.Singleton.Blinder.SetActive(false);
                GameManager.Singleton.CurseIndicator.gameObject.SetActive(false);
                GameManager.Singleton.GhostHelpText.SetActive(false);
                GameManager.Singleton.ReaperHelpText.SetActive(false);
            
                if (IsReaper)
                {
                    GameManager.Singleton.Sync.photonView.RPC("GameEndedRPC", RpcTarget.AllBufferedViaServer, (byte)GameEndReason.ReaperDead);
                }
                else
                {
                    if(GameManager.Singleton.Sync.Ghosts.Count == 0)
                        GameManager.Singleton.Sync.photonView.RPC("GameEndedRPC", RpcTarget.AllBufferedViaServer, (byte)GameEndReason.AllGhostsDead);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }

        [PunRPC]
        private void BlindedRPC()
        {
            if(photonView.IsMine)
            {
                GameManager.Singleton.Blinder.SetActive(true);
                StartCoroutine("BlinderCoroutine");
            }
        }

        private IEnumerator BlinderCoroutine()
        {
            yield return new WaitForSeconds(15f);
            GameManager.Singleton.Blinder.SetActive(false);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(Health);
            }
            else
            {
                Health = (float) stream.ReceiveNext();
            }
        }

        public override int GetHashCode()
        {
            return photonView.Owner.UserId.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return other.GetType() == this.GetType() && ((PlayerSync)other).photonView.Owner.UserId == this.photonView.Owner.UserId;
        }

        public static bool operator ==(PlayerSync a, PlayerSync b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(a, null))
                return true;
            return !ReferenceEquals(a, null) && a.Equals(b);
        }

        public static bool operator !=(PlayerSync a, PlayerSync b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(a, null))
                return false;
            return ReferenceEquals(a, null) || !a.Equals(b);
        }
    }
}
