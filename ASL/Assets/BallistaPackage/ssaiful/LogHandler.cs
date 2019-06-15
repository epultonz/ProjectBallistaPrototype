using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LogHandler1 : MonoBehaviour
{
    private string output = "";
    private string stack = "";

    public Text txt;

    void OnEnable () {
         Application.logMessageReceived += HandleLog;
     }
     
     void OnDisable () {
         Application.logMessageReceived -= HandleLog;
     }

     void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
    }

    private void Update() {
        txt.text = output;
    }
}
