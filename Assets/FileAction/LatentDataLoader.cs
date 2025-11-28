using UnityEngine;
using System.IO;
using System;
using System.Globalization;
using System.Collections.Generic;

public class LatentDataLoader : MonoBehaviour
{
    public float[,,] channelData { get; private set; }  // [channel, height, width]
    public int dataWidth { get; private set; }
    public int dataHeight { get; private set; }
    public int channelCount { get; private set; }

    void Start()
    {
        //string filePath = @"C:\Users\admin\Desktop\parameter\latent_20250117_102634.txt";
        //LoadLatentData(filePath);
    }

    public bool LoadLatentData(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            List<List<float>> channelsValues = new List<List<float>>();
            List<float> currentChannelValues = null;

            foreach (string line in lines)
            {
                if (line.Contains("Batch 0, Channel"))
                {
                    if (currentChannelValues != null)
                    {
                        channelsValues.Add(currentChannelValues);
                    }
                    currentChannelValues = new List<float>();
                    continue;
                }

                if (currentChannelValues != null && !string.IsNullOrWhiteSpace(line) && !line.Contains("Batch"))
                {
                    string[] numberStrings = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string numStr in numberStrings)
                    {
                        if (float.TryParse(numStr, NumberStyles.Float | NumberStyles.AllowExponent,
                            CultureInfo.InvariantCulture, out float value))
                        {
                            currentChannelValues.Add(value);
                        }
                    }
                }
            }

            // 添加最後一個通道的數據
            if (currentChannelValues != null)
            {
                channelsValues.Add(currentChannelValues);
            }

            // 設置數據維度
            channelCount = channelsValues.Count;
            dataWidth = 5;
            dataHeight = 5;

            // 創建並填充數據數組
            channelData = new float[channelCount, dataHeight, dataWidth];

            for (int c = 0; c < channelCount; c++)
            {
                var values = channelsValues[c];
                for (int y = 0; y < dataHeight; y++)
                {
                    for (int x = 0; x < dataWidth; x++)
                    {
                        int index = y * dataWidth + x;
                        if (index < values.Count)
                        {
                            channelData[c, y, x] = values[index];
                        }
                    }
                }
            }

            Debug.Log($"Successfully loaded {channelCount} channels of {dataWidth}x{dataHeight} data");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading latent data: {e.Message}");
            return false;
        }
    }
}