using UnityEngine;

namespace FootballElite
{
    /// <summary>Simple lateral tracking; goalkeeper animation and diving come in Phase 3.</summary>
    public sealed class GoalkeeperAI : MonoBehaviour
    {
        [SerializeField] private BallController ball;
        [SerializeField] private float lateralSpeed = 5.4f;
        [SerializeField] private float reactionDistance = 27f;
        [SerializeField] private float homeZ = 47.9f;
        private Vector3 initialPosition;

        public void Configure(BallController football, float z)
        {
            ball = football;
            homeZ = z;
            initialPosition = transform.position;
        }

        private void Start()
        {
            if (initialPosition == Vector3.zero) initialPosition = transform.position;
        }

        private void Update()
        {
            if (ball == null) return;
            float distance = Mathf.Abs(ball.Position.z - homeZ);
            float desiredX = distance < reactionDistance
                ? Mathf.Clamp(ball.Position.x * .82f, -3.1f, 3.1f)
                : 0f;
            Vector3 target = new Vector3(desiredX, transform.position.y, homeZ);
            transform.position = Vector3.MoveTowards(transform.position, target, lateralSpeed * Time.deltaTime);
            if (Mathf.Abs(ball.Position.x - transform.position.x) > .3f)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(new Vector3(ball.Position.x - transform.position.x, 0f, -4f)),
                    Time.deltaTime * 5f);
        }

        public void ResetGoalkeeper()
        {
            transform.SetPositionAndRotation(initialPosition, Quaternion.LookRotation(Vector3.back));
        }
    }
}
