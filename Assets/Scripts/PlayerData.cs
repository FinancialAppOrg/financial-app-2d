using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    private static string interest="";
    private static Dictionary<string, float> knowledgeLevels = new Dictionary<string, float>();

    public static void SetInterest(string interest)
    {
        PlayerData.interest = interest;
        Debug.Log($"Interés seleccionado: {interest}");
    }

    public static string GetInterest()
    {
        return interest;
    }
    public static string GetTema()
    {
        string tema;
        return tema="inversion";
    }
    public static string GetNivel()
    {
        string level;
        return level="basico";
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
        Debug.Log($"Nivel de conocimiento actualizado para {topic}: {level}");
    }

    public static float GetKnowledge(string topic)
    {
        if (knowledgeLevels.ContainsKey(topic))
        {
            return knowledgeLevels[topic];
        }
        return 0;
    }
    public static void ResetData()
    {
        interest = "";
        knowledgeLevels.Clear();
        Debug.Log("Datos del jugador reiniciados.");
    }
}
