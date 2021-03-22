using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.EnhancedTouch;
//using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
//using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Cinemachine;
using UnityEngine.EventSystems;
//using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private Camera _mainCamera;

    [SerializeField] private PlayerBody playerBody;
    [SerializeField] private CinemachineDollyCart playerCart;

    private bool inputsEnabled;

    [SerializeField] private float speed = 2f;
    private double maxSwipeDuration = 0.15f;
    private float minSwipeDistance = 50f;
    private float gestureTime = 0f;

    [Header("Events")]
    public ItemEventSO CoinCollect = default;
    public VoidEventSO StartLevel = default;

    [Header("Listeners")]
    public BoolEventSO StackBoxChange = default;
    public ItemEventSO BoxCollect = default;
    public VoidEventSO DeathEvent = default;
    
    public IntEventSO LevelFinish = default;

    [Header("Stack")]
    public BoxStackSO Stack = default;


    private bool levelStarted = false;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        StackBoxChange.OnEventRaised += UpdateBoxStack;
        DeathEvent.OnEventRaised += OnPlayerDeath;
        LevelFinish.OnEventRaised += OnPlayerWin;
    }
    private void OnDisable()
    {

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
        inputsEnabled = true;
        playerCart.m_Speed = 0;

    }
    private void OnPlayerDeath()
    {

        inputsEnabled = false;
        playerCart.m_Speed = 0;
    }

    private bool IsValidSwipe(Vector2 start, Vector2 end, double duration, out Vector3 swipeDirection)
    {
        swipeDirection = Vector3.zero;
        if (Vector2.Distance(start, end) >= minSwipeDistance && duration <= maxSwipeDuration)
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

    Vector3 start;
    float lastX;
    private void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            start = Input.mousePosition;
            lastX = start.x;
            InterationBegan();
        }

        if (Input.GetMouseButton(0))
        {
            var current = Input.mousePosition;
             var delta = current.x - lastX;
            var target = transform.localPosition;
             target.x += (delta / Screen.width) * speed;
            target.x = Mathf.Clamp(target.x, -2, 2);
            target.y = 0;
            target.z = 0;
            transform.localPosition = target;

            lastX = current.x;

        }
       
        if (Input.GetMouseButtonUp(0))
        {
            InterationEnded();
        }
   */
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Debug.Log("UI");
                return;
            }

            if (touch.phase.Equals(UnityEngine.TouchPhase.Began))
            {
                start = touch.position;
                lastX = start.x;
                InterationBegan();
            }
            if (inputsEnabled)
            {
                var current = Input.mousePosition;
                var delta = current.x - lastX;
                var target = transform.localPosition;
                target.x += (delta / Screen.width) * speed;
                target.x = Mathf.Clamp(target.x, -2, 2);
                target.y = 0;
                target.z = 0;
                transform.localPosition = target;

                lastX = current.x;
            }




            /*
            if (touch.phase.Equals(UnityEngine.TouchPhase.Canceled))
            {
                //Select closed line

            }
            */
        }

    }


    private void InterationBegan()
    {
        if (!levelStarted)
        {
            levelStarted = true;
            inputsEnabled = true;
            playerCart.m_Speed = 10;
            StartLevel.RaiseEvent();
        }

    }

    /*
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
    */

    private void AddBox(Item item)
    {
      
        var lastBoxPosition = Stack.LastBox().transform.localPosition;

        playerBody.rigidbody.isKinematic = true;

        for (int i = 0; i < item.amount; i++)
        {
            lastBoxPosition.y += i+1;
            var box = StackBoxPool.Instance.SpawnBox(lastBoxPosition, transform);
            Stack.Add(box);
        }
  
        playerBody.transform.localPosition = new Vector3(0, lastBoxPosition.y +0.6f, 0);
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
