using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FileListener : MonoBehaviour
{
    [Header("Watching Paths")]
    [SerializeField] private string ConditioningFolderPath = @"C:\Users\妖精王\Desktop\ComfyUI_windows_portable_nvidia\ComfyUI_windows_portable\ComfyUI\output\ConditioningData";
    [SerializeField] private string LatentFolderPath = @"C:\Users\妖精王\Desktop\ComfyUI_windows_portable_nvidia\ComfyUI_windows_portable\ComfyUI\output\LatentData";
    [SerializeField] private string VAEFolderPath = @"C:\Users\妖精王\Desktop\ComfyUI_windows_portable_nvidia\ComfyUI_windows_portable\ComfyUI\output\VAEDebug";
    [SerializeField] private string PNGFolderPath = @"C:\Users\妖精王\Desktop\ComfyUI_windows_portable_nvidia\ComfyUI_windows_portable\ComfyUI\output";
    public GoogleDriveManager googleDriveManager;
    // �w�q�e���M�ƥ�
    public delegate void FileCreatedHandler(string filePath, string folderName);
    public static event FileCreatedHandler OnFileCreated;

    private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

    void Start()
    {
        SetupFileWatchers();
    }

    private void SetupFileWatchers()
    {
        try
        {
            // �]�w�T�Ӹ�Ƨ�����ť
            SetupWatcher(ConditioningFolderPath, "ConditioningData");
            SetupWatcher(LatentFolderPath, "LatentData");
            SetupWatcher(VAEFolderPath, "VAEDebug");
            SetupWatcher(PNGFolderPath, "PNG");
            Debug.Log("All file watchers setup complete");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting up file watchers: {e.Message}");
        }
    }

    private void SetupWatcher(string path, string watcherName)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        FileSystemWatcher watcher = new FileSystemWatcher(path);

        watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

        string currentWatcherName = watcherName;

        watcher.Created += (sender, e) => OnNewFileDetected(e, currentWatcherName);
        watcher.EnableRaisingEvents = true;

        watchers.Add(watcher);
        Debug.Log($"File watcher {watcherName} setup complete at: {path}");
    }

    private async void OnNewFileDetected(FileSystemEventArgs e, string watcherName)
    {
        Debug.Log($"New file detected in {watcherName} at: {e.FullPath}");

        // ���դW�ǡA�̦h����3��
        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                // ���ݮɶ��H�ۭ��զ��ƼW�[
                int delayMs = (attempt + 1) * 1000;
                await System.Threading.Tasks.Task.Delay(delayMs);

                // �ˬd�ɮ׬O�_�i�H�s��
                if (await IsFileReady(e.FullPath))
                {
                    switch (watcherName)
                    {
                        case "PNG":
                            await googleDriveManager.UploadPNGFile(e.FullPath);
                            return;
                        case "LatentData":
                            await googleDriveManager.UploadLatentFile(e.FullPath);
                            return;
                        case "ConditioningData":
                            await googleDriveManager.UploadConditioningFile(e.FullPath);
                            return;
                        case "VAEDebug":
                            await googleDriveManager.UploadVAEFile(e.FullPath);
                            return;
                        default:
                            Debug.LogError($"Unknown folder type: {watcherName}");
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Upload attempt {attempt + 1} failed: {ex.Message}");
                if (attempt == 2) // �̫�@������
                {
                    Debug.LogError($"Failed to upload file after 3 attempts: {e.FullPath}");
                }
            }
        }
    }

    private async Task<bool> IsFileReady(string filePath)
    {
        try
        {
            // �ˬd�ɮ׬O�_�s�b
            if (!File.Exists(filePath))
                return false;

            // ���ն}���ɮ׶i��Ū������
            using (FileStream inputStream = File.Open(filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                return inputStream.Length > 0;
            }
        }
        catch (IOException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking file: {ex.Message}");
            return false;
        }
    }

    void OnDestroy()
    {
        foreach (var watcher in watchers)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        watchers.Clear();
    }
}