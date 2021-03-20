using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackBox : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private BoxStackSO stack;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        //StartBox
        if (transform.parent !=null && transform.parent.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            var player = transform.parent.GetComponent<PlayerController>();
            player.Stack.Add(this);

        }
    }

    public void Emergence()
    {
        VFXPool.PlayVFX(VFXPool.VFXType.BoxStack, transform.position);

        StartCoroutine(ScaleUp(0.2f));
    }
    public void Remove()
    {
        stack.Remove(this);
        if (transform.parent != null)
        {
            transform.parent = null;
        }
        gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
    }
    IEnumerator ScaleUp(float scaleDuration)
    {

        float elapsetTime = 0;
        while (elapsetTime < scaleDuration)
        {
            elapsetTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsetTime/scaleDuration);
            yield return null;
        }
        transform.localScale = Vector3.one;

    }
    IEnumerator RemoveAfter(float time)
    {
        stack.Remove(this);
        if (transform.parent != null)
        {
            transform.parent = null;
        }
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Obstacle")))
        {
            
            StartCoroutine(RemoveAfter(2f));
        }
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Lava")))
        {

            Remove();
        }
    }

}
