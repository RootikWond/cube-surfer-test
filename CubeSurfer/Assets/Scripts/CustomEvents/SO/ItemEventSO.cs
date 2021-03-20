using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Item Event")]
public class ItemEventSO : ScriptableObject
{
    public UnityAction<Item, Vector3> OnEventRaised;
    public void RaiseEvent(Item value, Vector3 screenPosition)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(value, screenPosition);
    }
}
