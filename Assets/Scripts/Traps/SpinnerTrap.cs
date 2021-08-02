using System.Collections;

using UnityEngine;

using Photon.Pun;

namespace RyanGQ.RunOrDie.Traps
{
    public class SpinnerTrap : ITrap
    {
        public Transform[] Spinners;
        
        public override bool IsActivated => _activated;
        private bool _activated = false;

        [PunRPC]
        public override IEnumerator Activate()
        {
            _activated = true;
            yield return new WaitForSeconds(3f);
            foreach (Transform t in Spinners)
                t.gameObject.SetActive(false);
        }
    }
}