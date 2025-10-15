using UnityEngine;

public class SceneStartManager : MonoBehaviour
{
    void Start()
    {
        TimerManager timer = FindObjectOfType<TimerManager>();
        if (timer != null)
        {
            timer.StartTimer();
        }
        Debug.Log("[SceneStartManager] 플레이씬 시작, 타이머 시작!");
    }
}
