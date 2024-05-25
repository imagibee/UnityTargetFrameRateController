using System.Diagnostics;
using UnityEngine;
using TMPro;

// Try to hit the requested frame rates in value1 and value2
// Either toggles between the two, or sets them to the same
// value to hold still
public class LoadSimulator : MonoBehaviour
{
    public static bool Ready;
    public static int Overhead;
    public TextMeshProUGUI VsyncText;
    readonly Stopwatch stopwatch = new();
    float value1 = 20;
    float value2 = 20;
    float expirationMs;
    int frameCount;
    int measureCount;
    float sum;
    bool vsync = true;
#if UNITY_EDITOR
    const int cycle = 5;
    const int measureCycle = 120;
#else
    const int cycle = 60;
    const int measureCycle = 60;
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
            var oddOrEven = frameCount++ % 2;
            if (oddOrEven == 0) {
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

    public void OnVsync()
    {
        vsync = !vsync;
        VsyncText.text = $"Vsync = {vsync}";
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
