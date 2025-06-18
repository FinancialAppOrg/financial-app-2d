using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;

[System.Serializable]
public class ProgressHistoryResponse
{
    public string tema;
    public float? evaluacion;
    public float? juego;
    public float? quizz;
}

public class ControladorGraficaMejoraAgrupada : MonoBehaviour
{
    public BarChart chart;
    
    public bool cargarDatosEnStart = true;

    private List<string> nombresDeTemasUnicos = new List<string>();
    private List<ProgressHistoryResponse> datosBackend = new List<ProgressHistoryResponse>();

    private List<string> temasDefault = new List<string>
    {
        "ahorro",
        "inversión",
        "credito-deudas",
    };

    void Start()
    {
        if (chart == null)
        {
            chart = FindObjectOfType<BarChart>();
            if (chart == null)
            {
                Debug.LogError("El componente BarChart no está asignado y no se encontró en la escena.");
                return;
            }
        }

        if (cargarDatosEnStart)
        {
            StartCoroutine(CargarDatosDesdeBackend());
        }
    }

    IEnumerator CargarDatosDesdeBackend()
    {
        int userId = PlayerData.GetUserId(); 
        Debug.Log($"ID de usuario obtenido: {userId}");

        if (userId <= 0)
        {
            Debug.LogError("ID de usuario no válido. Asegúrate de que el usuario esté correctamente autenticado.");
            yield break;
        }

        string apiUrl = "https://financeapp-backend-production.up.railway.app/api/v1/progress/history/" + userId;
        Debug.Log($"URL de la API: {apiUrl}");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error al cargar datos: {webRequest.error}");
                Debug.LogError($"Respuesta del servidor: {webRequest.downloadHandler.text}");

                ConfigurarDatosPorDefecto();
                yield break;
            }

            try
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"JSON recibido: {jsonResponse}");

                if (string.IsNullOrEmpty(jsonResponse) || jsonResponse.Trim() == "[]" || jsonResponse.Trim() == "null")
                {
                    Debug.Log("Respuesta vacía del backend. Configurando datos por defecto.");
                    ConfigurarDatosPorDefecto();
                    yield break;
                }

                //string wrappedJson = "{\"items\":" + jsonResponse + "}";
                //ProgressHistoryListWrapper wrapper = JsonUtility.FromJson<ProgressHistoryListWrapper>(wrappedJson);
                datosBackend = JsonConvert.DeserializeObject<List<ProgressHistoryResponse>>(jsonResponse);
                //datosBackend = wrapper.items;

                if (datosBackend == null || datosBackend.Count == 0)
                {
                    Debug.Log("No hay datos en la respuesta del backend. Configurando datos por defecto.");
                    ConfigurarDatosPorDefecto();
                    yield break;
                }

                //HashSet<string> temasSet = new HashSet<string>();
                //foreach (var item in datosBackend)
                //{
                //    if (!string.IsNullOrEmpty(item.tema))
                //    {
                //        temasSet.Add(item.tema);
                //    }
                //}
                //nombresDeTemasUnicos = new List<string>(temasSet);
                //nombresDeTemasUnicos.Sort();
                //
                //ConfigurarGrafica();
                ProcesarDatosBackend();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error al procesar los datos: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");

                ConfigurarDatosPorDefecto();
            }
        }
    }

    void ConfigurarDatosPorDefecto()
    {
        Debug.Log("Configurando gráfica con datos por defecto (todos en 0).");

        datosBackend.Clear();
        nombresDeTemasUnicos.Clear();

        nombresDeTemasUnicos = new List<string>(temasDefault);

        foreach (string tema in temasDefault)
        {
            datosBackend.Add(new ProgressHistoryResponse
            {
                tema = tema,
                evaluacion = 0f,
                juego = 0f,
                quizz = 0f
            });
        }

        ConfigurarGrafica();
    }

    void ProcesarDatosBackend()
    {
        HashSet<string> temasSet = new HashSet<string>();

        foreach (string tema in temasDefault)
        {
            temasSet.Add(tema);
        }

        foreach (var item in datosBackend)
        {
            if (!string.IsNullOrEmpty(item.tema))
            {
                temasSet.Add(item.tema);
            }
        }

        nombresDeTemasUnicos = new List<string>(temasSet);
        nombresDeTemasUnicos.Sort();

        ConfigurarGrafica();
    }

    [System.Serializable]
    private class ProgressHistoryListWrapper
    {
        public List<ProgressHistoryResponse> items;
    }

    void ConfigurarGrafica()
    {
        if (chart == null || datosBackend.Count == 0 || nombresDeTemasUnicos.Count == 0)
        {
            Debug.LogWarning("No hay datos suficientes o la gráfica no está configurada.");
            return;
        }

        chart.RemoveAllSerie();
        chart.ClearData();

        chart.EnsureChartComponent<Title>().text = "Progreso del Usuario (%)";

        // Eje X (Temas)
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category;
        xAxis.data = nombresDeTemasUnicos;
        xAxis.splitLine.show = false;

        // Eje Y (Mejora Porcentual)
        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;
        yAxis.axisName.show = true;
        yAxis.axisName.name = "%";
        yAxis.min = 0;
        yAxis.splitLine.show = true;

        AddSerie("Evaluación", "evaluacion");
        AddSerie("Juego", "juego");
        AddSerie("Quizz", "quizz");

        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = true;
        legend.itemGap = 10;

        chart.EnsureChartComponent<Tooltip>().show = true;

        chart.RefreshChart();
    }

    void AddSerie(string nombreSerie, string campoDatos)
    {
        Serie serie = chart.AddSerie<Bar>(nombreSerie);
        serie.serieName = nombreSerie;

        serie.barWidth = 0.2f;
        serie.barGap = 0.1f;
        serie.showDataName = false;

        foreach (string tema in nombresDeTemasUnicos)
        {
            var registro = datosBackend.Find(r => r.tema == tema);
            float valor = 0f;

            if (registro != null)
            {
                switch (campoDatos)
                {
                    case "evaluacion": valor = registro.evaluacion ?? 0; break;
                    case "juego": valor = registro.juego ?? 0; break;
                    case "quizz": valor = registro.quizz ?? 0; break;
                }
            }

            serie.AddData(valor);
        }
    }

    public void RecargarDatos()
    {
        StartCoroutine(CargarDatosDesdeBackend());
    }
}
