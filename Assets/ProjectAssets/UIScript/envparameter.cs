using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class envparameter : MonoBehaviour
{
    private int choosemodel;
    public Text titlecheckpoint;
    public Text titlelanternimage;
    public Text titleksampler;
    public Text titleprompt;
    private string image_to_3D;
    private string text_to_3D = "文字生3D模型";
    private string text_to_image = "文字生圖片";
    // Start is called before the first frame update
    void Start()
    {
        choosemodel = 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setimg23d()
    {
        choosemodel = 2;
    }
    public void settxt23d()
    {
        choosemodel = 1;
        titlecheckpoint.text = "文字生3D模型";
        titleksampler.text = "文字生3D模型";
        titlelanternimage.text  = "文字生3D模型";
        titleprompt.text  = "文字生3D模型";
    }
    public void settxt2img()
    {
        choosemodel = 0;
        titlecheckpoint.text = "文字生圖片";
        titleksampler.text = "文字生圖片";
        titlelanternimage.text  = "文字生圖片";
        titleprompt.text  = "文字生圖片";
    }
    public void resetenv()
    {
        choosemodel = 4;
    }
}
