using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private InputManager _inputManager;
    private Camera _mainCamera;

    [SerializeField] private PlayerBody playerBody;
    [SerializeField] private CinemachineDollyCart playerCart;

    private bool touchMove;
    private bool touchSwipe;
    private bool touchBegan;


    [SerializeField] private float speed = 2f;
    private Vector2 startTouchPosition;
    private double maxSwipeDuration = 0.15f;
    private float minSwipeDistance = 50f;
    private float gestureTime = 0f;

    [Header("Events")]
    public ItemEventSO CoinCollect = default;

    [Header("Listeners")]
    public BoolEventSO StackBoxChange = default;
    public ItemEventSO BoxCollect = default;
    public VoidEventSO DeathEvent = default;
    public VoidEventSO StartLevel = default;
    public IntEventSO LevelFinish = default;

    [Header("Stack")]
    public BoxStackSO Stack = default;


    private bool levelRunning = false;

    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        _mainCamera = Camera.main;
        EnhancedTouchSupport.Enable();
    }
    private void OnEnable()
    {
        _inputManager.Pressed += OnTouchStarted;
        _inputManager.Dragged += OnTouchMove;
        _inputManager.Released += OnTouchEnded;
        StackBoxChange.OnEventRaised += UpdateBoxStack;
        DeathEvent.OnEventRaised += OnPlayerDeath;
        LevelFinish.OnEventRaised += OnPlayerWin; 
}
    private void OnDisable()
    {
        _inputManager.Pressed -= OnTouchStarted;
        _inputManager.Dragged -= OnTouchMove;
        _inputManager.Released -= OnTouchEnded;
        StackBoxChange.OnEventRaised -= UpdateBoxStack;
        DeathEvent.OnEventRaised -= OnPlayerDeath;
        LevelFinish.OnEventRaised -= OnPlayerWin;
    }

    //Test to add remove boxes into stack
    private void UpdateBoxStack(bool value)
    {
        if (value)
        {
            var box = StackBoxPool.Instance.SpawnBox(playerBody.transform.localPosition, transform);
            Stack.Add(box);
            speed += 1;

        } 
        else
        {
            if (Stack.Items.Count > 0)
            {
                Stack.RemoveLast();
            }
            speed -= 1;
        }
    }
    
    private void OnPlayerWin(int value)
    {
        _inputManager.Controls.Disable();
        
    }
    private void OnPlayerDeath()
    {
        _inputManager.Controls.Disable();
        playerCart.m_Speed = 0;
    }
     
    private void OnTouchMove(Vector2 deltaPosition, double time)
    {
       
        if (touchMove)
        {
            var newPos = transform.localPosition + new Vector3(deltaPosition.x, 0, 0) * Time.deltaTime * speed;
            transform.localPosition = new Vector3(Mathf.Clamp(newPos.x, -2, 2), transform.localPosition.y, transform.localPosition.z);
            /*
            var xOffset = transform.localPosition;
            xOffset.x = Mathf.Clamp(transform.localPosition.x + deltaPosition.normalized.x * Time.deltaTime*speed, -2, 2);
            transform.localPosition = new Vector3(xOffset.x, transform.localPosition.y, transform.localPosition.z);
            */
        }
    
        
    }
    private void OnTouchStarted(Vector2 position, double time)
    {
        if (!levelRunning)
        {
            levelRunning = true;
            playerCart.m_Speed = 8;
            StartLevel.RaiseEvent();
        }
        touchBegan = true;
        touchMove = false;
        //touchSwipe = false;
        startTouchPosition = position;
    }
    
    private void OnTouchEnded(Vector2 position, double time)
    {
        
        touchMove = false;
        Vector3 direction = Vector3.zero;
        if (IsValidSwipe(startTouchPosition, position, time, out direction))
        {
            //touchSwipe = true;

           StartCoroutine(Swipe(direction));
                    
        }
        touchBegan = false;
        gestureTime = 0;


    }

    private bool IsValidSwipe(Vector2 start, Vector2 end, double duration, out Vector3 swipeDirection)
    {
        swipeDirection = Vector3.zero;
        if(Vector2.Distance(start, end) >= minSwipeDistance && duration <= maxSwipeDuration)
        {
            Vector2 dir = (end - start).normalized;
            if (dir.x > 0)
            {
                swipeDirection = Vector3.right;
            }
            else if (dir.x < 0)
            {
                swipeDirection = Vector3.left;
            }
            return true;
        }
        return false;
    }
    
    private void Update()
    {
        if (touchBegan)
        {

            
            gestureTime += Time.deltaTime;
            if (gestureTime > maxSwipeDuration && !touchSwipe)
            {
                touchMove = true;
            }
          
          
        }
        /*
        if (Touch.activeFingers.Count == 1)
        {
            Touch activeTouch = Touch.activeFingers[0].currentTouch;
            if (activeTouch.phase == TouchPhase.Moved)
            {
                Debug.Log("Delta = "+ activeTouch.delta);
                var scp = _mainCamera.ScreenToViewportPoint(activeTouch.screenPosition);
               
                var loc = transform.InverseTransformPoint(scp);
               
                //transform.localPosition += (direction * Time.deltaTime * speed);
              
                transform.localPosition = new Vector3(
                    transform.localPosition.x + activeTouch.delta.normalized.x * Time.deltaTime * speed, 
                    transform.localPosition.y, 
                    transform.localPosition.z);
                
            }
        }
        */

       
    }

    IEnumerator Swipe(Vector3 direction)
    {
        var newPosition = transform.localPosition + direction;
        float elpsedTime = 0f;
        while(elpsedTime < 0.2f)
        {
            elpsedTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, elpsedTime/0.2f);
            yield return null;
        }
        transform.localPosition = newPosition;
       
    }


    private void AddBox(Item item)
    {
        Vector3 topPosition = new Vector3(0,(float)Stack.Items.Count,0);
      
        //player jump
        playerBody.rigidbody.isKinematic = true;
        playerBody.transform.localPosition = new Vector3(0, playerBody.transform.localPosition.y + item.amount*2f, 0);
        for (int i = 0; i < item.amount; i++)
        {
            topPosition.y += i+1;
            var box = StackBoxPool.Instance.SpawnBox(topPosition, transform);
            Stack.Add(box);
        }
        playerBody.rigidbody.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.gameObject.GetComponent<Item>();
        if (item != null)
        {
            switch (item.type)
            {
                case Item.ItemType.Box:
                    //BoxCollect.RaiseEvent(item);
                  
                    AddBox(item);
                    item.OnInteraction();


                    break;
                case Item.ItemType.Coin:
                    
                    var position = _mainCamera.WorldToScreenPoint(item.transform.position);
                    CoinCollect.RaiseEvent(item, position);
                    item.OnInteraction();
                    break;
                case Item.ItemType.Bonus:
                    break;
            }
            
        }
    }

}
