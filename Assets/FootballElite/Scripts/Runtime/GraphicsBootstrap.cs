using UnityEngine;

namespace FootballElite
{
    /// <summary>Initial cross-platform quality policy; replace with proper URP profiles later.</summary>
    public sealed class GraphicsBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            bool mobile = Application.isMobilePlatform;
            Application.targetFrameRate = mobile ? 60 : 120;
            QualitySettings.vSyncCount = mobile ? 0 : 1;
            QualitySettings.shadowDistance = mobile ? 36f : 75f;
            QualitySettings.shadowCascades = mobile ? 1 : 2;
            QualitySettings.antiAliasing = mobile ? 2 : 4;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (mobile) Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }
}
