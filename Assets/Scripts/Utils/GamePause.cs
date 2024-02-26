namespace Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class GamePause
    {
        static bool gameIsPaused = false;
        public static void SetPause(bool pause)
        {
            if (!gameIsPaused && pause)
            {
                gameIsPaused = true;
                Time.timeScale = 0;
            }
            if (gameIsPaused && !pause)
            {
                gameIsPaused = false;
                Time.timeScale = 1;
            }
        }
    }
}
