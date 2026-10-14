using UnityEngine;
using UnityEngine.EventSystems;

namespace FootballElite
{
    /// <summary>Pointer-id aware joystick, including simultaneous presses on action buttons.</summary>
    public sealed class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform baseRect;
        [SerializeField] private RectTransform knob;
        [Range(.05f, 1f)] [SerializeField] private float deadZone = .12f;

        private int activePointerId = int.MinValue;
        public Vector2 Value { get; private set; }

        public void Configure(RectTransform baseTransform, RectTransform knobTransform)
        {
            baseRect = baseTransform;
            knob = knobTransform;
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (activePointerId != int.MinValue) return;
            activePointerId = data.pointerId;
            OnDrag(data);
        }

        public void OnDrag(PointerEventData data)
        {
            if (activePointerId != data.pointerId || baseRect == null) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    baseRect, data.position, data.pressEventCamera, out Vector2 local)) return;

            float radius = Mathf.Max(1f, Mathf.Min(baseRect.rect.width, baseRect.rect.height) * .37f);
            Vector2 normalized = Vector2.ClampMagnitude(local / radius, 1f);
            Value = normalized.magnitude < deadZone ? Vector2.zero : normalized;
            if (knob != null) knob.anchoredPosition = normalized * radius;
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (activePointerId != data.pointerId) return;
            ResetJoystick();
        }

        private void OnDisable() { ResetJoystick(); }

        private void ResetJoystick()
        {
            activePointerId = int.MinValue;
            Value = Vector2.zero;
            if (knob != null) knob.anchoredPosition = Vector2.zero;
        }
    }
}
