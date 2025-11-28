using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointLoaderSimpleUI : MonoBehaviour
{
    // 引用 Dropdown 和要顯示文字的 Text 元件
    [SerializeField] private Dropdown dropdown;
    // 儲存不同選項對應的文字
    void Start()
    {
        // 確保有參考到必要的元件
        if (dropdown == null)
        {
            dropdown = GetComponent<Dropdown>();
        }
        // 註冊 Dropdown 的值改變事件
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Dropdown 值改變時的處理函數
    private void OnDropdownValueChanged(int index)
    {
      
    }
}
