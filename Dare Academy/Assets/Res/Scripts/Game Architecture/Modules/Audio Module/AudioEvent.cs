using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEvent
{
    public bool released = false;

    private int polyphony = 0;
    private Queue<FMOD.Studio.EventInstance> _instances = null;
    private FMOD.Studio.EventInstance _instance;

    private string _eventName = "event:/Warning Noise/New Event";

    public AudioEvent(string name)
    {
        _eventName = name;
        _instance = FMODUnity.RuntimeManager.CreateInstance(_eventName);
    }

    public AudioEvent(string name, int maxPolyphany)
    {
        _eventName = name;
        polyphony = maxPolyphany;
        _instances = new Queue<FMOD.Studio.EventInstance>();
        for (int i = 0; i < maxPolyphany; i++)
        {
            _instances.Enqueue(FMODUnity.RuntimeManager.CreateInstance(_eventName));
        }
    }

    public AudioEvent()
    {
        _instance = FMODUnity.RuntimeManager.CreateInstance(_eventName);
    }

    public void Play()
    {
        if (polyphony == 0)
        {
            _instance.start();
        }
        else
        {
            for (int i = 0; i < polyphony; i++)
            {
                FMOD.Studio.PLAYBACK_STATE _STATE;
                _instances.Peek().getPlaybackState(out _STATE);
                if (_STATE != FMOD.Studio.PLAYBACK_STATE.STOPPED)
                {
                    var temp = _instances.Dequeue();
                    _instances.Enqueue(temp);
                }
                else
                {
                    _instances.Peek().start();
                    return;
                }
                _instances.Peek().stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _instances.Peek().start();
                var cycle = _instances.Dequeue();
                _instances.Enqueue(cycle);
            }
        }
    }

    public void TogglePause()
    {
        bool _paused;
        _instance.getPaused(out _paused);
        _instance.setPaused(!_paused);
    }

    public void Pause()
    {
        bool _paused;
        _instance.getPaused(out _paused);
        if (_paused)
            return;

        _instance.setPaused(true);
    }

    public void Unpause()
    {
        bool _paused;
        _instance.getPaused(out _paused);
        if (!_paused)
            return;

        _instance.setPaused(false);
    }

    public void FadeStop()
    {
        if (polyphony == 0)
            _instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        else
            foreach (var instance in _instances)
            {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
    }

    public void HardStop()
    {
        if (polyphony == 0)
            _instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        else
            foreach (var instance in _instances)
            {
                instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
    }

    public void DeleteEvent()
    {
        if (polyphony == 0)
        {
            _instance.release();
            released = true;
        }
        else
        {
            foreach (var instance in _instances)
            {
                instance.release();
            }
            released = true;
        }
    }

    public void SetParameter(string name, float value)
    {
        if (polyphony == 0)
            _instance.setParameterByName(name, value);
        else
            _instances.Peek().setParameterByName(name, value); // can only change top level
    }
}