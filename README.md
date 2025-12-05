# ComfyUI Server - 多人協作生成伺服器

<div align="center">

![放 Server Banner 圖]

**基於 Photon 的分散式 AI 圖像生成伺服器**

[![Unity](https://img.shields.io/badge/Unity-2021.3+-black?style=flat&logo=unity)](https://unity.com/)
[![Photon](https://img.shields.io/badge/Photon-PUN2-blue)](https://www.photonengine.com/)
[![ComfyUI](https://img.shields.io/badge/ComfyUI-Compatible-green)](https://github.com/comfyanonymous/ComfyUI)


</div>

---

## 📖 專案簡介

這是 ComfyUI Visualizer 的伺服器端，負責處理多人協作請求、檔案監聽與雲端同步。採用 Master-Client 架構，支援多人同時提交生成請求並自動排隊處理。

### 核心特色

- 🎯 **智能請求佇列**：自動管理多人請求，依序處理
- 📁 **即時檔案監聽**：監控 ComfyUI 輸出資料夾
- ☁️ **自動雲端同步**：生成檔案即時上傳 Google Drive
- 🔄 **狀態即時同步**：所有客戶端同步伺服器狀態

---

## 🏗️ 系統架構
```
┌─────────────────────────────────────────────────┐
│              Photon Cloud Server                │
│                  (Room System)                  │
└─────────────────────────────────────────────────┘
                        │
        ┌───────────────┴───────────────┐
        │                               │
┌───────▼────────┐             ┌───────▼────────┐
│  Master Client │             │     Clients    │
│   (Host PC)    │             │  (User Devices)│
├────────────────┤             ├────────────────┤
│ • 處理請求佇列  │             │ • 提交參數      │
│ • 執行 ComfyUI │             │ • 接收結果      │
│ • 檔案監聽     │             │ • 觀察視覺化    │
│ • 雲端上傳     │             └────────────────┘
└────────────────┘
        │
        ▼
┌────────────────┐
│   ComfyUI API  │
│  (localhost)   │
└────────────────┘
        │
        ▼
┌────────────────┐
│  File Watcher  │
│  (4 folders)   │
└────────────────┘
```

---

## ✨ 核心功能

### 1️⃣ 請求佇列系統

自動管理多人請求，防止資源競爭：
```csharp
private IEnumerator ProcessRequestQueue()
{
    isProcessingQueue = true;
    
    while (requestQueue.Count > 0)
    {
        RequestInfo currentRequest = requestQueue.Peek();
        
        // 通知當前處理的用戶
        Player sender = PhotonNetwork.CurrentRoom.GetPlayer(currentRequest.SenderID);
        photonView.RPC("ReceiveServerStatus", sender, "processing");
        
        // 處理請求...
        texttoimage.SetAllParametertxt2img(/* parameters */);
        
        yield return new WaitUntil(() => isProcessing == "finish");
        requestQueue.Dequeue();
    }
}
```

**佇列狀態顯示**：
- 等待中：顯示佇列位置
- 處理中：即時更新進度
- 完成：自動通知用戶

---

### 2️⃣ 四資料夾監聽系統

即時監控 ComfyUI 輸出資料夾：

| 資料夾 | 監聽內容 | 處理動作 |
|--------|---------|---------|
| 📊 ConditioningData | Token 權重矩陣 | 上傳至 Google Drive |
| 🎨 LatentData | 潛空間特徵 | 上傳至 Google Drive |
| 🖼️ VAEDebug | VAE 解碼資訊 | 上傳至 Google Drive |
| 🌅 PNG | 最終生成圖片 | 上傳至 Google Drive |

**智能檔案偵測**：
```csharp
private async Task<bool> IsFileReady(string filePath)
{
    try
    {
        using (FileStream inputStream = File.Open(filePath,
            FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            return inputStream.Length > 0;
        }
    }
    catch (IOException)
    {
        return false; // 檔案尚未完成寫入
    }
}
```

**重試機制**：最多重試 3 次，每次延遲遞增

---

### 3️⃣ Google Drive 自動同步

檔案生成後自動上傳並分享給客戶端：
```csharp
private async void OnNewFileDetected(FileSystemEventArgs e, string watcherName)
{
    for (int attempt = 0; attempt < 3; attempt++)
    {
        if (await IsFileReady(e.FullPath))
        {
            switch (watcherName)
            {
                case "PNG":
                    await googleDriveManager.UploadPNGFile(e.FullPath);
                    break;
                case "LatentData":
                    await googleDriveManager.UploadLatentFile(e.FullPath);
                    break;
                // ...
            }
            return;
        }
        await Task.Delay((attempt + 1) * 1000);
    }
}
```

---

### 4️⃣ Photon 網路同步

**RPC 通訊架構**：
```csharp
// Client → Master: 提交請求
photonView.RPC("ReceivePrompt", PhotonNetwork.MasterClient, 
    width, height, seed, steps, cfg, prompt, senderId);

// Master → Client: 回傳結果
photonView.RPC("ReceiveFileIds", sender, 
    pngId, latentId, conditioningId, vaeId, "isfinish");

// Master → All: 廣播狀態
photonView.RPC("ReceiveConfirmation", RpcTarget.Others, 
    "Current finish", isProcessing);
```

---

## 📁 專案結構
```
Assets/
├── 📡 FileAction/
│   ├── PhotonPrompt.cs              # Photon 網路管理核心
│   │   ├── 請求佇列系統
│   │   ├── RPC 通訊處理
│   │   └── 狀態同步管理
│   │
│   ├── FileListener.cs              # 檔案監聽系統
│   │   ├── 四資料夾監聽
│   │   ├── 檔案就緒檢測
│   │   └── 自動上傳觸發
│   │
│   ├── StartVisualize.cs            # 視覺化流程控制
│   └── VisualizeDataListener.cs    # 資料接收處理
│
├── ☁️ GoogleDrive/
│   └── GoogleDriveManager.cs        # 雲端同步管理
│       ├── 檔案上傳
│       ├── 權限設定
│       └── ID 回傳
│
├── 🎨 TextToImage/
│   └── texttoimage.cs               # ComfyUI API 整合
│       ├── JSON Workflow 生成
│       ├── API 請求發送
│       └── 參數驗證
│
└── 📊 XAIData/
    ├── ConditioningData.cs          # Conditioning 資料處理
    └── LatentData.cs                # Latent 資料處理
```

---

## 🚀 部署指南

### 環境需求

**硬體需求**：
- GPU: NVIDIA RTX 3060 以上（VRAM ≥ 12GB）
- RAM: 16GB 以上
- Storage: SSD 推薦

**軟體需求**：
- Unity 2021.3+
- ComfyUI (已安裝自定義節點)
- Photon PUN 2
- Google Drive API 憑證

### 安裝步驟
```bash
# 1. Clone 專案
git clone https://github.com/yourusername/comfyui-server.git

# 2. 設定 ComfyUI 自定義節點
cd ComfyUI/custom_nodes
git clone https://github.com/yourusername/comfyui-debug-nodes.git

# 3. 配置資料夾路徑
# 編輯 FileListener.cs 中的路徑：
# - ConditioningFolderPath
# - LatentFolderPath
# - VAEFolderPath
# - PNGFolderPath

# 4. 設定 Google Drive API
# 將 credentials.json 放入專案根目錄

# 5. 啟動 ComfyUI
python main.py --listen 127.0.0.1 --port 8188

# 6. 用 Unity 執行專案
```

### 配置檔案

**FileListener.cs** 路徑設定：
```csharp
[SerializeField] private string ConditioningFolderPath = 
    @"C:\ComfyUI\output\ConditioningData";
[SerializeField] private string LatentFolderPath = 
    @"C:\ComfyUI\output\LatentData";
[SerializeField] private string VAEFolderPath = 
    @"C:\ComfyUI\output\VAEDebug";
[SerializeField] private string PNGFolderPath = 
    @"C:\ComfyUI\output";
```

---

## 💡 技術亮點

### 🎯 非阻塞式佇列處理

使用 Coroutine 實現非同步佇列，避免 UI 凍結：
```csharp
yield return new WaitUntil(() => isProcessing == "finish");
```

### 🔒 檔案鎖定檢測

多層檢測確保檔案完整性：
```csharp
private bool IsFileLocked(string filePath)
{
    try
    {
        using (FileStream stream = File.Open(filePath, 
            FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            return false; // 檔案可用
        }
    }
    catch (IOException)
    {
        return true; // 檔案被鎖定
    }
}
```

### ⚡ 智能重試機制

指數退避策略，提高成功率：
```csharp
for (int attempt = 0; attempt < 3; attempt++)
{
    int delayMs = (attempt + 1) * 1000; // 1s, 2s, 3s
    await Task.Delay(delayMs);
    // 重試邏輯...
}
```

### 📡 RPC 狀態廣播

即時同步所有客戶端狀態：
```csharp
public override void OnPlayerEnteredRoom(Player newPlayer)
{
    // 新玩家加入時通知當前狀態
    photonView.RPC("ReceiveConfirmation", RpcTarget.Others, 
        "Current status", isProcessing);
}
```

---

## 🔄 運作流程

### 完整請求處理流程

```
1. Client 提交請求
   ↓
2. Master 接收並加入佇列
   ↓
3. 回傳「已接收」確認 + 佇列位置
   ↓
4. 依序處理佇列
   ↓
5. 發送 ComfyUI API 請求
   ↓
6. FileListener 監聽輸出資料夾
   ↓
7. 偵測到新檔案
   ↓
8. 檢查檔案完整性（重試機制）
   ↓
9. 上傳至 Google Drive
   ↓
10. 取得 File ID
    ↓
11. RPC 回傳 File IDs 給 Client
    ↓
12. Client 下載並視覺化
    ↓
13. 處理下一個請求
```

---

## 📊 效能優化

### 佇列處理統計

| 項目 | 數值 |
|------|------|
| 平均等待時間 | 30-60 秒/請求 |
| 最大並發數 | 10 個客戶端 |
| 檔案上傳速度 | ~2 秒/檔案 |
| 記憶體佔用 | ~500 MB |

### 資源管理
```csharp
void OnDestroy()
{
    // 清理 FileSystemWatcher
    foreach (var watcher in watchers)
    {
        watcher.EnableRaisingEvents = false;
        watcher.Dispose();
    }
    watchers.Clear();
}
```

---

## 🐛 錯誤處理

### 檔案監聽異常
```csharp
catch (Exception ex)
{
    Debug.LogWarning($"Upload attempt {attempt + 1} failed: {ex.Message}");
    if (attempt == 2) // 最後一次嘗試
    {
        Debug.LogError($"Failed after 3 attempts: {e.FullPath}");
    }
}
```

### 網路斷線處理
```csharp
public override void OnPlayerLeftRoom(Player otherPlayer)
{
    UpdateStatus($"設備已離線，{PhotonNetwork.PlayerList.Length} 個使用者");
    // 自動清理該玩家的待處理請求
}
```

---

## 🔧 常見問題

<details>
<summary><b>Q: 如何修改佇列大小限制？</b></summary>

修改 `PhotonPrompt.cs`：
```csharp
private const int MAX_QUEUE_SIZE = 20; // 預設無限制
```
</details>

<details>
<summary><b>Q: 檔案上傳失敗怎麼辦？</b></summary>

檢查：
1. Google Drive API 配額
2. 網路連線狀態
3. credentials.json 是否有效
4. 檔案路徑權限
</details>

<details>
<summary><b>Q: 如何增加重試次數？</b></summary>

修改 `FileListener.cs`：
```csharp
for (int attempt = 0; attempt < 5; attempt++) // 改為 5 次
```
</details>

---

## 🗺️ 未來規劃

- [ ] 支援多 ComfyUI 實例負載平衡
- [ ] 實作優先權佇列系統
- [ ] 加入請求取消功能
- [ ] WebSocket 替代 Photon（降低延遲）
- [ ] 支援 Redis 佇列持久化
- [ ] Docker 容器化部署

---

## 📈 監控與日誌

### 伺服器狀態監控
```csharp
private void UpdateStatus(string message)
{
    if (statusText != null)
        statusText.text = message;
    Debug.Log($"[{DateTime.Now:HH:mm:ss}] {message}");
}
```

### 關鍵日誌點

- ✅ 新請求接收
- ✅ 佇列狀態變更
- ✅ 檔案監聽觸發
- ✅ 上傳成功/失敗
- ✅ 客戶端連線/斷線

---

## 📝 相關專案

- [ComfyUI Visualizer (Client)](https://github.com/yourusername/comfyui-visualizer) - 視覺化客戶端
- [ComfyUI Debug Nodes](https://github.com/yourusername/comfyui-debug-nodes) - 自定義節點

---

## 📧 聯絡方式

- 📫 Email: a664104797@gmail.com

---

## 📄 授權

MIT License - 詳見 [LICENSE](LICENSE)

---

<div align="center">


Made by 胡修銘

</div>
