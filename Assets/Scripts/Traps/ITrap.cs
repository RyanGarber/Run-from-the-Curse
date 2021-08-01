using Photon.Pun;

namespace RyanGQ.RunOrDie.Traps
{
    /// <summary>
    /// A trap in the scene that can be activated by the reaper.
    /// </summary>
    public abstract class ITrap : MonoBehaviourPun
    {
        /// <summary>
        /// Whether this trap has been used.
        /// </summary>
        public bool IsActivated = false;
        
        /// <summary>
        /// Activates the trap. It can only be used once.
        /// </summary>
        [PunRPC]
        public abstract void Activate();
    }
}
