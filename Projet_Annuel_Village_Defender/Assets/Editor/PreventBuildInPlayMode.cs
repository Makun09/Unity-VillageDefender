using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace EditorTools
{
    /// <summary>
    /// Prevents BuildPipeline from running while Unity is in Play Mode.
    /// Some editor packages create temporary scenes during build and will throw
    /// InvalidOperationException if build starts in Play Mode.
    /// </summary>
    public sealed class PreventBuildInPlayMode : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            throw new BuildFailedException(
                "Build cancelled: stop Play Mode before building. " +
                "Unity editor packages cannot create temporary scenes during Play Mode.");
        }
    }
}
