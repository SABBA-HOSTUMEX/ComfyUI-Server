using UnityEngine;
using System.IO;
using System.Threading;
using System;

public class LatentData : MonoBehaviour
{
    private FileSystemWatcher watcher;

    private string folderPath = @"C:\Users\admin\Desktop\ComfyUI_windows_portable\ComfyUI\output\LatentData";
    // Start is called before the first frame update
    void Start()
    {
        InitializeLatentFileWatcher();
    }
    private void InitializeLatentFileWatcher()
    {
        try
        {
            watcher = new FileSystemWatcher(folderPath);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.Created += OnConditioningFileCreated;
            watcher.EnableRaisingEvents = true;
            Debug.Log("LatentData Folder Found");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error initializing Conditioning Folder: {e.Message}");
        }
    }
    private void OnConditioningFileCreated(object sender, FileSystemEventArgs e)
    {
        string fileName = e.Name;
        string fullPath = e.FullPath;
        Debug.Log($"FileName : {fileName}  File Path : {fullPath}");

        bool isFileReady = false;
        int retryCount = 0;
        const int maxRetries = 60; // 增加總重試次數，因為我們需要更多時間來確認
        const int retryDelayMs = 500; // 每次檢查的間隔
        const int requiredSameSize = 10; // 需要連續10次相同大小

        long lastSize = 0;
        int sameSizeCount = 0;

        while (!isFileReady && retryCount < maxRetries)
        {
            try
            {
                // 先檢查檔案是否被鎖定
                if (IsFileLocked(fullPath))
                {
                    Debug.Log("Latent檔案正在被使用，等待...");
                    sameSizeCount = 0; // 重置連續計數
                    Thread.Sleep(retryDelayMs);
                    retryCount++;
                    continue;
                }

                // 取得當前檔案大小
                long currentSize = new FileInfo(fullPath).Length;
                Debug.Log($"檢查次數: {retryCount + 1}, 當前檔案大小: {currentSize}, 連續相同次數: {sameSizeCount}");

                if (currentSize == lastSize)
                {
                    sameSizeCount++;
                    if (sameSizeCount >= requiredSameSize)
                    {
                        // 檔案大小連續10次相同且沒有被鎖定，開始讀取
                        using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            if (fs.Length > 0)
                            {
                                Debug.Log($"檔案準備完成，最終大小: {fs.Length}");
                                byte[] imageBytes = new byte[fs.Length];
                                int bytesRead = fs.Read(imageBytes, 0, (int)fs.Length);

                                if (bytesRead == fs.Length)
                                {
                                    Debug.Log($"成功讀取檔案: {bytesRead} bytes");
                                    isFileReady = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 檔案大小改變，重置計數
                    sameSizeCount = 1;
                    lastSize = currentSize;
                }

                Thread.Sleep(retryDelayMs);
                retryCount++;
            }
            catch (IOException ex)
            {
                Debug.Log($"重試次數 {retryCount + 1}, 錯誤: {ex.Message}");
                sameSizeCount = 0; // 發生錯誤時重置計數
                Thread.Sleep(retryDelayMs);
                retryCount++;
            }
        }

        if (!isFileReady)
        {
            Debug.LogError($"無法讀取檔案: {fullPath}, 重試次數已達上限");
        }
    }
    private bool IsFileLocked(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        try
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                return false; // 檔案沒有被鎖定
            }
        }
        catch (IOException)
        {
            return true; // 檔案正在被使用
        }
        catch (Exception)
        {
            return true; // 其他例外情況也視為檔案被鎖定
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
