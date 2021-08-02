using UnityEngine;

using Photon.Pun;

using RyanGQ.RunOrDie.Traps;

namespace RyanGQ.RunOrDie.Logic
{
    public class TrapActivator : MonoBehaviour
    {
        public ITrap Trap;
        public MeshRenderer Button;
        public Material ActivatedButton;

        public void Activate()
        {
            Debug.Log("Activating " + Trap.name);
            if (Trap.IsActivated)
                return;
            Trap.photonView.RPC("Activate", RpcTarget.AllBufferedViaServer);
            Button.material = ActivatedButton;
        }
    }
}
