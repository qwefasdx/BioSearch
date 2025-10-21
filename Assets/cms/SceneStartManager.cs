using UnityEngine;

public class SceneStartManager : MonoBehaviour
{
    void Start()
    {
        TimerManager timer = FindObjectOfType<TimerManager>();
        SanityManager sanity = FindObjectOfType<SanityManager>();
        GameOverManager gameOver = FindObjectOfType<GameOverManager>();

        if (timer != null)
        {
            timer.ResetTimer();
            timer.StartTimer();
        }

        if (sanity != null)
        {
            sanity.ResetSanity();
        }

        if (gameOver != null)
        {
            gameOver.ResetGameOver();
        }

        Debug.Log("[SceneStartManager] Timer & Sanity reset, GameOver state cleared!");
    }
}
