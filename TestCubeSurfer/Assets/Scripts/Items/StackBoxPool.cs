using System.Collections.Generic;
using UnityEngine;

public class StackBoxPool : MonoBehaviour
{
    public static StackBoxPool Instance { get; private set; }
    public StackBox prefab;
    public Queue<StackBox> m_Pool = new Queue<StackBox>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
              Instance = this;
        }
    }

    private void Start()
    {
        if (prefab != null)
        {

            int size = 30;
            for (int i = 0; i < size; ++i)
            {
                StackBox box = Instantiate(prefab);
                box.transform.localScale = Vector3.zero;
                box.gameObject.SetActive(false);
                m_Pool.Enqueue(box);
            }
        }
    }
    private StackBox GetBox()
    {
        if (Instance.m_Pool.Count == 0)
        {
            return null;
        }

        var box = m_Pool.Dequeue();

        box.gameObject.SetActive(true);
        m_Pool.Enqueue(box);
        return box;
    }
    public StackBox SpawnBox(Vector3 position, Transform parrent)
    {
        var b = GetBox();
        if (b != null)
        {
            b.transform.parent = parrent;
            position.y -= 0.5f;
            b.transform.localPosition = position;
            b.transform.localRotation = Quaternion.identity;
            b.Emergence();
            return b;
        }
        return null;
    }
}
