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

        public override bool IsActivated => _activated;
        private bool _activated = false;

        [PunRPC]
        public override IEnumerator Activate()
        {
            _activated = true;

            foreach (Rigidbody r in Spikes)
            {
                r.useGravity = true;
            }
            if (GameManager.Singleton.Player.Sync.IsReaper)
            {
                foreach (PlayerSync player in Trigger.Players)
                    player.photonView.RPC("DieRPC", player.photonView.Owner);
            }

            yield break;
        }
    }
}
