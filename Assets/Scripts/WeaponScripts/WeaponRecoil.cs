using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera PlayerCamera { get { return playerCam; } set { playerCam = value; } }

    [SerializeField] float verticalRecoil = 0.01f;
    [SerializeField] float duration;
    float time;
    Vector2 aimDir;


    private void Update()
    {
        if (time > 0)
        {
            playerCam.gameObject.transform.rotation = Quaternion.Euler(playerCam.transform.position.x - (verticalRecoil * Time.deltaTime) / duration, 0f, 0f);
            time -= Time.deltaTime;
        }
    }
    public void GenerateRecoil()
    {
        time = duration;
    }

}

