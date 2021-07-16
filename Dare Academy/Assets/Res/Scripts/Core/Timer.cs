using System.Diagnostics;

public struct Timer
{
    private Stopwatch sw;

    public void Start()
    {
        sw = new Stopwatch();
        sw.Start();
    }

    public void Stop()
    {
        sw.Stop();
        UnityEngine.Debug.Log($"Stopwatch [Time = {sw.Elapsed.TotalMilliseconds}ms]");
    }
}