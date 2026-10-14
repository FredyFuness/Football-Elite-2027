using UnityEngine;

namespace FootballElite
{
    /// <summary>Temporary visual rig. Later replaced by rigged glTF/FBX animations.</summary>
    public sealed class FootballerVisualAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private Transform leftLeg;
        [SerializeField] private Transform rightLeg;
        [SerializeField] private Transform leftArm;
        [SerializeField] private Transform rightArm;
        private float cycle;

        public void Configure(PlayerController owner, Transform legLeft, Transform legRight, Transform armLeft, Transform armRight)
        {
            player = owner;
            leftLeg = legLeft; rightLeg = legRight;
            leftArm = armLeft; rightArm = armRight;
        }

        private void LateUpdate()
        {
            float speed = player != null ? player.LocomotionAmount : 0f;
            cycle += Time.deltaTime * Mathf.Lerp(1f, 12f, Mathf.Clamp01(speed / 8f));
            float swing = Mathf.Sin(cycle) * Mathf.Clamp01(speed / 6f) * 34f;
            if (leftLeg != null) leftLeg.localRotation = Quaternion.Euler(swing, 0, 0);
            if (rightLeg != null) rightLeg.localRotation = Quaternion.Euler(-swing, 0, 0);
            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(-swing * .6f, 0, 0);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(swing * .6f, 0, 0);
        }
    }
}
