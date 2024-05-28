using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeforeStarting : MonoBehaviour
{
    [SerializeField] private CameraFollowing cameraFollowing;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private Image joyStick;

    void Start()
    {
        inGameUI.SetActive(false);
        joyStick.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraFollowing.isStarting)
        {
            inGameUI.SetActive(false);
            joyStick.gameObject.SetActive(false);
        }
        else
        {
            inGameUI.SetActive(true);
            joyStick.gameObject.SetActive(true);
        }
    }
}
