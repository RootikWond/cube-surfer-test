using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

//Load all additional(persistant) scenes to test level
public class EditorLevelTest : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private GameSceneSO _thisSceneSO = default;
    [SerializeField] private GameSceneSO _persistentManagersSO = default;
    [SerializeField] private AssetReference _loadSceneEvent = default;

    void Start()
    {
        if (!SceneManager.GetSceneByName(_persistentManagersSO.sceneReference.editorAsset.name).isLoaded)
        {
            _persistentManagersSO.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
        }
    }

    private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
    {
        _loadSceneEvent.LoadAssetAsync<LoadEventSO>().Completed += ReloadScene;
    }

    private void ReloadScene(AsyncOperationHandle<LoadEventSO> obj)
    {
        LoadEventSO loadEvent = (LoadEventSO)_loadSceneEvent.Asset;
        loadEvent.RaiseEvent(new GameSceneSO[] { _thisSceneSO });
        SceneManager.UnloadSceneAsync(_thisSceneSO.sceneReference.editorAsset.name);
    }
#endif

}