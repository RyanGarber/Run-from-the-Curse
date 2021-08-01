using RyanGQ.RunOrDie.Logic;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace RyanGQ.RunOrDie.UI
{
    public class SkippableIntro : MonoBehaviour
    {
        [SerializeField] Text IntroText;
        [SerializeField] CanvasGroup IntroTextGroup;
        [SerializeField] GameObject SkipText;
        [SerializeField] GameObject Background;

        public bool IsRunning => Background.activeSelf;

        private readonly string[] IntroLines = new string[]
        {
            "When you die, you become a ghost.",
            "You thought it would be easy.",
            "But on the way to heaven, you realize:",
            "The end was much further than you thought.",
            "The reaper has you trapped, forever fighting to leave.",
            "He says, \"welcome, little ghost.\"",
            "The time to fight the curse has come."
        };
        private int _introState = -1;

        private void Start()
        {
            Background.SetActive(true);
        }

        private void Update()
        {
            if(SkipText.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Singleton.Sync.photonView.RPC("GameStartedRPC", Photon.Pun.RpcTarget.AllBufferedViaServer);
            }
        }

        public void Skip()
        {
            Background.SetActive(false);
            StopCoroutine("IntroCoroutine");
        }

        private IEnumerator IntroCoroutine(bool isHost)
        {
            SkipText.SetActive(isHost);
            _introState = 0;
            while(_introState < IntroLines.Length)
            {
                IntroTextGroup.alpha = 0f;
                IntroText.text = IntroLines[_introState];
                while(IntroTextGroup.alpha < 0.97f)
                {
                    IntroTextGroup.alpha = Mathf.Lerp(IntroTextGroup.alpha, 1f, Time.deltaTime * 3f);
                    yield return null;
                }
                yield return new WaitForSeconds(1.66f);
                while(IntroTextGroup.alpha > 0.03f)
                {
                    IntroTextGroup.alpha = Mathf.Lerp(IntroTextGroup.alpha, 0f, Time.deltaTime * 3f);
                    yield return null;
                }
                yield return new WaitForSeconds(0.66f);
                _introState++;
            }
            Background.SetActive(false);
        }
    }
}