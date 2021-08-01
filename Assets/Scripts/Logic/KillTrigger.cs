using System.Collections.Generic;

using UnityEngine;

using RyanGQ.RunOrDie.Player;

namespace RyanGQ.RunOrDie.Logic
{
    public class KillTrigger : MonoBehaviour
    {
        public List<PlayerSync> Players = new List<PlayerSync>();

        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.root.TryGetComponent(out PlayerSync player))
            {
                if (!Players.Contains(player))
                    Players.Add(player);
                Debug.Log(player.photonView.Owner.NickName + " entered");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.transform.root.TryGetComponent(out PlayerSync player))
            {
                if (Players.Contains(player))
                    Players.Remove(player);
                Debug.Log(player.photonView.Owner.NickName + " exited");
            }
        }
    }
}
