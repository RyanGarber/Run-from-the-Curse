using UnityEngine;

using Photon.Pun;

using RyanGQ.RunOrDie.Logic;

namespace RyanGQ.RunOrDie.Player
{
    public class Bullet : MonoBehaviour
    {
        public Rigidbody Body;
        private bool _fired = false;
        private string player;

        public void Fire(string player)
        {
            _fired = true;
            this.player = player;
            gameObject.SetLayerRecursively(LayerMask.NameToLayer(LayerMask.LayerToName(GameManager.Singleton.Sync.Players[player].gameObject.layer) + " Bullet"));
            Invoke("Reset", 5f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_fired)
                return;

            // Ignore other player's bullet collisions and our own team.
            if (player == PhotonNetwork.LocalPlayer.UserId)
            {
                if (collision.transform.root.TryGetComponent(out PlayerSync player))
                {
                    GameManager.Singleton.Crosshair.color = Color.red;
                    player.photonView.RPC("DamageRPC", player.photonView.Owner);
                }
            }
            Reset();
        }

        private void Reset()
        {
            _fired = false;
            gameObject.SetActive(false);
        }
    }
}
