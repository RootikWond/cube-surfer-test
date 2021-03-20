using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameSceneSO _gameplayScene = default;

    [Header("Load Events")]
    [SerializeField] private LoadEventSO _loadLevelEvent = default;

    [Header("Listeners")]
    [SerializeField] private BoolEventSO _toggleLoadingScreen = default;
    [SerializeField] private VoidEventSO _onSceneReady = default;
    [SerializeField] private VoidEventSO _onSceneReload = default;

    private List<AsyncOperationHandle<SceneInstance>> _loadingOperationHandles = new List<AsyncOperationHandle<SceneInstance>>();
    private AsyncOperationHandle<SceneInstance> _gameplayLoadingHandle;
    //Scene loading requests parameters
    private GameSceneSO[] _scenesToLoad;
    private GameSceneSO[] _loadedScenes = new GameSceneSO[] { };
    private bool _showLoadingScreen;

    private SceneInstance _gameplaySceneInstance = new SceneInstance();
    private void OnEnable()
    {
        _loadLevelEvent.OnLoadingRequested += LoadLevel;
        _onSceneReload.OnEventRaised += ReloadLevel;

    }
    private void OnDisable()
    {
        _loadLevelEvent.OnLoadingRequested -= LoadLevel;
        _onSceneReload.OnEventRaised -= ReloadLevel;
    }
    private void LoadLevel(GameSceneSO[] levelsToLoad, bool showLoadingScreen)
    {
 
        _scenesToLoad = levelsToLoad;

        _showLoadingScreen = showLoadingScreen;

        if (_gameplaySceneInstance.Scene == null || !_gameplaySceneInstance.Scene.isLoaded)
        {

            StartCoroutine(ProcessGameplaySceneLoading(levelsToLoad, showLoadingScreen));
        }
        else
        {

            UnloadPreviousScenes();
        }
 
    }
    private void ReloadLevel()
    {
  
        LoadLevel(_loadedScenes, true);
    }
    
    private IEnumerator ProcessGameplaySceneLoading(GameSceneSO[] levelsToLoad, bool showLoadingScreen)
    {

        _gameplayLoadingHandle = _gameplayScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        while (_gameplayLoadingHandle.Status != AsyncOperationStatus.Succeeded)
        {
            yield return null;
        }
        _gameplaySceneInstance = _gameplayLoadingHandle.Result;

        UnloadPreviousScenes();
    }
  
    private void UnloadPreviousScenes()
    {

        for (int i = 0; i < _loadedScenes.Length; i++)
        {

            _loadedScenes[i].sceneReference.UnLoadScene();
            _loadedScenes[i].sceneReference.ReleaseAsset();
        }
       
        LoadNewScenes();
    }
    private void LoadNewScenes()
    {

        if (_showLoadingScreen)
        {
            _toggleLoadingScreen.RaiseEvent(true);
        }
        _loadingOperationHandles.Clear();

        for (int i = 0; i < _scenesToLoad.Length; i++)
        {

            _loadingOperationHandles.Add(_scenesToLoad[i].sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true, 0));
        }

        StartCoroutine(LoadingProcess());
    }
    private IEnumerator LoadingProcess()
    {
        bool done = _loadingOperationHandles.Count == 0;

        while (!done)
        {
            for (int i = 0; i < _loadingOperationHandles.Count; i++)
            {
                if (_loadingOperationHandles[i].Status != AsyncOperationStatus.Succeeded)
                {
                    break;
                }
                else
                {
                    done = true;
                }
            }
            yield return null;
        }

        //Save loaded scenes (to be unloaded at next load request)
        _loadedScenes = _scenesToLoad;
        SetActiveScene();
        if (_showLoadingScreen)
        {
            _toggleLoadingScreen.RaiseEvent(false);
        }

    }
    private void SetActiveScene()
    {
        Scene s = ((SceneInstance)_loadingOperationHandles[0].Result).Scene;
        SceneManager.SetActiveScene(s);
        //Run Spawn system to spawn the Player
        _onSceneReady.RaiseEvent();
    }
}
