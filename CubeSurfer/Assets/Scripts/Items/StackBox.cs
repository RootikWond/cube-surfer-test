using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackBox : MonoBehaviour
{
    private Rigidbody rigidbody;
    private RigidbodyConstraints rigidbodyConstrains;
    [SerializeField] private BoxStackSO stack;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbodyConstrains = rigidbody.constraints;
    }
    private void OnEnable()
    {


        rigidbody.constraints = rigidbodyConstrains;
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

        
    }
    public void Remove()
    {
        VFXPool.PlayVFX(VFXPool.VFXType.BoxStack, transform.position);
        stack.Remove(this);
        if (transform.parent != null)
        {
            transform.parent = null;
        }
        gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
    }

    IEnumerator RemoveAfter(float time)
    {
        stack.Remove(this);
        if (transform.parent != null)
        {
            transform.parent = null;

            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
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
