using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera playerCam;
    [SerializeField] Transform target;
    [SerializeField] float verticalRecoil = 0.01f;

    public CinemachineVirtualCamera PlayerCamera { get { return playerCam; } set { playerCam = value; } }

    // Get Cinemachine Aim Camera Component
    CinemachineComposer composerAim;

    private void Start()
    {
        composerAim = playerCam.GetCinemachineComponent<CinemachineComposer>();
        target = composerAim.LookAtTarget;
    }


    public void GenerateRecoil()
    {
        // playerCam.m_Lens


        Vector3 offset = playerCam.transform.position;
        offset.y += verticalRecoil;
        composerAim.m_TrackedObjectOffset = new Vector3(playerCam.transform.position.x, offset.y, playerCam.transform.position.z);

    }

}



