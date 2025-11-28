using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmptyLatentImageUI : MonoBehaviour
{
    public TMP_InputField width;
    public TMP_InputField height;
    public Button EmptyLatentImageBTN;
    // Start is called before the first frame update
    void Start()
    {
        EmptyLatentImageBTN.onClick.AddListener(OnSaveData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSaveData()
    {

    }
}
