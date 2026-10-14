using UnityEngine;

namespace FootballElite
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class GoalTrigger : MonoBehaviour
    {
        [SerializeField] private MatchManager match;
        [SerializeField] private bool scoresForHome;

        public void Configure(MatchManager manager, bool forHome)
        {
            match = manager;
            scoresForHome = forHome;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (match == null || other.GetComponent<BallController>() == null) return;
            match.RegisterGoal(scoresForHome);
        }
    }
}
