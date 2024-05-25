using UnityEngine;

//
// TargetFrameRateLimiter monitors Time.smoothDeltaTime and
// automatically lowers the Application.targetFrameRate if
// frames are taking to long to complete.
//
// Given a minimum and maximum frame rate, it starts at the
// maximum and works its way down until it settles at a frame
// rate that is actually being achieved.
//
// The controller only adjusts the frame rate downward.  It
// never increases it. Once it settles at a frame rate the
// frame rate will not be increased unless Start is called again.
//
public class TargetFrameRateLimiter : MonoBehaviour {
    //
    // Start the controller.  May be called at anytime, including while
    // already running.  It is not necessary to call Stop before calling
    // Start.
    //
    // minimum - minimum target frame rate value.  The controller doesn't
    // adjust target frame rate below this value.
    //
    // maximum - maximum target frame rate value.  The controller doesn't
    // adjust target frame rate above this value.
    //
    // vsync - when true only discrete vsync-compatible target frame rate
    // values are used.  When false frame rate values in 1 Hz incriments
    // are used.  For systems that enforce vsync this value should be set
    // to true, but for systems that do not force vsync it may be set to
    // true or false.  The must universally compatible setting is true.
    //
    // bias - a nonpositive value that determines the weight given to
    // control events that are below the threshold.  A small amount of
    // negative bias is needed to move oscillating systems to the next
    // lower target frame rate setting.  Increasingly negative bias will
    // tend to take the system out of osciallation more rapidly.  However,
    // if negative bias becomes too large, the system may settle into
    // artificially low frame rates.
    //
    // cycle - the number of frames between control cycles. Larger values
    // result in less frequent adjustments to the target frame rate.  For
    // example, given a cycle value of 60, a system running at 30 fps
    // will adjust to the next lower frame rate in 2 seconds (assuming it
    // is consistently over the Target threshold).
    //
    // tolerance - the +/- thresholding tolerance in hertz.  Used to limit
    // oscillation. Higher values limit oscillation. Lower values allow
    // more fine grain control (especially when vsync = false).
    //
    // clock - the frequency of the vsync clock in hertz.
    //
    public static void Start(
        int minimum,
        int maximum,
        bool vsync = true,
        float bias = -1,
        int cycle = 60,
        int tolerance = 1,
        int clock = 60)
    {
        if (bias > 0) {
            throw new System.ArgumentException("bias must be a nonpositive value");
        }
        TargetFrameRateLimiter.clock = clock < 60 ? 60 : clock;
        TargetFrameRateLimiter.vsync = vsync;
        TargetFrameRateLimiter.minimum = MaybeGetVsyncFps(minimum < 1 ? 1 : minimum);
        TargetFrameRateLimiter.maximum = MaybeGetVsyncFps(maximum < minimum ? minimum : maximum);
        if (bias < 0) {
            var magnitude = 1f - bias;
            incriment = 1 / magnitude;
            decriment = 1;
        }
        else {
            incriment = 1;
            decriment = 1;
        }
        TargetFrameRateLimiter.cycle = cycle < 1 ? 1 : cycle;
        TargetFrameRateLimiter.tolerance = tolerance < 0 ? 0 : tolerance;
        targetFrameRate = AdjustFps(maximum, 0);
        controlSignal = 0;
        started = true;
    }

    // Stop the controller
    public static void Stop()
    {
        started = false;
    }

    // Get the target fps value
    public static float Target {
        get { return targetFrameRate; }
    }

    // Get the actual fps value
    public static float Actual { get; private set; }

    void Update()
    {
        Actual = Mathf.Ceil(1 / Time.smoothDeltaTime);
        var threshold = Mathf.FloorToInt(1 / Time.smoothDeltaTime);
        if (started) {
            if (threshold < targetFrameRate - tolerance) {
                controlSignal -= decriment;
                if (controlSignal <= -cycle) {
                    var nextTargetFrameRate = AdjustFps(targetFrameRate, -1);
                    if (nextTargetFrameRate != targetFrameRate) {
                        targetFrameRate = nextTargetFrameRate;
                    }
                    controlSignal = 0;
                }
            }
            else {
                controlSignal += incriment;
                if (controlSignal >= cycle) {
                    controlSignal = 0;
                }
            }
            if (targetFrameRate != Application.targetFrameRate) {
                Application.targetFrameRate = targetFrameRate;
                Debug.Log($"target frame rate {targetFrameRate}");
            }
        }
    }

    // Return the next fps value adjusted for vsync.  If amount is
    // 1 it returns the next higher fps value adjusted for vsync.
    // If 0 it returns the current value adjusted for vsync.  If -1
    // it returns the next lower fps value adjusted for vsync.
    static int AdjustFps(int fps, int direction)
    {
        fps += direction;
        if (fps >= maximum) {
            return maximum;
        }
        else if (fps <= minimum) {
            return minimum;
        }
        else if (vsync == false) {
            return fps;
        }
        else { // Vsync == true
            if (direction < 0) {
                return clock / Mathf.CeilToInt((float)clock / fps);
            }
            else if (direction > 0) {
                // NOTE: Not currently used and not tested
                return clock / (Mathf.FloorToInt((float)clock / fps) - 1);
            }
            else {
                return GetVsyncFps(fps);
            }
        }
    }

    static int MaybeGetVsyncFps(int fps)
    {
        if (vsync == false) {
            return fps;
        }
        else {
            return GetVsyncFps(fps);
        }
    }

    static int GetVsyncFps(int fps)
    {
        return clock / (clock / fps);
    }

    //
    // Private data
    //
    static bool vsync = true;
    static int clock;
    static int tolerance;
    static int minimum;
    static int maximum;
    static int targetFrameRate;
    static float controlSignal;
    static float incriment;
    static float decriment;
    static int cycle;
    static bool started;
}
