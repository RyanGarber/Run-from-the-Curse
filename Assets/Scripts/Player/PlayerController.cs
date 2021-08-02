using UnityEngine;

using RyanGQ.RunOrDie.UI;
using RyanGQ.RunOrDie.Logic;

namespace RyanGQ.RunOrDie.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Rigidbody Body;
        public PlayerCamera Camera;
        private Vector2 _movement
        {
            get
            {
                return _movementCache;
            }
            set
            {
                _walking = Mathf.Abs(value.x) + Mathf.Abs(value.y) > float.Epsilon * 2f;
                if (value.y < float.Epsilon)
                    _sprinting = false;
                _movementCache = value;
            }
        }
        private Vector2 _movementCache;
        private bool _walking;
        private bool _sprinting = false;
        private bool _grounded;
        private float _movementCumulative = 50f;
        private float _movementDelta = 0f;
        private Vector3 _positionCache;
        private bool _blinderUsed = false;
        private bool _hasMoved = false;
        private float _lastShoot = -1f;

        [HideInInspector] public PlayerSync Sync = null;

        private void OnEnable()
        {
            GameManager.Singleton.Player = this;
        }

        private void Start()
        {
            Body.isKinematic = false;
        }

        private void Update()
        {
            if (Sync == null)
                Sync = GetComponent<PlayerSync>();

            // Detect ground / falling through map.
            _grounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit ground, 1.1f);
            if (!_grounded)
                _grounded = Physics.Raycast(transform.position + (transform.right * 0.35f), Vector3.down, out ground, 1.1f);
            if (!_grounded)
                _grounded = Physics.Raycast(transform.position + (transform.right * -0.35f), Vector3.down, out ground, 1.1f);
            if (transform.position.y <= -15f)
            {
                if (Sync.IsReaper)
                    GameManager.Singleton.Sync.photonView.RPC("GameEndedRPC", Photon.Pun.RpcTarget.AllBufferedViaServer, (byte) GameEndReason.ReaperDead);
                Sync.DieRPC();
            }


            // Controls.
            if (!GameManager.Singleton.IsPaused)
            {
                if (_grounded && Input.GetKeyDown(KeyCode.Space))
                    Body.AddForce(Vector3.up * 1000f);

                if (Input.GetKeyDown(KeyCode.LeftShift) && _walking && (_sprinting || _grounded))
                    _sprinting = !_sprinting;

                transform.localEulerAngles += new Vector3(0f, Input.GetAxis("Mouse X") * OptionsMenu.Sensitivity / OptionsMenu.MouseDivisorX);
                Camera.Apply(Input.GetAxis("Mouse Y") * OptionsMenu.Sensitivity / OptionsMenu.MouseDivisorY);
                
            
                if (Sync.IsReaper)
                {
                    if(Input.GetKeyDown(KeyCode.F) && Physics.Raycast(Camera.transform.position, Camera.transform.forward, out RaycastHit button, 20f, 1 << LayerMask.NameToLayer("Button")))
                    {
                        if(button.collider.TryGetComponent(out TrapActivator activator))
                        {
                            activator.Activate();
                        }
                    }
                    if (!_blinderUsed)
                    {
                        if (Input.GetKeyDown(KeyCode.G) && Physics.Raycast(Camera.transform.position, Camera.transform.forward, out RaycastHit ghost, 40f, 1 << LayerMask.NameToLayer("Ghost")))
                        {
                            {
                                if (ghost.collider.transform.root.TryGetComponent(out PlayerSync player))
                                {
                                    _blinderUsed = true;
                                    GameManager.Singleton.ReaperHelpText.GetComponent<UnityEngine.UI.Text>().text = "<color=#ccc>F</color>  activate traps";
                                    player.photonView.RPC("BlindedRPC", player.photonView.Owner);
                                }
                            }

                        }
                    }
                }
            }

            // Animations.
            if (Sync.Animator != null)
            {
                Sync.Animator.SetBool("Walk", _walking);
                Sync.Animator.SetBool("Run", _sprinting);
                Sync.Animator.SetBool("Grounded", _grounded);
            }

            if(!GameManager.Singleton.IsPaused)
            {
                if (!Sync.IsReaper)
                {
                    _movementDelta = Vector3.Distance(transform.position, _positionCache);
                    _movementCumulative = Mathf.Clamp(_movementCumulative + _movementDelta, 0f, 7.5f);
                    _positionCache = transform.position;

                    if (!_hasMoved && _movementDelta >= 1f)
                    {
                        _hasMoved = true;
                        _movementCumulative = 7.5f;
                    }

                    _movementCumulative = Mathf.MoveTowards(_movementCumulative, 0f, Time.deltaTime / 4f);
                    GameManager.Singleton.CurseIndicator.alpha = Mathf.Clamp01(1f - (_movementCumulative / 7.5f));
                    if (_movementCumulative == 0f)
                    {
                        Sync.DieRPC();
                    }
                }

                if (Input.GetKey(KeyCode.Mouse0) && Time.time - _lastShoot >= 0.25f)
                {
                    _lastShoot = Time.time;
                    GameManager.Singleton.Sync.photonView.RPC(
                        "FireBulletRPC",
                        Photon.Pun.RpcTarget.All,
                        Camera.transform.position + (Camera.transform.forward * (-Camera.transform.localPosition.z + 0.4f)),
                        Camera.transform.forward,
                        Sync.photonView.Owner.UserId
                    );
                }
            }
        }

        private void FixedUpdate()
        {
            // Apply force.
            Body.AddForce(Physics.gravity * 2.33f, ForceMode.Force);

            if (!GameManager.Singleton.IsPaused)
            {
                _movement = new Vector2(
                    Input.GetAxis("Horizontal") * (Sync.IsReaper ? 55f : 30f),
                    Input.GetAxis("Vertical") * (Sync.IsReaper ? 55f : 30f)
                );

                if (_sprinting)
                    _movementCache.y *= 1.5f;

                if (!_grounded)
                    _movementCache *= 0.6f;

                Body.AddForce((transform.right * _movement.x) + (transform.forward * _movement.y), ForceMode.Force);
            }
            else
            {
                _movement = Vector2.zero;
            }
        }
    }
}