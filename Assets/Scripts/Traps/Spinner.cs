using UnityEngine;

using RyanGQ.RunOrDie.Player;

namespace RyanGQ.RunOrDie.Traps
{
    public class Spinner : MonoBehaviour
    {
        public SpinnerTrap Trap;

        private void FixedUpdate()
        {
            if (Trap.IsActivated)
            {
                transform.Rotate(new Vector3(0f, 0f, Time.fixedDeltaTime * 200f));
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!Trap.IsActivated)
                return;

            if (collision.transform.parent == transform.parent)
                return;

            if(collision.transform.root.TryGetComponent(out PlayerSync player))
            {
                player.photonView.RPC("DieRPC", player.photonView.Owner);
            }
        }
    }
}