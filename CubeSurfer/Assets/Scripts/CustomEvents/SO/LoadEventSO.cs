using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Load Event")]
public class LoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO[], bool> OnLoadingRequested;

    public void RaiseEvent(GameSceneSO[] levelsToLoad, bool showLoadingScreen = false)
    {
        if (OnLoadingRequested != null)
        {
            OnLoadingRequested.Invoke(levelsToLoad, showLoadingScreen);
            
        }
        else
        {
            Debug.LogWarning("Check why there is no SceneLoader already present. ");
        }
    }
}

