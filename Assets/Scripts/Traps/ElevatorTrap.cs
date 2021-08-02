using System.Collections;

using UnityEngine;

using Photon.Pun;

namespace RyanGQ.RunOrDie.Traps
{
    public class ElevatorTrap : ITrap
    {
        public Transform[] Platforms;
        public float Elevation;
        private int[] _targetPlatforms;
        private Vector3[] _originalPositions;

        public override bool IsActivated => _activated;
        private bool _activated = false;

        private void Update()
        {
            Debug.Log("Update");
            if(IsActivated)
            {
                Debug.Log("Lerping elevator");
                for (int i = 0; i < 2; i++)
                {
                    Platforms[_targetPlatforms[i]].position = Vector3.Lerp(
                        Platforms[_targetPlatforms[i]].position,
                        _originalPositions[i] + (Vector3.up * Elevation),
                        Time.deltaTime * 5f
                    );
                }
            }
        }

        [PunRPC]
        public override IEnumerator Activate()
        {
            _activated = true;

            _targetPlatforms = new int[2] {
                Random.Range(0, Platforms.Length),
                Random.Range(0, Platforms.Length)
            };
            while (_targetPlatforms[1] == _targetPlatforms[0])
                _targetPlatforms[1] = Random.Range(0, Platforms.Length);

            _originalPositions = new Vector3[2];
            for (int i = 0; i < 2; i++) {
                _originalPositions[i] = Platforms[_targetPlatforms[i]].position;
            }

            // Reset
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < 2; i++)
                _originalPositions[i] = _originalPositions[i] - (Vector3.up * Elevation);
        }
    }
}