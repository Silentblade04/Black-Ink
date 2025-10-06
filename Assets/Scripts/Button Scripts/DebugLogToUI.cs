using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLogToUI : MonoBehaviour
{
    public TextMeshProUGUI logText; // Replace Text with TextMeshProUGUI if using TMP
    private Queue<string> logQueue = new Queue<string>();
    public int maxLines = 20;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Format log entry
        string logEntry = logString;
        if (type == LogType.Error || type == LogType.Exception)
            logEntry = $"<color=red>{logString}</color>";
        else if (type == LogType.Warning)
            logEntry = $"<color=yellow>{logString}</color>";

        logQueue.Enqueue(logEntry);

        if (logQueue.Count > maxLines)
            logQueue.Dequeue();

        logText.text = string.Join("\n", logQueue.ToArray());
    }
}
