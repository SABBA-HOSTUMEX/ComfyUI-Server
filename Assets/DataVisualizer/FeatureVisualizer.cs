using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeatureVisualizer : MonoBehaviour
{
    [Header("UI References")]
    public RawImage[] channelDisplays;
    public TextMeshProUGUI infoText;

    [Header("Visualization Settings")]
    public Color lowValueColor = Color.blue;
    public Color midValueColor = Color.green;
    public Color highValueColor = Color.red;
    public float visualScale = 4f;

    private LatentDataLoader dataLoader;
    private Texture2D[] channelTextures;

    void Start()
    {
        dataLoader = GetComponent<LatentDataLoader>();
        InitializeTextures();
        UpdateVisualization();
    }

    private void InitializeTextures()
    {
        channelTextures = new Texture2D[4];
        for (int i = 0; i < 4; i++)
        {
            if (channelDisplays[i] != null)
            {
                channelTextures[i] = new Texture2D(5, 5, TextureFormat.RGBA32, false);
                channelTextures[i].filterMode = FilterMode.Point;

                channelDisplays[i].texture = channelTextures[i];
                
            }
        }
    }

    public void UpdateVisualization()
    {
        if (dataLoader.channelData == null)
        {
            Debug.LogError("No latent data available!");
            return;
        }

        // 更新所有通道
        for (int channel = 0; channel < dataLoader.channelCount; channel++)
        {
            UpdateChannelVisualization(channel);
        }
    }

    private void UpdateChannelVisualization(int channel)
    {
        float minVal = float.MaxValue;
        float maxVal = float.MinValue;

        // 找出最大最小值
        for (int y = 0; y < dataLoader.dataHeight; y++)
        {
            for (int x = 0; x < dataLoader.dataWidth; x++)
            {
                float val = dataLoader.channelData[channel, y, x];
                minVal = Mathf.Min(minVal, val);
                maxVal = Mathf.Max(maxVal, val);
            }
        }

        Debug.Log($"Channel {channel} - Min: {minVal:F2}, Max: {maxVal:F2}");

        // 更新紋理
        for (int y = 0; y < dataLoader.dataHeight; y++)
        {
            for (int x = 0; x < dataLoader.dataWidth; x++)
            {
                float val = dataLoader.channelData[channel, y, x];
                float normalizedValue = Mathf.InverseLerp(minVal, maxVal, val);

                Color color;
                if (normalizedValue < 0.5f)
                {
                    color = Color.Lerp(lowValueColor, midValueColor, normalizedValue * 2);
                }
                else
                {
                    color = Color.Lerp(midValueColor, highValueColor, (normalizedValue - 0.5f) * 2);
                }

                channelTextures[channel].SetPixel(x, dataLoader.dataHeight - 1 - y, color);
            }
        }

        channelTextures[channel].Apply();
    }

    public void ShowFeatureInfo(int channel, Vector2 position)
    {
        if (infoText == null) return;

        int x = Mathf.FloorToInt(position.x * dataLoader.dataWidth);
        int y = Mathf.FloorToInt(position.y * dataLoader.dataHeight);

        if (x >= 0 && x < dataLoader.dataWidth && y >= 0 && y < dataLoader.dataHeight)
        {
            float value = dataLoader.channelData[channel, y, x];
            string info = $"Channel {channel}\n" +
                         $"Position: ({x}, {y})\n" +
                         $"Value: {value:F2}";
            infoText.text = info;
        }
    }
}