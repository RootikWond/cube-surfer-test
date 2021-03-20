using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public Camera mainCamera;
	public CinemachineVirtualCamera playerCameraCM;
	public CinemachineVirtualCamera dollyCameraCM;
	public Transform cameraRotationTrack;
	public AudioSource finishMusic;
	[SerializeField] private TransformEventSO PlayerTransformAnchor;

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
	}

	private void OnDisable()
	{
		PlayerTransformAnchor.OnEventRaised -= SetupCameraOnTarget;
	}

	private void SetupCameraOnTarget(Transform value)
	{
		SetupPlayerVirtualCamera(value);
		dollyCameraCM.LookAt = value;
		dollyCameraCM.Priority = 60;
		cameraRotationTrack.position = value.position;
		finishMusic.Play();
		
	}
	IEnumerator IncreseMusicVolume()
    {
		float time = 0;
		while(time < 2f)
        {
			time += Time.deltaTime;
			finishMusic.volume = Mathf.Lerp(finishMusic.volume, 1.0f, time / 2f);
			yield return new WaitForEndOfFrame();
        }
    }
}
