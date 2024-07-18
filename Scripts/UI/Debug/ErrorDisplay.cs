using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ErrorDisplay : MonoBehaviour
{
    public TextMeshProUGUI errorText;

    void Start()
    {
        // Настройте ваш обработчик ошибок
        Application.logMessageReceived += HandleLog;
    }

    void HandleLog(string logText, string stackTrace, LogType type)
    {
        // Ваша логика обработки ошибок
        if (type == LogType.Error || type == LogType.Exception)
        {
            // Вывести ошибку на экран
            DisplayError(logText);
        }
    }

    void DisplayError(string error)
    {
        // Вывести ошибку на UI Text или другой элемент GUI
        if (errorText != null)
        {
            errorText.text = "Error: " + error;
        }
    }
}
