using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [HideInInspector] public Rigidbody rigidbody;

    [SerializeField] private VoidEventSO DeathEvent;
    [SerializeField] private IntEventSO VictoryEvent;
    [SerializeField] private VoidEventSO StartLevelEvent;
    [SerializeField] private TransformEventSO PlayerTransformAnchor;
    private Animator _animator;

    private int finishMultiplier = 0;

    const string isGroundedBool = "isGrounded";
    const string victoryTrigger = "Victory";
    const string defeatTrigger = "Defeat";
    const string walkTrigger = "Walk";
    const string surfTrigger = "Surf";
    private bool isGrounded = false;
    private bool isDead = false;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        
        _animator.Rebind();
    }
    private void OnEnable()
    {
        StartLevelEvent.OnEventRaised += LevelStarted;
    }
    private void OnDisable()
    {
        StartLevelEvent.OnEventRaised -= LevelStarted;
    }

    private void LevelStarted()
    {
        _animator.SetTrigger(surfTrigger);
    }

    private void FixedUpdate()
    {
        if (rigidbody.velocity.y < -5)
        {
            SetGroundedState(false);
        } else
        {
            SetGroundedState(true);
        }
    }
    private void SetGroundedState(bool value)
    {
        if (isGrounded != value)
        {
            isGrounded = value;
            _animator.SetBool(isGroundedBool, value);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead)
        {
            return;
        }

        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")) || collision.gameObject.layer.Equals(LayerMask.NameToLayer("Wall")) || collision.gameObject.layer.Equals(LayerMask.NameToLayer("Lava")))
        {
            if (finishMultiplier > 0)
            {
                VictoryEvent.RaiseEvent(finishMultiplier);
                PlayerTransformAnchor.RaiseEvent(transform);
                rigidbody.detectCollisions = false;
                rigidbody.isKinematic = true;
                finishMultiplier = 0;
                _animator.SetTrigger(victoryTrigger);
                isDead = true;
                return;
            }
            else
            {
                isDead = true;
                DeathEvent.RaiseEvent();
                rigidbody.detectCollisions = false;
                rigidbody.isKinematic = true; 
                _animator.SetTrigger(defeatTrigger);

            }


        }
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Finish")))
        {
            var finish = collision.gameObject.GetComponent<FinishZone>();
            finishMultiplier = finish.multiplier;
            if (finishMultiplier == 20)
            {
                _animator.SetTrigger(walkTrigger);
                PlayerTransformAnchor.RaiseEvent(transform);
                VictoryEvent.RaiseEvent(finishMultiplier);
                
            }
        }
    }
}
