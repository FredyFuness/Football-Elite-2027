using UnityEngine;

namespace FootballElite
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private FootballInput input;
        [SerializeField] private BallController ball;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float jogSpeed = 5.4f;
        [SerializeField] private float sprintSpeed = 8.8f;
        [SerializeField] private float rotationSpeed = 12f;
        private Vector3 velocity;

        public float LocomotionAmount { get; private set; }

        public void Configure(FootballInput source, BallController football, CharacterController character)
        {
            input = source; ball = football; controller = character;
        }

        private void Awake()
        {
            if (controller == null) controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (input == null || controller == null) return;
            Vector3 move = new Vector3(input.Move.x, 0f, input.Move.y);
            move = Vector3.ClampMagnitude(move, 1f);
            float speed = input.Sprint ? sprintSpeed : jogSpeed;
            LocomotionAmount = move.magnitude * speed;

            if (move.sqrMagnitude > .025f)
            {
                Quaternion desired = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desired, rotationSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && velocity.y < 0f) velocity.y = -1.5f;
            velocity.y += Physics.gravity.y * Time.deltaTime;
            controller.Move(move * speed * Time.deltaTime + velocity * Time.deltaTime);

            if (ball != null)
            {
                if (input.PassPressed) ball.Kick(transform.position, transform.forward, false);
                if (input.ShootPressed) ball.Kick(transform.position, transform.forward, true);
            }
        }

        private void FixedUpdate()
        {
            if (ball != null && controller != null && controller.enabled)
                ball.TryDribble(transform.position, transform.forward, LocomotionAmount);
        }

        public void Teleport(Vector3 position, Quaternion facing)
        {
            if (controller == null) return;
            controller.enabled = false;
            transform.SetPositionAndRotation(position, facing);
            velocity = Vector3.zero;
            controller.enabled = true;
        }
    }
}
