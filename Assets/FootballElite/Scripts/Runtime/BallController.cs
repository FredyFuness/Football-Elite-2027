using UnityEngine;

namespace FootballElite
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public sealed class BallController : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        [SerializeField] private float dribbleForce = 16f;
        [SerializeField] private float maxDribbleSpeed = 8.3f;
        [SerializeField] private float passImpulse = 6.4f;
        [SerializeField] private float shotImpulse = 17f;

        public Vector3 Position => transform.position;
        public Rigidbody Body => body;

        private void Awake()
        {
            if (body == null) body = GetComponent<Rigidbody>();
            body.mass = .43f;
            body.linearDamping = .5f;
            body.angularDamping = .27f;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.maxAngularVelocity = 32f;
        }

        public void TryDribble(Vector3 footballerPosition, Vector3 forward, float runningSpeed)
        {
            Vector3 delta = transform.position - footballerPosition;
            delta.y = 0f;
            if (delta.sqrMagnitude > 2.2f || transform.position.y > 1.1f) return;

            Vector3 target = footballerPosition + forward * 1.14f;
            Vector3 desired = target - transform.position;
            desired.y = 0f;
            if (desired.sqrMagnitude < .02f) return;
            if (body.linearVelocity.magnitude > maxDribbleSpeed + runningSpeed * .25f) return;
            body.AddForce(desired.normalized * (dribbleForce + runningSpeed * 1.15f), ForceMode.Acceleration);
        }

        public bool Kick(Vector3 footballerPosition, Vector3 forward, bool shot)
        {
            Vector3 horizontal = transform.position - footballerPosition;
            horizontal.y = 0;
            if (horizontal.sqrMagnitude > 5.2f || transform.position.y > 1.35f) return false;

            forward.y = 0;
            if (forward.sqrMagnitude < .01f) forward = Vector3.forward;
            forward.Normalize();

            body.linearVelocity = new Vector3(body.linearVelocity.x * .14f,
                Mathf.Max(0, body.linearVelocity.y), body.linearVelocity.z * .14f);
            Vector3 impulse = (forward * (shot ? shotImpulse : passImpulse) +
                               Vector3.up * (shot ? 4.0f : .75f));
            body.AddForce(impulse, ForceMode.Impulse);
            body.AddTorque(new Vector3(forward.z, 0, -forward.x) * (shot ? 1.1f : .45f), ForceMode.Impulse);
            return true;
        }

        public void ResetBall(Vector3 position)
        {
            body.position = position;
            body.rotation = Quaternion.identity;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.Sleep();
            body.WakeUp();
        }
    }
}
