using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera PlayerCamera { get { return playerCam; } set { playerCam = value; } }
    [SerializeField] float duration;

    CinemachineImpulseSource source;

    private void Start()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }
    private void Update()
    {
        // if (time > 0)
        // {
        //     playerCam.gameObject.transform.rotation = Quaternion.Euler(playerCam.transform.position.x - (verticalRecoil * Time.deltaTime) / duration, 0f, 0f);
        //     time -= Time.deltaTime;
        // }


    }
    public void GenerateRecoil()
    {
        // source.GenerateImpulse(Camera.main.transform.right);
        source.GenerateImpulse(Camera.main.transform.forward);
    }


}

