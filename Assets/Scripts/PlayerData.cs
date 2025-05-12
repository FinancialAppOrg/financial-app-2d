using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    private const string UserIdKey = "user_id";
    private static string selectedTopic;

    private static string interest;
    private static Dictionary<string, int> knowledgeLevels = new Dictionary<string, int>();

    private static int evaluationId = 0;

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

}
