using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Particles effect played on activation and auto disabled through particle system
public class VFXPool : MonoBehaviour
{
    [SerializeField] private VoidEventSO OnSceneReady = default;
    [System.Serializable]
    public class VFX
    {
        public VFXType type;
        public GameObject effect;
    }


    public enum VFXType
    {
        BoxStack,
        CoinCollect
    }
    public static VFXPool Instance { get; private set; }

    [SerializeField] private int poolSize = 10;

    [SerializeField] private VFX[] List;

    private Queue<VFX>[] m_Pool;
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
    private void OnEnable()
    {
        OnSceneReady.OnEventRaised += SetupPool;
    }
    private void OnDisable()
    {
        OnSceneReady.OnEventRaised -= SetupPool;
    }


    private void SetupPool()
    {
        m_Pool = new Queue<VFX>[List.Length];

        for (int i = 0; i < List.Length; ++i)
        {
            m_Pool[i] = new Queue<VFX>();
            CreateNewInstances(i);
        }
    }
    void CreateNewInstances(int index)
    {
        var vfx = List[index];

        for (int i = 0; i < poolSize; ++i)
        {
            VFX newVfx = new VFX();
            var particles = Instantiate(vfx.effect);
            particles.SetActive(false);

            newVfx.effect = particles;
            newVfx.type = vfx.type;

            m_Pool[(int)vfx.type].Enqueue(newVfx);
        }
    }
      
    private static VFX GetVFX(VFXType type)
    {
        int idx = (int)type;
        /*
        if (Instance.m_Pool[idx].Count == 0)
        {
            Instance.CreateNewInstances(idx);
        }
        */
        var inst = Instance.m_Pool[idx].Dequeue();
        inst.effect.SetActive(true);
        Instance.m_Pool[idx].Enqueue(inst);
        return inst;
    }

    public static VFX PlayVFX(VFXType type, Vector3 position)
    {

        var i = GetVFX(type);
        i.effect.transform.position = position;
        return i;
    }
}
