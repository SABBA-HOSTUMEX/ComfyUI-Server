using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TouchController : MonoBehaviour
{
    // 拖動速度
    public float dragSpeed = 0.1f;
    public Camera mainCamera;
    // 縮放靈敏度
    public float zoomSpeed = 0.1f;
    // 最小和最大縮放限制
    public float minZoom = 1f;
    public float maxZoom = 10f;

    //-----------------------------------

    private float dist;
    private bool dragging = false;
    private Vector3 offset;
    private Transform toDrag;
    private Transform lasttouchobject;
    void Start()
    {
        // 如果沒有在Inspector中指定，則預設使用主攝影機
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    void Update()
    {
        if (transform.position.y < -5) transform.position = new Vector3(transform.position.x, 5, transform.position.z);
        if (CameraRotationController.edit)
        {
            // 雙指縮放
            if (Input.touchCount == 2 && CameraRotationController.editPosandScal == true)
            {
                if (lasttouchobject != null)
                    HandlePinchZoom(toDrag);
            }
            //Raycast偵測物件並拖拉
            Vector3 v3;
            if (Input.touchCount != 1)
            {
                dragging = false;
                return;
            }
            Touch touch = Input.touches[0];
            Vector3 pos = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(pos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag != "Ground")
                    {
                        toDrag = hit.transform;
                        lasttouchobject = hit.transform;
                        dist = hit.transform.position.z - Camera.main.transform.position.z;
                        v3 = new Vector3(pos.x, pos.y, dist);
                        v3 = Camera.main.ScreenToWorldPoint(v3);
                        offset = toDrag.position - v3;
                        dragging = true;
                    }
                }
            }
            if (dragging && touch.phase == TouchPhase.Moved)
            {
                v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
                v3 = Camera.main.ScreenToWorldPoint(v3);
                toDrag.position = v3 + offset;
            }
            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                dragging = false;
            }
        }
        //物件旋轉
        if (!CameraRotationController.editPosandScal && CameraRotationController.editRotation)
        {
            if (Input.touchCount == 1)
            {
                Touch screenTouch = Input.GetTouch(0);
                if (screenTouch.phase == TouchPhase.Moved)
                {
                    toDrag.Rotate(0f, screenTouch.deltaPosition.x, 0f);
                }
            }
        }
    }
    void HandlePinchZoom(Transform transform)
    {
        // 取得兩個觸摸點
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        // 前一幀的觸摸點位置
        Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        // 計算前一幀和當前幀的觸摸距離
        float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
        float touchDeltaMag = (touch0.position - touch1.position).magnitude;

        // 計算縮放差值
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        // 縮放相機或物體
        float newScale = transform.localScale.x - deltaMagnitudeDiff * zoomSpeed;
        newScale = Mathf.Clamp(newScale, minZoom, maxZoom);

        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
