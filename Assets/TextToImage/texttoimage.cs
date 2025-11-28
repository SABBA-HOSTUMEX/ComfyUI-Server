using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class texttoimage : MonoBehaviourPunCallbacks
{
    public TMP_InputField seedInputField;
    public TMP_InputField stepsInputField;
    public TMP_InputField cfgInputField;
    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;
    public TMP_InputField promptInputField;
    public Button generateButton;
    public Button EmptyLatentImageBTN;
    public Button KsamplerBTN;
    private string Model1 = "realisticVision.safetensors";
    private string Model2 = "sdxlNijiSeven_sdxlNijiSeven.safetensors";
    public string seed { get; set; }
    public string steps { get; set; }
    public string cfg { get; set; }
    public string width { get; set; }
    public string height { get; set; }
    public string prompt { get; set; }
    public string Modelname { get; set; }

    // public TMP_Text process;
    public Text CheckpointValue;
    public Text LatentImageValue;
    public Text KsamplerValue;
    public PhotonPrompt networkManager;
    private string apiUrl = "http://127.0.0.1:8188/prompt";//your ComfyUI web

    // Start is called before the first frame update
    void Start()
    {
        generateButton.onClick.AddListener(SendValue);  // �]�m���s�I���ƥ�
        EmptyLatentImageBTN.onClick.AddListener(OnSaveEmptyLatentImageData);
        KsamplerBTN.onClick.AddListener(OnSaveKsamplerData);
    }
    public void SendValue()
    {
        prompt = promptInputField.text;
        string send_width = width;  // 假設你有 InputField 組件
        string send_height = height;
        string send_seed = seed;
        string send_steps = steps;
        string send_cfg = cfg;
        string send_prompt = prompt;
        // networkManager.SendPrompt(send_width, send_height, send_seed, send_steps, send_cfg, send_prompt);
    }
    // Update is called once per frame
    void Update()
    {

    }
    // 接回接收到的 api 參數並且設置到 api json 裡面
    public void SetAllParametertxt2img(string User_model, string User_width, string User_height, string User_seed, string User_steps, string User_cfg, string User_prompt)
    {
        width = User_width;
        height = User_height;
        seed = User_seed;
        steps = User_steps;
        cfg = User_cfg;
        prompt = User_prompt;
        if(User_model == "Realistic")
            Modelname = Model1;
        if (User_model == "Normal")
            Modelname = Model2;
        OnGenerateButtonClicked();
    }
    // 把輸入的參數傳給裝置 A (master client)
    public void OnSaveEmptyLatentImageData()
    {
        width = widthInputField.text;
        height = heightInputField.text;
        Debug.Log($"width : {width}  height : {height}");
        LatentImageValue.text = $"width : {width}  height : {height}";
    }
    public void OnSaveKsamplerData()
    {
        seed = seedInputField.text;
        steps = stepsInputField.text;
        cfg = cfgInputField.text;
        Debug.Log($"seed : {seed}  steps : {steps}  cfg : {cfg}");
        KsamplerValue.text = $"seed : {seed}  steps : {steps}  cfg : {cfg}";
    }
    void OnGenerateButtonClicked()
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            StartCoroutine(SendPromptToServer());
        }
        else
        {
            // process.text = "Prompt cannot be empty!";
            // Debug.Log(process.text);
        }
    }

    IEnumerator SendPromptToServer()
    {
        // �Ы� JSON �ó]�m prompt
        string jsonPayload = CreateTexttoImageJson();

        // �o�e API �ШD
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log(jsonPayload);

        // ���ݦ^��
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // process.text = "Prompt sent successfully! Checking model generation...";
        }
        else
        {
            // process.text = "Error sending prompt: " + request.error;
            // Debug.Log(process.text);
        }
    }
    string CreateTexttoImageJson()
    {
        // 創建主要的工作流程物件
        var workflow = new JObject
        {
            // KSampler 節點
            ["3"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["seed"] = seed,
                    ["steps"] = steps,
                    ["cfg"] = cfg,
                    ["sampler_name"] = "euler",
                    ["scheduler"] = "normal",
                    ["denoise"] = 1,
                    ["model"] = new JArray { "4", 0 },
                    ["positive"] = new JArray { "10", 0 },
                    ["negative"] = new JArray { "13", 0 },  // 新增 negative prompt 連接
                    ["latent_image"] = new JArray { "5", 0 }
                },
                ["class_type"] = "KSampler",
                ["_meta"] = new JObject { ["title"] = "KSampler" }
            },

            // 模型載入節點
            ["4"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["ckpt_name"] = Modelname
                },
                ["class_type"] = "CheckpointLoaderSimple",
                ["_meta"] = new JObject { ["title"] = "Load Checkpoint" }
            },

            // 空白潛空間圖像節點
            ["5"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["width"] = width,
                    ["height"] = height,
                    ["batch_size"] = 1
                },
                ["class_type"] = "EmptyLatentImage",
                ["_meta"] = new JObject { ["title"] = "Empty Latent Image" }
            },

            // 正面提示詞編碼節點
            ["6"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["text"] = prompt,
                    ["clip"] = new JArray { "4", 1 }
                },
                ["class_type"] = "CLIPTextEncode",
                ["_meta"] = new JObject { ["title"] = "CLIP Text Encode (Prompt)" }
            },

            // VAE 解碼節點
            ["8"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["samples"] = new JArray { "11", 0 },
                    ["vae"] = new JArray { "4", 2 }
                },
                ["class_type"] = "VAEDecode",
                ["_meta"] = new JObject { ["title"] = "VAE Decode" }
            },

            // 儲存圖像節點
            ["9"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["filename_prefix"] = "ComfyUI",
                    ["images"] = new JArray { "12", 0 }
                },
                ["class_type"] = "SaveImage",
                ["_meta"] = new JObject { ["title"] = "Save Image" }
            },

            // 正面提示詞調試節點
            ["10"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["show_full_matrix"] = true,
                    ["num_values"] = 10,
                    ["save_to_file"] = true,
                    ["force_run"] = true,
                    ["conditioning"] = new JArray { "6", 0 }
                },
                ["class_type"] = "DebugConditioning",
                ["_meta"] = new JObject { ["title"] = "Debug Conditioning Matrix" }
            },

            // 潛空間保存節點
            ["11"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["add_timestamp"] = true,
                    ["latent"] = new JArray { "3", 0 }
                },
                ["class_type"] = "SaveLatentToFile",
                ["_meta"] = new JObject { ["title"] = "Save Latent to TXT" }
            },

            // VAE調試保存節點
            ["12"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["add_timestamp"] = true,
                    ["samples"] = new JArray { "8", 0 },
                    ["original_latent"] = new JArray { "3", 0 }
                },
                ["class_type"] = "VAEDebugSave",
                ["_meta"] = new JObject { ["title"] = "Save VAE Debug Info" }
            },

            // 負面提示詞編碼節點
            ["13"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["text"] = "Bad Things",  // 新增負面提示詞參數
                    ["clip"] = new JArray { "4", 1 }
                },
                ["class_type"] = "CLIPTextEncode",
                ["_meta"] = new JObject { ["title"] = "CLIP Text Encode (Prompt)" }
            }
        };

        var finalPayload = new JObject { ["prompt"] = workflow };
        return finalPayload.ToString(Formatting.None);
    }
}
