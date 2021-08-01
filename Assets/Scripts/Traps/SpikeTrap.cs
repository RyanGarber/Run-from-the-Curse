using System.Collections;

using UnityEngine;

using Photon.Pun;

using RyanGQ.RunOrDie.Logic;
using RyanGQ.RunOrDie.Player;

namespace RyanGQ.RunOrDie.Traps
{
    public class SpikeTrap : ITrap
    {
        public Rigidbody[] Spikes;
        public KillTrigger Trigger;

        [PunRPC]
        public override void Activate()
        {
            if (IsActivated)
                return;

            IsActivated = true;
            foreach(Rigidbody r in Spikes)
            {
                r.useGravity = true;
            }
            if (GameManager.Singleton.Player.Sync.IsReaper)
                StartCoroutine("ActivateCoroutine");
        }

        private IEnumerator ActivateCoroutine()
        {
            yield return new WaitForSeconds(0.45f);
            foreach (PlayerSync player in Trigger.Players)
                player.photonView.RPC("DieRPC", player.photonView.Owner);
        }
    }
}
