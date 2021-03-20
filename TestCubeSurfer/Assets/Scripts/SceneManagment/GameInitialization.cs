using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class GameInitialization : MonoBehaviour
{

    [Header("Game manager scene")]
    [SerializeField] private GameSceneSO _persistentManagersScene = default;

    [Header("Loading settings")]
    [SerializeField] private GameSceneSO[] _levelToLoad = default;

    [Header("Events")]
    [SerializeField] private AssetReference _levelLoadEvent = default;
    void Start()
    {
        _persistentManagersScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEvent;
    }
    private void LoadEvent(AsyncOperationHandle<SceneInstance> obj)
    {
        _levelLoadEvent.LoadAssetAsync<LoadEventSO>().Completed += LoadGameScene;
    }

    private void LoadGameScene(AsyncOperationHandle<LoadEventSO> obj)
    {
        LoadEventSO loadEventSO = (LoadEventSO)_levelLoadEvent.Asset;
        
        loadEventSO.RaiseEvent(_levelToLoad,true);
      
        SceneManager.UnloadSceneAsync(0);
    }
}
