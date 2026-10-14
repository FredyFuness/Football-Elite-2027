using System.Collections;
using UnityEngine;

namespace FootballElite
{
    public sealed class MatchManager : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private BallController ball;
        [SerializeField] private GoalkeeperAI keeper;
        [SerializeField] private float matchDurationSeconds = 180f;

        private bool goalLocked;
        private float timeLeft;
        public int HomeScore { get; private set; }
        public int AwayScore { get; private set; }
        public float TimeRemaining => timeLeft;
        public bool IsResetting => goalLocked;

        public void Configure(PlayerController playerController, BallController football, GoalkeeperAI goalkeeper)
        {
            player = playerController; ball = football; keeper = goalkeeper;
        }

        private void Awake() { timeLeft = matchDurationSeconds; }

        private void Update()
        {
            if (timeLeft <= 0f) return;
            timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);
        }

        public void RegisterGoal(bool homeScores)
        {
            if (goalLocked || timeLeft <= 0f) return;
            if (homeScores) HomeScore++; else AwayScore++;
            StartCoroutine(ResetAfterGoal());
        }

        private IEnumerator ResetAfterGoal()
        {
            goalLocked = true;
            yield return new WaitForSeconds(1.35f);
            RestorePositions();
            goalLocked = false;
        }

        private void RestorePositions()
        {
            if (player != null) player.Teleport(new Vector3(0f, 0f, -23f), Quaternion.identity);
            if (ball != null) ball.ResetBall(new Vector3(0f, .26f, -21.55f));
            if (keeper != null) keeper.ResetGoalkeeper();
        }

        public void ResetMatch()
        {
            StopAllCoroutines();
            goalLocked = false;
            HomeScore = AwayScore = 0;
            timeLeft = matchDurationSeconds;
            RestorePositions();
        }
    }
}
