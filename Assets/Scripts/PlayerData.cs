using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    private const string UserIdKey = "user_id";
    private static string selectedTopic;
    private static string level;

    private static string interest;
    private static Dictionary<string, int> knowledgeLevels = new Dictionary<string, int>();

    private static int evaluationId = 0;

    private static readonly List<string> temasDisponibles = new List<string>
    {
        "ahorro",
        "inversión",
        "credito-deudas"
    };

    public static void SetUserId(int userId)
    {
        PlayerPrefs.SetInt(UserIdKey, userId);
        PlayerPrefs.Save();
    }

    public static int GetUserId()
    {
        return PlayerPrefs.GetInt(UserIdKey, 0); 
    }

    public static void SetInterest(string interest)
    {
        PlayerData.interest = interest;
    }

    public static string GetInterest()
    {
        return interest;
    }

    public static void SetKnowledge(string topic, int level)
    {
        knowledgeLevels[topic] = level;
    }

    public static int GetKnowledge(string topic)
    {
        return knowledgeLevels.ContainsKey(topic) ? knowledgeLevels[topic] : 0;
    }

    public static void SetSelectedTopic(string topic)
    {
        selectedTopic = topic;
    }
    public static string GetSelectedTopic()
    {
        return selectedTopic;
    }

    public static int GetEvaluationId()
    {
        return evaluationId;
    }

    public static void SetEvaluationId(int id)
    {
        evaluationId = id;
    }

    public static void SetSelectedLevel(string nivel)
    {
        level = nivel;
    }
    public static string GetSelectedLevel()
    {
        return string.IsNullOrEmpty(level) ? "basico" : level;
    }

    //
    public static void SetEvaluationCompleted(string tema, bool completed)
    {
        PlayerPrefs.SetInt($"evaluation_completed_{tema}", completed ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool GetEvaluationCompleted(string tema)
    {
        return PlayerPrefs.GetInt($"evaluation_completed_{tema}", 0) == 1;
    }

    public static void SetUserLevel(string tema, string nivel)
    {
        PlayerPrefs.SetString($"user_level_{tema}", nivel);
        PlayerPrefs.Save();
    }

    public static string GetUserLevel(string tema)
    {
        return PlayerPrefs.GetString($"user_level_{tema}", "");
    }

    public static bool HasCompletedAnyInitialEvaluation()
    {
        foreach (var tema in temasDisponibles)
        {
            if (GetEvaluationCompleted(tema))
            {
                return true;
            }
        }
        return false;
    }

}
