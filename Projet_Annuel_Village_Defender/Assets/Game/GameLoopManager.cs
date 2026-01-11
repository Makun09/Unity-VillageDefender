using System.Collections;
using UnityEngine;


namespace Game
{
    public class GameLoopManager : MonoBehaviour
    {
        public bool LoopShouldEnd;
        void Start()
        {
            
        }

        IEnumerator GameLoop()
        {
            while (LoopShouldEnd == false)
            {
                yield return null;
            }
        }
    }
}