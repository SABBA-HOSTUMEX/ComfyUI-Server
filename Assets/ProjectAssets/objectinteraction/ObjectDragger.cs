using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDragger : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // 將物件的世界座標轉換為螢幕座標
        screenPoint = mainCamera.WorldToScreenPoint(transform.position);

        // 計算觸控點與物件中心的偏移量
        offset = transform.position - GetWorldPosition();
    }

    void OnMouseDrag()
    {
        // 當手指拖動時，更新物件位置
        transform.position = GetWorldPosition() + offset;
    }

    Vector3 GetWorldPosition()
    {
        // 將螢幕座標轉換為世界座標
        Vector3 mousePosition = Input.GetTouch(0).position;
        mousePosition.z = screenPoint.z;
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
