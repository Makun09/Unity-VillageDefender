using UnityEngine;
using Core;

namespace UI
{
    public class MenuButtonBridge : MonoBehaviour
    {
        public void OnStartPlayingClicked()
        {
            GameLoopManager.Instance?.StartPlaying();
        }

        public void OnRestartClicked()
        {
            GameLoopManager.Instance?.RestartGame();
        }

        public void OnReturnToMenuClicked()
        {
            GameLoopManager.Instance?.ReturnToMenu();
        }

        public void OnQuitClicked()
        {
            GameLoopManager.Instance?.QuitGame();
        }

        public void OnPauseClicked()
        {
            GameLoopManager.Instance?.Pause();
        }

        public void OnResumeClicked()
        {
            GameLoopManager.Instance?.Resume();
        }
    }
}