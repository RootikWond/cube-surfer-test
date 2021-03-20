using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{


	[Header("Spawn Point")]
	[SerializeField] private Transform _spawnPoint;

	[Header("Events")]
	[SerializeField] private VoidEventSO _OnSceneReady = default;
	[SerializeField] private VoidEventSO _OnLevelStarted = default;


	private void OnEnable()
	{
		if (_OnSceneReady != null)
		{
			_OnSceneReady.OnEventRaised += LevelLoaded;
		}
	}

	private void OnDisable()
	{
		if (_OnSceneReady != null)
		{
			_OnSceneReady.OnEventRaised -= LevelLoaded;
		}
	}
	//Spawn player after level loaded
	private void LevelLoaded()
	{
		Debug.Log("LevelManager > LevelLoaded");
		//PlayerController playerInstance = InstantiatePlayer(_playerPrefab, _spawnPoint);

		//_playerInstantiatedChannel.RaiseEvent(playerInstance.transform); // The CameraSystem will pick this up to frame the player
		//_playerTransformAnchor.transform = playerInstance.transform;
	}

}