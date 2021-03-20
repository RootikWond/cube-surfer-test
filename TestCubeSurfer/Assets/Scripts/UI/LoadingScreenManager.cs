using UnityEngine;
public class LoadingScreenManager : MonoBehaviour
{
	[Header("Event")]
	[SerializeField] private BoolEventSO _ToggleLoadingScreen = default;

	[Header("Loading screen ")]
	public GameObject loadingScreen;

	private void OnEnable()
	{
		if (_ToggleLoadingScreen != null)
		{
			_ToggleLoadingScreen.OnEventRaised += ToggleLoadingScreen;
		}
	}

	private void OnDisable()
	{
		if (_ToggleLoadingScreen != null)
		{
			_ToggleLoadingScreen.OnEventRaised -= ToggleLoadingScreen;
		}
	}

	private void ToggleLoadingScreen(bool state)
	{
		loadingScreen.SetActive(state);
	}
}
