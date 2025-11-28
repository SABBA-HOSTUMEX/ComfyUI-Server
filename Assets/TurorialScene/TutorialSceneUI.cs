using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TutorialSceneUI : MonoBehaviour
{
    public Button OneButton;
    public Button TwoButton;
    public Button ThreeButton;
    public Button Image2Button;
    public Button Image3Button;
    public GameObject one;
    public GameObject two;
    public GameObject three;
    public GameObject[] Image2Group;
    public GameObject[] Image3Group;
    private int Image2page;
    private int Image3page;
    public Button ChangeScene;

    // Start is called before the first frame update
    void Start()
    {
        ChangeScene.onClick.AddListener(SwitchScene);
        one.SetActive(false);
        two.SetActive(false);
        three.SetActive(false);
        OneButton.onClick.AddListener(DisplayOne);
        TwoButton.onClick.AddListener(DisplayTwo);
        ThreeButton.onClick.AddListener(DisplayThree);
        Image2Button.onClick.AddListener(Image2Display);
        Image3Button.onClick.AddListener(Image3Display);
        Image2page = 0;
        Image3page = 0;
    }
    public void SwitchScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Image2Display()
    {
        Image2page++;
        if (Image2page > 2)
        {
            Image2page = 0;
        }
        switch (Image2page)
        {
            case 0:
                Image2Group[0].SetActive(true);
                Image2Group[1].SetActive(false);
                Image2Group[2].SetActive(false);
                break;
            case 1:
                Image2Group[0].SetActive(false);
                Image2Group[1].SetActive(true);
                Image2Group[2].SetActive(false);
                break;
            case 2:
                Image2Group[0].SetActive(false); 
                Image2Group[1].SetActive(false);
                Image2Group[2].SetActive(true);
                break;
        }
    }
    public void Image3Display()
    {
        Image3page++;
        if (Image3page > 2)
        {
            Image3page = 0;
        }
        int start = Image3page * 2; // start = 0
        int end = start + 2; // end = 2
        for (int i = 0; i < Image3Group.Length; i++) // i = 2
        {
            Image3Group[i].SetActive(i >= start && i < end);
        }
    }
    public void UpdateCanvas()
    {
        Canvas.ForceUpdateCanvases();
    }
    public void DisplayOne()
    {
        one.SetActive(true);
        two.SetActive(false);
        three.SetActive(false);
        UpdateCanvas();
    }
    public void DisplayTwo()
    {
        one.SetActive(false);
        two.SetActive(true);
        three.SetActive(false);
        UpdateCanvas();
    }
    public void DisplayThree()
    {
        one.SetActive(false);
        two.SetActive(false);
        three.SetActive(true);
        UpdateCanvas();
    }
}
