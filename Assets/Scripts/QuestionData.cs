using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionData
{
    public int id_pregunta;
    public string texto_pregunta;
    public string opcion_1;  
    public string opcion_2;
    public string opcion_3;
    public string opcion_4;
    public int opcion_correcta; 

    public List<string> opciones
    {
        get
        {
            var options = new List<string>();
            if (!string.IsNullOrEmpty(opcion_1)) options.Add(opcion_1);
            if (!string.IsNullOrEmpty(opcion_2)) options.Add(opcion_2);
            if (!string.IsNullOrEmpty(opcion_3)) options.Add(opcion_3);
            if (!string.IsNullOrEmpty(opcion_4)) options.Add(opcion_4);
            return options;
        }
    }
}
