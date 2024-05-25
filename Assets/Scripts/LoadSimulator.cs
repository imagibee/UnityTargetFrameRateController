using System.Diagnostics;
using UnityEngine;

// Try to hit the requested frame rates in value1 and value2
// Either toggles between the two, or sets them to the same
// value to hold still
public class LoadSimulator : MonoBehaviour
{
    public static bool Ready;
    public static int Overhead;
    readonly Stopwatch stopwatch = new();
    float value1 = 20;
    float value2 = 20;
    float expirationMs;
    int frameCount;
#if UNITY_EDITOR
    const int measureCycle = 120;
#else
    const int measureCycle = 60;
#endif
    int measureCount;
    float sum;
#if UNITY_EDITOR
    const bool vsync = false;
    const int cycle = 5;
#else
    const bool vsync = true;
    const int cycle = 60;
#endif

    void Update()
    {
        if (measureCount < measureCycle) {
            sum += Time.smoothDeltaTime;
            measureCount++;
            Overhead = Mathf.CeilToInt(sum / measureCount);
        }
        else {
            stopwatch.Restart();
            var cycle = frameCount++ % 2;
            if (cycle == 0) {
                expirationMs = 1000 / (value1 + Overhead);
            }
            else {
                expirationMs = 1000 / (value2 + Overhead);
            }
            while (stopwatch.ElapsedMilliseconds < expirationMs) {
                // wait
            }
            Ready = true;
        }
    }

    public void On17()
    {
        TargetFrameRateLimiter.Start(5, 60, vsync: vsync, cycle: cycle);
        value1 = value2 = 17;
    }

    public void On25()
    {
        TargetFrameRateLimiter.Start(5, 60, vsync: vsync, cycle: cycle);
        value1 = value2 = 25;
    }

    public void On1020()
    {
        TargetFrameRateLimiter.Start(5, 60, vsync: vsync, cycle: cycle);
        value1 = 10;
        value2 = 20;
    }

    public void On1520()
    {
        TargetFrameRateLimiter.Start(5, 60, vsync: vsync, cycle: cycle);
        value1 = 15;
        value2 = 20;
    }
}
