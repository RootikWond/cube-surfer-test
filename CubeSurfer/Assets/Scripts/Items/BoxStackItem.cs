using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxStackItem : Item
{
    private Vector3 spawnPosition = Vector3.zero;
    //private List<GameObject> container = new List<GameObject>();
    private void Start()
    {
       // container.Add(initialPrefab);
        if (amount > 1)
        {
            for (int i = 0; i < amount-1; i++)
            {
                spawnPosition.y += transform.localScale.y;
                var box = Instantiate(initialPrefab, transform);
                box.transform.localPosition = spawnPosition;
                //container.Add(box);
            }
        }
    }

    public override void OnInteraction()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
        //Playe VFX
    }

}
