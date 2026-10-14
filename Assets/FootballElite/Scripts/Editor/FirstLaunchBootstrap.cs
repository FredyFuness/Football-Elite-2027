#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FootballElite.Editor
{
    /// <summary>One-time, non-destructive bootstrap for freshly cloned projects.</summary>
    [InitializeOnLoad]
    public static class FirstLaunchBootstrap
    {
        private const string TrainingPath = "Assets/FootballElite/Scenes/Training.unity";
        private const string PendingSession = "FootballElite.BootstrapPending";

        static FirstLaunchBootstrap()
        {
            EditorApplication.delayCall += TryPrepare;
        }

        private static void TryPrepare()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling ||
                EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer)
            {
                EditorApplication.delayCall += TryPrepare;
                return;
            }

            // Reopening an existing project must never replace saved/user work.
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(TrainingPath) != null) return;
            if (SessionState.GetBool(PendingSession, false)) return;

            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded) return;

            // Only proceed from Unity's untouched, unsaved default scene.
            if (!string.IsNullOrEmpty(scene.path) || scene.name != "Untitled" ||
                SceneManager.sceneCount != 1)
                return;

            GameObject[] roots = scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                if (root.name != "Main Camera" && root.name != "Directional Light" &&
                    root.name != "Global Volume")
                    return;
            }

            SessionState.SetBool(PendingSession, true);
            try
            {
                TrainingSceneBuilder.GenerateTrainingScene(true);
            }
            catch (System.Exception error)
            {
                Debug.LogError("[Football Elite] No se pudo preparar automáticamente el entrenamiento: " + error);
                SessionState.SetBool(PendingSession, false);
            }
        }
    }
}
#endif
