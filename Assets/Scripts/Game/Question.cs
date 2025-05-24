using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Question
{
    public int id_quizz;
    public string nivel;
    public string opcion_1;
    public string opcion_3;
    public int opcion_correcta;
    public string tema;
    public int id_pregunta;
    public string texto_pregunta;
    public string opcion_2;
    public string opcion_4;
    
    public string[] GetAnswers()
    {
        return new string[] { opcion_1, opcion_2, opcion_3, opcion_4 };
    }
}


[Serializable]
public class AnswerRequest
{
    public int id_quizz;
    public int id_pregunta;
    public int opcion_elegida;
}




