using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CameraRotationController : MonoBehaviour
{
    public float rotationSpeed = 0.3f;
    public static bool edit;
    public static bool editPosandScal;
    public static bool editRotation;

    public Button editbutton;
    public Canvas ConnectUI;
    public GameObject WorkFlowUI;
    public GameObject ChangeWorkFlowUI;
    public TMP_Dropdown myDropdown;
    public Button HomePage;
    void Start()
    {
        myDropdown.ClearOptions();
        edit = false;
        editPosandScal = false;
        editRotation = false;
        editbutton.onClick.AddListener(Edit);
        List<string> options = new List<string>
        {
            "位置及縮放",
            "旋轉"            
        };
        myDropdown.AddOptions(options);
        myDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        myDropdown.gameObject.SetActive(false);
    }
    void OnDropdownValueChanged(int index)
    {
        switch (index)
        {
            case 0:
                editPosandScal = editPosandScal switch { true => false, false => true }; // lambda
                editRotation = editRotation switch { true => false, false => true }; // lambda
                break;

            case 1:
                editPosandScal = editPosandScal switch { true => false, false => true }; // lambda
                editRotation = editRotation switch { true => false, false => true }; // lambda
                break;
        }
    }
    void Update()
    {
        if (!edit)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 deltaPosition = touch.deltaPosition;
                    transform.Rotate(Vector3.up * deltaPosition.x * rotationSpeed, Space.World);
                    transform.Rotate(Vector3.right * -deltaPosition.y * rotationSpeed, Space.Self);
                }
            }
        }
    }
    private void Edit()
    {
        edit = edit switch { true => false, false => true }; // lambda
        editPosandScal = editPosandScal switch { true => false, false => true }; // lambda
        myDropdown.gameObject.SetActive(!myDropdown.gameObject.activeSelf);
        HomePage.gameObject.SetActive(!HomePage.gameObject.activeSelf);
        // edit = edit ? false : true;//三元運算子       
        //三元運算及實現切換true false的功能
        ConnectUI.gameObject.SetActive(ConnectUI.gameObject.activeSelf ? false : true);//三元運算子
        WorkFlowUI.gameObject.SetActive(WorkFlowUI.gameObject.activeSelf ? false : true);//三元運算子 
        ChangeWorkFlowUI.gameObject.SetActive(ChangeWorkFlowUI.gameObject.activeSelf ? false : true);//三元運算子 

        // if (edit)
        // {
        //     edit = false;
        //     editPosandScal = false;
        //     editRotation = false;
        //     myDropdown.gameObject.SetActive(myDropdown.gameObject.activeSelf ? false : true);
        //     ConnectUI.gameObject.SetActive(ConnectUI.gameObject.activeSelf ? false : true);//三元運算子
        //     PromptUI.gameObject.SetActive(PromptUI.gameObject.activeSelf ? false : true);//三元運算子 
        // }
    }
}
