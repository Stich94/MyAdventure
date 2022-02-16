using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ScriptableEvent")]
public class ScriptableEvent : ScriptableObject
{
    List<ScriptableEventListener> listeners = new List<ScriptableEventListener>();

    public void Register(ScriptableEventListener _listener)
    {
        if (!listeners.Contains(_listener))
        {
            listeners.Add(_listener);
        }
    }

    public void UnRegister(ScriptableEventListener _listener)
    {
        if (listeners.Contains(_listener))
        {
            listeners.Remove(_listener);
        }
    }

    public void RaisEvent()
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnEventRaised();
        }
    }
}
