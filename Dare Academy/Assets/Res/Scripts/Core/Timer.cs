using System.Diagnostics;
using JUtil;
using System;

public class Timer : IDisposable
{
    private Stopwatch sw;
    string name;

    public Timer()
    {
        name = "stopwatch";
        sw = new Stopwatch();
        sw.Start();
    }

    public Timer(string in_name)
    {
        name = in_name;
        sw = new Stopwatch();
        sw.Start();
    }

    public void Dispose() => Stop();

    [Obsolete]
    public void Start()
    {
        sw = new Stopwatch();
        sw.Start();
    }

    public void Stop()
    {
        sw.Stop();
        JUtils.ShowTime(sw.ElapsedTicks, "Timer: " + name + ", elapsed in: ");
    }
}