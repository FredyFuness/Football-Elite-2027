using UnityEngine;

namespace FootballElite
{
    [RequireComponent(typeof(Camera))]
    public sealed class BroadcastCamera : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform ball;
        [SerializeField] private Vector3 followOffset = new Vector3(0f, 24f, -29f);
        [SerializeField] private float smoothing = 4.2f;
        [SerializeField] private float focusBlend = .42f;

        public void Configure(Transform trackedPlayer, Transform football)
        {
            player = trackedPlayer; ball = football;
        }

        private void LateUpdate()
        {
            if (player == null || ball == null) return;
            Vector3 focus = Vector3.Lerp(player.position, ball.position, focusBlend);
            focus.y = .4f;
            Vector3 targetPosition = focus + followOffset;
            float factor = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPosition, factor);
            Quaternion look = Quaternion.LookRotation(focus - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, factor);
        }
    }
}
