using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Unity Event System

public class ScriptableEventListener : MonoBehaviour
{
    [SerializeField] ScriptableEvent myEvent; // subsribe to the event

    [SerializeField] UnityEvent eventResponse; //

    private void OnEnable()
    {
        myEvent?.Register(this);
    }

    private void OnDisable()
    {
        myEvent?.UnRegister(this);
    }

    public void OnEventRaised()
    {
        // this calls the unity event
        eventResponse?.Invoke();
    }
}
