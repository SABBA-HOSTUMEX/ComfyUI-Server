using UnityEngine;
using System;
using UnityEngine.Android;
using System.IO;
public class AndroidPermission : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetDCIMPath();
        CheckAndRequestPermission();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private string GetDCIMPath()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                // 直接使用 DCIM 資料夾路徑               
                string dcimPath = "/storage/emulated/0/DCIM";

                // 創建應用程式專屬資料夾
                string appFolder = Path.Combine(dcimPath, "FileFolder");
                Directory.CreateDirectory(appFolder);
                return appFolder;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Error getting DCIM path: {e.Message}");
                return Application.persistentDataPath;
            }
        }

        // 如果不是 Android 平台，回傳原本的 persistentDataPath
        return Application.persistentDataPath;
    }
    private void CheckAndRequestPermission()
    {
        // 检查是否已经有权限
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            // 请求权限
            string[] permissions = {
                Permission.ExternalStorageWrite,
                Permission.ExternalStorageRead
            };

            // 请求权限
            Permission.RequestUserPermissions(permissions);
        }
    }
}
