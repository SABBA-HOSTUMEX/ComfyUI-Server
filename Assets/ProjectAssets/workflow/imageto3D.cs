using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using TMPro;

public class imageto3D : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public string CreateStableFast3DJson(string imagePath)
    {
        var workflow = new JObject
        {
            ["1"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["image"] = imagePath,
                    ["upload"] = "image"
                },
                ["class_type"] = "LoadImage",
                ["_meta"] = new JObject { ["title"] = "Load Image" }
            },
            ["7"] = new JObject
            {
                ["inputs"] = new JObject(),
                ["class_type"] = "StableFast3DLoader",
                ["_meta"] = new JObject { ["title"] = "Stable Fast 3D Loader" }
            },
            ["8"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["foreground_ratio"] = 0.87,
                    ["texture_resolution"] = 2048,
                    ["remesh"] = "quad",
                    ["vertex_count"] = -1,
                    ["model"] = new JArray { "7", 0 },
                    ["image"] = new JArray { "11", 0 },
                    ["mask"] = new JArray { "11", 1 }
                },
                ["class_type"] = "StableFast3DSampler",
                ["_meta"] = new JObject { ["title"] = "Stable Fast 3D Sampler" }
            },
            ["9"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["filename_prefix"] = "SF3D",
                    ["preview"] = null,
                    ["mesh"] = new JArray { "8", 0 }
                },
                ["class_type"] = "StableFast3DSave",
                ["_meta"] = new JObject { ["title"] = "Stable Fast 3D Save" }
            },
            ["11"] = new JObject
            {
                ["inputs"] = new JObject
                {
                    ["alpha_matting"] = false,
                    ["alpha_matting_foreground_threshold"] = 240,
                    ["alpha_matting_background_threshold"] = 10,
                    ["alpha_matting_erode_size"] = 10,
                    ["post_process_mask"] = false,
                    ["bgcolor"] = "#000000",
                    ["image"] = new JArray { "1", 0 }
                },
                ["class_type"] = "Image Remove Background Rembg (mtb)",
                ["_meta"] = new JObject { ["title"] = "Image Remove Background Rembg (mtb)" }
            }
        };

        var finalPayload = new JObject { ["prompt"] = workflow };
        return finalPayload.ToString(Formatting.None);
    }
}
