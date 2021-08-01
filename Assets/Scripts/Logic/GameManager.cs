using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

using RyanGQ.RunOrDie.UI;
using RyanGQ.RunOrDie.Player;

namespace RyanGQ.RunOrDie.Logic
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup OptionsGroup;
        public Transform[] GhostSpawns;
        public Transform ReaperSpawn;
        public GameObject LobbyCamera;
        public GameObject WaitingCard;
        public Text WaitingText;
        public GameObject ReaperHelpText;
        public GameObject GhostHelpText;
        public GameObject Crosshair;
        public GameObject Blinder;
        public OptionsMenu Options;
        public SkippableIntro Intro;
        public CanvasGroup CurseIndicator;

        [HideInInspector] public PlayerController Player;
        [HideInInspector] public GameSync Sync;
        private int _optionsAlpha = 0;

        public static GameManager Singleton;

        public bool IsPaused
        {
            get
            {
                return
                       Intro.IsRunning
                    || Options.gameObject.activeSelf;
            }
        }

        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.InstantiateRoomObject("Game Sync", Vector3.zero, Quaternion.identity);
                Intro.StartCoroutine("IntroCoroutine", true);
            }
            else
            {
                Intro.StartCoroutine("IntroCoroutine", false);
            }
        }

        private void Update()
        {
            if (Intro.IsRunning)
                _optionsAlpha = 0;
            else if (Input.GetKeyDown(KeyCode.Escape))
                _optionsAlpha = _optionsAlpha == 0 ? 1 : 0;

            OptionsGroup.alpha = Mathf.MoveTowards(OptionsGroup.alpha, _optionsAlpha, Time.deltaTime * 5f);
            Options.gameObject.SetActive(OptionsGroup.alpha > float.Epsilon);
            Cursor.lockState = OptionsGroup.alpha > float.Epsilon ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}