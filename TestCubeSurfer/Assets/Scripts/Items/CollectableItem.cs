using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : Item
{

    public override void OnInteraction()
    {
        gameObject.SetActive(false);
        VFXPool.PlayVFX(VFXPool.VFXType.CoinCollect, transform.position);
    }
}
