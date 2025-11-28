using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextToImageworkflowui : MonoBehaviour
{
    public Button CheckPointLoaderBTN;
    public Button EmptyLatentImageBTN;
    public Button KSamplerBTN;
    public Button TextToImagePromptBTN;
    public Canvas CheckpointLoaderSimpleUI;
    public Canvas EmptyLatentImageUI;
    public Canvas KSamplerUI;
    public Canvas TextToImagePromptUI;
    // Start is called before the first frame update
    void Start()
    {
        CheckPointLoaderBTN.onClick.AddListener(DisplayCheckpointLoaderSimpleUI);
        EmptyLatentImageBTN.onClick.AddListener(DisplayEmptyLatentImageUI);
        KSamplerBTN.onClick.AddListener(DisplayKSamplerUI);
        TextToImagePromptBTN.onClick.AddListener(DisplayPromptUI);
    }
    // Update is called once per frame
    public void DisplayCheckpointLoaderSimpleUI()
    {
        CheckpointLoaderSimpleUI.gameObject.SetActive(true);
        EmptyLatentImageUI.gameObject.SetActive(false);
        KSamplerUI.gameObject.SetActive(false);
        TextToImagePromptUI.gameObject.SetActive(false);
    }
    public void DisplayEmptyLatentImageUI()
    {
        EmptyLatentImageUI.gameObject.SetActive(true);
        CheckpointLoaderSimpleUI.gameObject.SetActive(false);
        KSamplerUI.gameObject.SetActive(false);
        TextToImagePromptUI.gameObject.SetActive(false);
    }
    public void DisplayKSamplerUI()
    {
        KSamplerUI.gameObject.SetActive(true);
        EmptyLatentImageUI.gameObject.SetActive(false);
        CheckpointLoaderSimpleUI.gameObject.SetActive(false);
        TextToImagePromptUI.gameObject.SetActive(false);
    }
    public void DisplayPromptUI()
    {
        TextToImagePromptUI.gameObject.SetActive(true);
        CheckpointLoaderSimpleUI.gameObject.SetActive(false);
        EmptyLatentImageUI.gameObject.SetActive(false);
        KSamplerUI.gameObject.SetActive(false);
    }
}
