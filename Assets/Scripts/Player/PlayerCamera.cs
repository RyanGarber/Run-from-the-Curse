using UnityEngine;

namespace RyanGQ.RunOrDie.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        public Transform WallRaycast;
        
        private float _y = 0f;

        private void Update()
        {
            Vector3 target;
            if (Physics.Linecast(transform.root.position, WallRaycast.position, out RaycastHit hit))
            {
                target = hit.point + transform.forward * 0.25f;
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 20f);
            }
            else
            {
                target = new Vector3(0f, 1.25f + (_y / 35f), -3f + (Mathf.Abs(_y) / 45f));
                transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * 20f);
            }
        }

        public void Apply(float y)
        {
            _y = Mathf.Clamp(_y - y, -75f, 75f);
            transform.localEulerAngles = new Vector3(_y, 0f);
        }

        public void Enable() => gameObject.SetActive(true);
        public void Disable() => gameObject.SetActive(false);
    }
}