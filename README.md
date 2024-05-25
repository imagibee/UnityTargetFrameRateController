# UnityTargetFrameRateController

Monitors Time.smoothDeltaTime and automatically lowers the Application.targetFrameRate if frames are consistently taking to long to complete.  Solves the problem of automatically choosing a stable frame rate for a game on a given platform for fixed and oscillating loads.

## Installation and Usage
1. Copy ./Assets/Scripts/TargetFrameRateLimiter.cs into your project and attach it to a `GameObject`.
2. Call `TargetFrameRateLimiter.Start` to find a stable target frame rate for the given load.
```csharp
    // Limit the frame rate between 20 and 60 hertz
    TargetFrameRateLimiter.Start(20, 60);
```
## Demo
A demo project is included that allows you to select different loads and observe the behaviour.  The demo has been tested on OSX and iOS using 2022.3.  Refer to the demo for a more complete usage example.

## Contributing
Pull requests are welcome.