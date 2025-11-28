using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System.IO;
public class FileDownloader : MonoBehaviour
{
    [SerializeField] private GoogleDriveManager googleDriveManager;
    [SerializeField] private Text statusText;
    private bool isDownloading = false;

    private void Start()
    {
        if (googleDriveManager == null)
        {
            googleDriveManager = FindObjectOfType<GoogleDriveManager>();
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }

    public async void StartDownload(string pngId, string latentId, string conditioningId)
    {
        if (isDownloading)
        {
            Debug.Log("Already downloading files...");
            return;
        }

        isDownloading = true;
        UpdateStatus("秨﹍更郎...");

        try
        {
            // 絋玂ヘ魁
            string[] paths = {
                "/storage/emulated/0/DCIM/Picture",
                "/storage/emulated/0/DCIM/LatentData",
                "/storage/emulated/0/DCIM/ConditioningData"
            };

            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Debug.Log($"Created directory: {path}");
                }
            }

            // 更 PNG 郎
            if (!string.IsNullOrEmpty(pngId))
            {
                UpdateStatus("更瓜い...");
                await googleDriveManager.DownloadAndProcessFile(pngId, "/storage/emulated/0/DCIM/Picture");

                // 絋粄郎琌更Θ
                var files = Directory.GetFiles("/storage/emulated/0/DCIM/Picture");
                Debug.Log($"Picture directory contains {files.Length} files");
            }
            if (!string.IsNullOrEmpty(latentId))
            {
                UpdateStatus("更 Latent い...");
                await googleDriveManager.DownloadAndProcessFile(latentId, "/storage/emulated/0/DCIM/LatentData");

                // 絋粄郎琌更Θ
                var files = Directory.GetFiles("/storage/emulated/0/DCIM/LatentData");
                Debug.Log($"Picture directory contains {files.Length} files");
            }
            if (!string.IsNullOrEmpty(conditioningId))
            {
                UpdateStatus("更 Conditioning い...");
                await googleDriveManager.DownloadAndProcessFile(conditioningId, "/storage/emulated/0/DCIM/ConditioningData");

                // 絋粄郎琌更Θ
                var files = Directory.GetFiles("/storage/emulated/0/DCIM/ConditioningData");
                Debug.Log($"Picture directory contains {files.Length} files");
            }

            // 妓家Αノㄤ郎...

            UpdateStatus("┮Τ郎更ЧΘ");
        }
        catch (System.Exception e)
        {
            string errorMessage = $"更筁祘祇ネ岿粇: {e.Message}\nStack Trace: {e.StackTrace}";
            Debug.LogError(errorMessage);
            UpdateStatus($"更岿粇: {e.Message}");
        }
        finally
        {
            isDownloading = false;
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log(message);
    }
}