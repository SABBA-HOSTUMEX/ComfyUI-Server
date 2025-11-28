using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

public class PhotonPrompt : MonoBehaviourPunCallbacks
{
   
    [SerializeField] private Text statusText;
    private const string roomName = "ComfyUIServer";
    private PhotonView photonView;
    public texttoimage texttoimage;
    public MenuUI menuUI;
    private int currentClientId = -1;
    private string latestPngId;
    private string latestLatentId;
    private string latestConditioningId;
    private string latestVAEId;
    private int clientID;
    private bool hasReceivedConfirmation = false;
    [HideInInspector]
    public string isProcessing;
    
    // 新增佇列相關變數
    private Queue<RequestInfo> requestQueue = new Queue<RequestInfo>();
    private bool isProcessingQueue = false;
    
    // 新增 GoogleDriveManager 參考
    [SerializeField] private GoogleDriveManager googleDriveManager;
        
    // 請求資訊類別
    private class RequestInfo
    {
        public string ModelType;
        public string Width;
        public string Height;
        public string Seed;
        public string Steps;
        public string Cfg;
        public string Prompt;
        public int SenderID;

        public RequestInfo(string modelType, string width, string height, string seed, 
                           string steps, string cfg, string prompt, int senderID)
        {
            ModelType = modelType;
            Width = width;
            Height = height;
            Seed = seed;
            Steps = steps;
            Cfg = cfg;
            Prompt = prompt;
            SenderID = senderID;
        }
    }
    
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
            Debug.LogError("PhotonView component not found!");
    }

    void Start()
    {
        isProcessing = "finish";
        PhotonNetwork.ConnectUsingSettings();
        UpdateStatus("連線中...");
    }

    // 建立房間 (Master Client用)
    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions options = new RoomOptions { MaxPlayers = 10 };
            PhotonNetwork.CreateRoom(roomName, options);
            UpdateStatus("房間已建立");
        }
    }

    // 加入房間 (一般Client用)
    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }
   

    [PunRPC]
    private void ShareClientId(int clientId)
    {
        Debug.Log($"Client ID received: {clientId}");
        // 這裡可以存儲其他客戶端的ID
        if (clientId != currentClientId)
        {
            Debug.Log($"Other client connected with ID: {clientId}");
        }
    }
    
    [PunRPC]
    // 發送 prompt
    public void SendPrompt(string User_width, string User_height, string User_seed, string User_steps, string User_cfg, string User_prompt)
    {
        if (!PhotonNetwork.InRoom) return;

        int senderId = PhotonNetwork.LocalPlayer.ActorNumber;

        if (PhotonNetwork.IsMasterClient)
        {
            UpdateStatus("您是主機，無法傳送訊息給自己");
            return;
        }
        photonView.RPC("ReceivePrompt", PhotonNetwork.MasterClient, "", User_width, User_height, User_seed, User_steps, User_cfg, User_prompt, senderId);
    }

    [PunRPC]
    private void ReceivePrompt(string User_Model, string User_width, string User_height, string User_seed, 
                              string User_steps, string User_cfg, string User_prompt, int senderId)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        // 將請求加入佇列
        RequestInfo request = new RequestInfo(User_Model, User_width, User_height, User_seed, 
                                             User_steps, User_cfg, User_prompt, senderId);
        requestQueue.Enqueue(request);
        
        // 更新狀態為忙碌
        isProcessing = "busy";
        
        // 回覆用戶已接收請求
        Player sender = PhotonNetwork.CurrentRoom.GetPlayer(senderId);
        if (sender != null)
        {
            string queuePosition = requestQueue.Count > 1 ? $"（佇列位置：{requestQueue.Count}）" : "";
            photonView.RPC("ReceiveConfirmation", sender, $"已接收您的請求{queuePosition}","success");
            
            // 通知用戶佇列狀態
            string queueStatus = $"佇列中的請求數：{requestQueue.Count}";
            UpdateStatus(queueStatus);
        }
        
        // 若尚未處理佇列，則開始處理
        if (!isProcessingQueue)
        {
            StartCoroutine(ProcessRequestQueue());
        }
    }
    
    private IEnumerator ProcessRequestQueue()
    {
        isProcessingQueue = true;
        
        while (requestQueue.Count > 0)
        {
            // 取出佇列中的第一個請求
            RequestInfo currentRequest = requestQueue.Peek();
            
            // 通知當前處理的用戶
            Player sender = PhotonNetwork.CurrentRoom.GetPlayer(currentRequest.SenderID);
            if (sender != null)
            {
                photonView.RPC("ReceiveServerStatus", sender, "processing");
            }
            
            // 更新狀態顯示
            string allparameter = $"width: {currentRequest.Width} height: {currentRequest.Height} " +
                                 $"seed: {currentRequest.Seed} steps: {currentRequest.Steps} cfg: {currentRequest.Cfg}";
            UpdateStatus($"處理來自玩家 {currentRequest.SenderID} 的參數: {allparameter}");
            
            // 保存當前處理的客戶端ID
            clientID = currentRequest.SenderID;
            
            // 設定參數並開始處理圖像
            texttoimage.SetAllParametertxt2img(currentRequest.ModelType, currentRequest.Width, currentRequest.Height, currentRequest.Seed, 
                                              currentRequest.Steps, currentRequest.Cfg, currentRequest.Prompt);
            
            // 等待處理完成的信號 (SendFileIds 會被呼叫)
            bool isRequestProcessed = false;
            yield return new WaitUntil(() => isProcessing == "finish");
            
            // 移除已處理的請求
            requestQueue.Dequeue();
            
            // 簡短延遲，確保處理完成
            yield return new WaitForSeconds(0.5f);
        }
        
        // 所有請求處理完畢，更新狀態為完成
        isProcessing = "finish";
        UpdateStatus("所有請求處理完畢");
        isProcessingQueue = false;
    }
    
    [PunRPC]
    private void ReceiveServerStatus(string message)
    {
        Debug.Log($"收到伺服器狀態: {message}");
        UpdateStatus(message);
    }


    [PunRPC]
    private void ReceiveConfirmation(string message)
    {
        Debug.Log(message);
        UpdateStatus(message);
    }
    
    [PunRPC]
    public void SendFileIds(string pngId, string latentId, string conditioningId, string vaeId)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        latestPngId = pngId;
        latestLatentId = latentId;
        latestConditioningId = conditioningId;
        latestVAEId = vaeId;
        string isfinishbool = "isfinish";
        Player sender = PhotonNetwork.CurrentRoom.GetPlayer(clientID);

        // 確認所有檔案都處理完成
        if (!string.IsNullOrEmpty(pngId) &&
            !string.IsNullOrEmpty(latentId) &&
            !string.IsNullOrEmpty(conditioningId) &&
            !string.IsNullOrEmpty(vaeId))
        {
            if (sender != null)
            {
                // 發送檔案 ID
                photonView.RPC("ReceiveFileIds", sender, pngId, latentId, conditioningId, vaeId, isfinishbool);
                isProcessing = "finish";
                // 發送可以再次傳送的訊息
                photonView.RPC("ReceiveConfirmation", RpcTarget.Others, " Current finish", isProcessing);
                Debug.Log("所有檔案處理完成，通知用戶");
            }

            // 清除已使用的 ID
            latestPngId = null;
            latestLatentId = null;
            latestConditioningId = null;
            latestVAEId = null;
        }
    }

   

    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        Debug.Log(message);
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        UpdateStatus("已連線到伺服器");
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        UpdateStatus($"已進入房間，{PhotonNetwork.PlayerList.Length} 個使用者");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateStatus($"新設備已連線，{PhotonNetwork.PlayerList.Length} 個使用者");
        photonView.RPC("ReceiveConfirmation", RpcTarget.Others, "Current busy", isProcessing);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateStatus($"設備已離線，{PhotonNetwork.PlayerList.Length} 個使用者");
    }
    #endregion
}