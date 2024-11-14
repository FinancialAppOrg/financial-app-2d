using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    private static string interest;
    private static Dictionary<string, float> knowledgeLevels = new Dictionary<string, float>();

    public static void SetInterest(string interest)
    {
        PlayerData.interest = interest;
    }

    public static string GetInterest()
    {
        return interest;
    }

    public static void SetKnowledge(string topic, float level)
    {
        if (knowledgeLevels.ContainsKey(topic))
        {
            knowledgeLevels[topic] = level;
        }
        else
        {
            knowledgeLevels.Add(topic, level);
        }
    }

    public static float GetKnowledge(string topic)
    {
        if (knowledgeLevels.ContainsKey(topic))
        {
            return knowledgeLevels[topic];
        }
        return 0;
    }
}
