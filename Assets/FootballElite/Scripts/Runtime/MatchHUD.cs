using UnityEngine;
using UnityEngine.UI;

namespace FootballElite
{
    public sealed class MatchHUD : MonoBehaviour
    {
        [SerializeField] private MatchManager match;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text timerText;
        [SerializeField] private Button restartButton;

        public void Configure(MatchManager manager, Text score, Text timer, Button restart)
        {
            match = manager; scoreText = score; timerText = timer; restartButton = restart;
        }

        private void Start()
        {
            if (restartButton != null && match != null)
                restartButton.onClick.AddListener(match.ResetMatch);
        }

        private void Update()
        {
            if (match == null) return;
            if (scoreText != null) scoreText.text = "ELITE FC  " + match.HomeScore + "  :  " +
                match.AwayScore + "  RIVALES";
            int seconds = Mathf.CeilToInt(match.TimeRemaining);
            if (timerText != null)
                timerText.text = string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
        }
    }
}
