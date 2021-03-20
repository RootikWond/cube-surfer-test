using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public GameObject initialPrefab;
    public int amount = 1;
    public enum ItemType
    {
        Box,
        Coin,
        Bonus
    }
    public ItemType type;
    public virtual void OnInteraction()
    {
       
    }
}
