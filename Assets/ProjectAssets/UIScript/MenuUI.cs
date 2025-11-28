using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUI : MonoBehaviour
{
    public Camera camera;
    ////////// workflow ui //////////
    public Canvas TextToImagePromptUI;
    public Canvas KSamplerUI;
    public Canvas EmptyLatentImageUI;
    public Canvas CheckpointLoaderSimpleUI;
    ////////// workflow ui //////////
    public Button txt2img;
    public Button Homepage;
    public TMP_Text MenuUItxt;
    public GameObject buttonlist;
    public GameObject Changeworkflowui_txt2img;
    public GameObject TextToImage;
    // Start is called before the first frame update
    void Start()
    {
        txt2img.onClick.AddListener(Text2Image);
        Homepage.onClick.AddListener(SetHomepage);
        SetHomepage();
    }
    // Update is called once per frame
    private void SetHomepage()
    {
        ////////// main Scene object //////////
        camera.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;
        Homepage.gameObject.SetActive(false);     
        Changeworkflowui_txt2img.gameObject.SetActive(true);
        ////////// main Scene object //////////

        ////////// menu ui //////////
        MenuUItxt.gameObject.SetActive(true);
        buttonlist.SetActive(true);
        ////////// menu ui //////////
        TextToImage.SetActive(true);
    }
    private void SetScene()
    {
        ////////// main Scene object //////////
        camera.clearFlags = CameraClearFlags.Skybox;
        Homepage.gameObject.SetActive(true);
        
        ////////// main Scene object //////////
        
        
        ////////// workflow ui //////////
        TextToImagePromptUI.gameObject.SetActive(false);
        KSamplerUI.gameObject.SetActive(false);
        EmptyLatentImageUI.gameObject.SetActive(false);
        CheckpointLoaderSimpleUI.gameObject.SetActive(true);
        ////////// workflow ui //////////
        

        ////////// menu ui //////////
        MenuUItxt.gameObject.SetActive(false);
        buttonlist.SetActive(false);
        ////////// menu ui //////////
    }

    private void Text2Image()
    {
        ////////// Text2Image Scene object //////////
        TextToImage.SetActive(true);
        Changeworkflowui_txt2img.SetActive(true);
        Homepage.gameObject.SetActive(true);
        ////////// Text2Image Scene object //////////

        ////////// menu ui //////////
        MenuUItxt.gameObject.SetActive(false);
        buttonlist.SetActive(false);
        ////////// menu ui //////////          
    }   
}
