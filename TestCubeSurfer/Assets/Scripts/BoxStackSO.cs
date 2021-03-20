using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Data/Box Stack")]
public class BoxStackSO : ScriptableObject
{
    public List<StackBox> Items = new List<StackBox>();

    public UnityAction OnEventRaised;

    public StackBox LastBox()
    {
        if (Items.Count > 0)
        {
            return Items[Items.Count - 1];
        }
        return null;
    }
    private void OnEnable()
    {
        Items.Clear();
    }
    private void RaiseEvent()
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke();
    }

    public void Add(StackBox thing)
    {
        if (!Items.Contains(thing))
            Items.Add(thing);
        RaiseEvent();
    }

    public void Remove(StackBox thing)
    {
        if (Items.Contains(thing))
            Items.Remove(thing);
        RaiseEvent();
    }
    public void RemoveLast()
    {
        if(Items.Count > 0)
        {
            Items.RemoveAt(Items.Count - 1);
            RaiseEvent();
        }
     
    }
}
