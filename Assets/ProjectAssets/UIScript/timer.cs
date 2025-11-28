using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using TMPro;
using System.Linq;
public class timer : MonoBehaviour
{
    public TMP_Text timetext;
    List<double> TimeList = new List<double>();
    Stopwatch sw = new Stopwatch();
    // Start is called before the first frame update
    public void timestart()
    {
        sw.Restart();
    }
    public void timestop()
    {
        sw.Stop();
        double elapsedSeconds = sw.Elapsed.TotalSeconds;
        TimeList.Add(elapsedSeconds);
        timetext.text = $"Generate time : {TimeList.Last().ToString("F1")}";
    }

}
