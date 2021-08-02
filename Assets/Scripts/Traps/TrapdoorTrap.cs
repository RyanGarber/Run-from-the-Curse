using System.Collections;

using UnityEngine;

using Photon.Pun;

namespace RyanGQ.RunOrDie.Traps
{
    public class TrapdoorTrap : ITrap
    {
        public GameObject[] Trapdoors;
        private Vector3 _originalScale;
        private Vector3 _targetScale;
        
        public override bool IsActivated => _activated;
        private bool _activated = false;

        private void Update()
        {
            if (IsActivated)
            {
                foreach (GameObject t in Trapdoors)
                {
                    t.transform.localScale = Vector3.Lerp(t.transform.localScale, _targetScale, Time.deltaTime * 25f);
                }
            }
        }

        [PunRPC]
        public override IEnumerator Activate()
        {
            _activated = true;
            _originalScale = Trapdoors[0].transform.localScale;
            _targetScale = Vector3.zero;
            while (Trapdoors[0].transform.localScale.sqrMagnitude > 0.01f)
            {
                yield return null;
            }
            yield return new WaitForSeconds(3f);
            _targetScale = _originalScale;
        }
    }
}