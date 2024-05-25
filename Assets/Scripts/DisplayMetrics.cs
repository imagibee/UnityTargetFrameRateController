using UnityEngine;
using TMPro;

public class DisplayMetrics : MonoBehaviour
{
    public TextMeshProUGUI tmpro;

    void Update()
    {
        if (LoadSimulator.Ready) {
            var color = "<color=\"red\">";
            if (TargetFrameRateLimiter.Actual >= TargetFrameRateLimiter.Target) {
                color = "<color=\"green\">";
            }
            tmpro.text = $"<color=\"black\">Target: {TargetFrameRateLimiter.Target}\n" +
                $"{color}Actual: {TargetFrameRateLimiter.Actual}</color>\n";
        }
        else {
            tmpro.text = "<color=\"yellow\">Warming up";
        }
    }
}
