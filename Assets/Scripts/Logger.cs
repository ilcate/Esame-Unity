/*using System.Linq;
using DilmerGames.Core.Singletons;
using TMPro;
using UnityEngine;
using System;

public class Logger : Singleton<Logger>
{
    [SerializeField] private TextMeshProUGUI debugAreaText = null;
    [SerializeField] private bool enableDebug = false;
    [SerializeField] private int maxLines = 15;

    void Awake()
    {
        if (debugAreaText == null)
        {
            debugAreaText = GetComponent<TextMeshProUGUI>();
        }
        debugAreaText.text = string.Empty;
    }

    void OnEnable()
    {
        debugAreaText.enabled = enableDebug;
        enabled = enableDebug;

        if (enabled)
        {
            LogMessage($"{this.GetType().Name} enabled", Color.white);
        }
    }

    private void LogMessage(string message, Color color)
    {
        string formattedMessage = $"<color=\"{ColorUtility.ToHtmlStringRGB(color)}\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        debugAreaText.text += formattedMessage;
        ClearLines();
    }

    public void LogInfo(string message)
    {
        LogMessage(message, Color.green);
    }

    public void LogError(string message)
    {
        LogMessage(message, Color.red);
    }

    public void LogWarning(string message)
    {
        LogMessage(message, Color.yellow);
    }

    private void ClearLines()
    {
        string[] lines = debugAreaText.text.Split('\n');
        if (lines.Length >= maxLines)
        {
            debugAreaText.text = string.Join("\n", lines.Skip(lines.Length - maxLines));
        }
    }
}
*/