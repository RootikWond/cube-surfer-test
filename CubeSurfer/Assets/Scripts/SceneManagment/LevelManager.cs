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
	//On level loaded actions
	private void LevelLoaded()
	{
		
	}

}