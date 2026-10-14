using UnityEngine;

namespace FootballElite
{
    /// <summary>
    /// Hides touch controls on desktop while keeping keyboard and gamepad active.
    /// Enable ShowOnDesktop in the Inspector to test buttons using a mouse in Editor.
    /// </summary>
    public sealed class TouchControlVisibility : MonoBehaviour
    {
        [SerializeField] private bool showOnDesktop;
        [SerializeField] private GameObject[] elements;

        public void Configure(params GameObject[] controls)
        {
            elements = controls;
        }

        private void Start()
        {
            bool visible = Application.isMobilePlatform || showOnDesktop;
            if (elements == null) return;
            foreach (GameObject element in elements)
                if (element != null) element.SetActive(visible);
        }
    }
}
