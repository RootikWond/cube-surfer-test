using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UISystemManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private RectTransform startScreen;
    [SerializeField] private RectTransform gameScreen;
    [SerializeField] private RectTransform winScreen;
    [SerializeField] private RectTransform defeatScreen;

    [Header("Test buttons")]
    public Button addBoxButton;
    public Button removeBoxButton;

    [Header("Coins counter")]
    public Text totalCoinsLabel;
    public Image coinsIcon;

    public Image coinsImagePrefab;

    
    [Header("Events")]
    public VoidEventSO LevelStart = default;
    public BoolEventSO OnBoxChange = default;
    public VoidEventSO LevelRestart = default;
    

    [Header("Listeners")]
    public VoidEventSO OnSceneReady = default;
    public ItemEventSO CoinCollect = default;
    public IntEventSO LevelFinish = default;
    public VoidEventSO LevelFailed = default;
    
    

    private int collected = 0;
    private int totalCollected = 0;
    private float animationSpeed = 1f;
    private void Awake()
    {
        Debug.Log("Update Label by saved collected coins");
    }
    private void OnEnable()
    {
        OnSceneReady.OnEventRaised += ResetScreens;
        CoinCollect.OnEventRaised += UpdateCoinsCounter;
        LevelFinish.OnEventRaised += MultiplyCoins;
        LevelFailed.OnEventRaised += ShowDefeatScreen;
        LevelStart.OnEventRaised += OnLevelStarted;
    }
    private void OnDisable()
    {
        OnSceneReady.OnEventRaised -= ResetScreens;
        CoinCollect.OnEventRaised -= UpdateCoinsCounter;
        LevelFinish.OnEventRaised -= MultiplyCoins;
        LevelFailed.OnEventRaised -= ShowDefeatScreen;
        LevelStart.OnEventRaised -= OnLevelStarted;
       
    }

    public Queue<Image> m_Pool = new Queue<Image>();
    private void Start()
    {
        if (coinsImagePrefab != null)
        {
            int size = 20;
            for (int i = 0; i < size; ++i)
            {
                Image c = Instantiate(coinsImagePrefab, transform);
                  c.gameObject.SetActive(false);
                m_Pool.Enqueue(c);
            }
        }
    }
    private Image GetCoinImage()
    {
        if (m_Pool.Count == 0)
        {
            return null;
        }

        var coin = m_Pool.Dequeue();

        coin.rectTransform.gameObject.SetActive(true);
        m_Pool.Enqueue(coin);
        return coin;
    }
    private void UpdateCoinsCounter(Item coin, Vector3 screenPosition)
    {
        collected += 1;
       
        totalCoinsLabel.text = (totalCollected + collected).ToString();
        CoinAnimation(screenPosition);
    }
    private void ResetScreens()
    {
        gameScreen.gameObject.SetActive(true);
        startScreen.gameObject.SetActive(true);
    }


    private void ShowDefeatScreen()
    {
        defeatScreen.gameObject.SetActive(true);
    }


    private void OnLevelStarted()
    {
        startScreen.gameObject.SetActive(false);
    }

    private void MultiplyCoins(int value)
    {
        collected *= value;
        int coins = 0;
        Int32.TryParse(totalCoinsLabel.text, out coins);
        totalCollected = collected;
        totalCoinsLabel.text = totalCollected.ToString();
        winScreen.gameObject.SetActive(true);
    }
    private void CoinAnimation(Vector3 position)
    {
       StartCoroutine(MoveCoin(position));
    }
    IEnumerator MoveCoin(Vector3 position)
    {
        var coin = GetCoinImage();
        coin.transform.position = position;
        float elapsedTime = 0;
        while(elapsedTime < 1)
        {
            elapsedTime += animationSpeed * Time.deltaTime;
            coin.transform.position = Vector3.Lerp(coin.transform.position, coinsIcon.transform.position, elapsedTime);
            yield return null;
        }
        coin.rectTransform.gameObject.SetActive(false);
        
        
    }

    public void RestartLevel()
    {
        collected = 0;
        totalCoinsLabel.text = totalCollected.ToString();
        gameScreen.gameObject.SetActive(false);
        startScreen.gameObject.SetActive(false);
        defeatScreen.gameObject.SetActive(false);
        winScreen.gameObject.SetActive(false);
        LevelRestart.OnEventRaised();
    }

    public void OnAddBox()
    {
        OnBoxChange?.OnEventRaised(true);
    }
    public void OnRemoveBox()
    {
        OnBoxChange?.OnEventRaised(false);
    }
}
