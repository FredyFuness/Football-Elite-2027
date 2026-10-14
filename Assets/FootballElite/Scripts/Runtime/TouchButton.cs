using UnityEngine;
using UnityEngine.EventSystems;

namespace FootballElite
{
    /// <summary>Separate held and one-shot event semantics for multitouch game controls.</summary>
    public sealed class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private int activePointerId = int.MinValue;
        private bool queued;
        public bool IsHeld { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (activePointerId != int.MinValue) return;
            activePointerId = eventData.pointerId;
            queued = true;
            IsHeld = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointerId) return;
            activePointerId = int.MinValue;
            IsHeld = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Do not release while swiping away: mobile OS still owns this pointer.
        }

        public bool ConsumePress()
        {
            bool value = queued;
            queued = false;
            return value;
        }

        private void OnDisable()
        {
            queued = false;
            IsHeld = false;
            activePointerId = int.MinValue;
        }
    }
}
