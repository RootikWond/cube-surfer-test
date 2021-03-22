using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
	public Camera mainCamera;
	public CinemachineVirtualCamera playerCameraCM;
	public CinemachineVirtualCamera dollyCameraCM;
	
	public Transform cameraRotationTrack;
	[SerializeField] private Volume wideVolume;
	public AudioSource finishMusic;
	[SerializeField] private TransformEventSO PlayerTransformAnchor;
	[SerializeField] private IntEventSO VictoryEvent;

	private bool _cameraMovementLock = false;
	
	public void SetupPlayerVirtualCamera(Transform target)
	{
		playerCameraCM.Follow = target;
		playerCameraCM.LookAt = target;
		playerCameraCM.OnTargetObjectWarped(target, target.position - playerCameraCM.transform.position - Vector3.forward);
	}

	private void OnEnable()
	{
		PlayerTransformAnchor.OnEventRaised += SetupCameraOnTarget;
		VictoryEvent.OnEventRaised += Victory;
	}

	private void OnDisable()
	{
		PlayerTransformAnchor.OnEventRaised -= SetupCameraOnTarget;
		VictoryEvent.OnEventRaised -= Victory;
	}
	private void Victory(int value)
    {
		if (value >= 20)
        {
			finishMusic.Play();
			wideVolume.enabled = true;
		}
		
	}
	private void SetupCameraOnTarget(Transform value)
	{
		SetupPlayerVirtualCamera(value);
		dollyCameraCM.LookAt = value;
		dollyCameraCM.Priority = 60;
		cameraRotationTrack.position = value.position;
		
		
	}
	
}
