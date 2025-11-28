using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectStartupStatus : MonoBehaviour
{
    public Camera camera;
    public Canvas EditUI;
    public GameObject GroundPlane;
    public GameObject ChangeUIObject;
    public GameObject[] HideInAndroidDevice;
    public GameObject[] HideInPCServer;

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultStatus();
    }
    private void SetDefaultStatus()
    {
        EditUI.gameObject.SetActive(false);
        GroundPlane.SetActive(false);
        ChangeUIObject.gameObject.SetActive(false);
        camera.clearFlags = CameraClearFlags.SolidColor;
        if(Application.platform == RuntimePlatform.Android)
        {
            GameObjectHideInAndroidDevice();
        }
        if(Application.platform != RuntimePlatform.Android)
        {
            GameObjectHideInPCServer();
        }
    }
    private void GameObjectHideInAndroidDevice()
    {
        foreach(GameObject gbj in HideInAndroidDevice)
        {
            gbj.SetActive(false);
        }
    }
    private void GameObjectHideInPCServer()
    {
        foreach (GameObject gbj in HideInPCServer)
        {
            gbj.SetActive(false);
        }
    }
    // Update is called once per frame
}
