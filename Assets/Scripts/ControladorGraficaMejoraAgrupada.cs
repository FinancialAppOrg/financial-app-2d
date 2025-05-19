using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

// Estructura para los datos que leer�s (o simular�s)
public class DatosMejoraTemaPorJuego
{
    public string nombreJuego; // Ej: "Juego 1", "Pr�ctica Alpha"
    public Dictionary<string, float> mejoraPorTema; // Key: nombreTema, Value: mejoraPorcentual

    public DatosMejoraTemaPorJuego(string juego)
    {
        nombreJuego = juego;
        mejoraPorTema = new Dictionary<string, float>();
    }
}

public class ControladorGraficaMejoraAgrupada : MonoBehaviour
{
    public BarChart chart; // Arrastra tu GameObject con el componente BarChart de XCharts aqu�

    // Lista de todos los temas que se mostrar�n en el eje X
    private List<string> nombresDeTemasUnicos = new List<string>();

    // Lista para guardar los datos de mejora de varios juegos
    private List<DatosMejoraTemaPorJuego> datosDeTodosLosJuegos = new List<DatosMejoraTemaPorJuego>();

    void Start()
    {
        if (chart == null)
        {
            // Intenta encontrarlo si no est� asignado, o crea uno din�micamente
            // chart = FindObjectOfType<BarChart>();
            Debug.LogError("El componente BarChart no est� asignado.");
            return;
        }

        CargarYProcesarDatos(); // M�todo para simular o cargar tus datos reales
        ConfigurarGrafica();
    }

    void CargarYProcesarDatos()
    {
        // --- Simulaci�n de Datos ---
        // En un caso real, aqu� leer�as de tu base de datos la tabla "ResultadosJuegoTema"
        // y calcular�as las mejoras porcentuales para cada tema y juego.

        // Juego 1
        DatosMejoraTemaPorJuego juego1 = new DatosMejoraTemaPorJuego("Juego N�1");
        juego1.mejoraPorTema.Add("Ahorro", 20f);    // Mejora del 20% en Ahorro en Juego 1
        juego1.mejoraPorTema.Add("Inversi�n", 15f); // Mejora del 15% en Inversi�n en Juego 1
        juego1.mejoraPorTema.Add("Cr�dito", 5f);
        datosDeTodosLosJuegos.Add(juego1);

        // Juego 2
        DatosMejoraTemaPorJuego juego2 = new DatosMejoraTemaPorJuego("Juego N�2");
        juego2.mejoraPorTema.Add("Ahorro", 35f);
        juego2.mejoraPorTema.Add("Inversi�n", 25f);
        juego2.mejoraPorTema.Add("Cr�dito", 10f);
        datosDeTodosLosJuegos.Add(juego2);

        // Pr�ctica del 17/05
        DatosMejoraTemaPorJuego practicaHoy = new DatosMejoraTemaPorJuego("Pr�ctica 17/05");
        practicaHoy.mejoraPorTema.Add("Ahorro", 50f);
        practicaHoy.mejoraPorTema.Add("Inversi�n", 30f);
        // Nota: "Cr�dito" no tiene datos para este juego, XCharts lo manejar� (barra ausente o cero)
        // O puedes asegurarte de que todos los juegos tengan una entrada (incluso 0) para todos los temas.
        practicaHoy.mejoraPorTema.Add("Presupuesto", 40f); // Un nuevo tema
        datosDeTodosLosJuegos.Add(practicaHoy);


        // Obtener la lista �nica de todos los temas para el eje X
        HashSet<string> temasSet = new HashSet<string>();
        foreach (var datosJuego in datosDeTodosLosJuegos)
        {
            foreach (var temaKey in datosJuego.mejoraPorTema.Keys)
            {
                temasSet.Add(temaKey);
            }
        }
        nombresDeTemasUnicos = new List<string>(temasSet);
        nombresDeTemasUnicos.Sort(); // Opcional: ordenar los temas
    }

    void ConfigurarGrafica()
    {
        if (chart == null || datosDeTodosLosJuegos.Count == 0 || nombresDeTemasUnicos.Count == 0)
        {
            Debug.LogWarning("No hay datos suficientes o la gr�fica no est� configurada.");
            return;
        }

        chart.ClearData(); // Limpia datos y series anteriores

        // 1. Configurar T�tulo (Opcional, se puede hacer en el Inspector)
        chart.EnsureChartComponent<Title>().text = "Evoluci�n de Mejora Porcentual";

        // 2. Configurar Eje X (Temas)
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Category; // Los temas son categor�as
        xAxis.boundaryGap = true;
        xAxis.data.Clear();
        foreach (string nombreTema in nombresDeTemasUnicos)
        {
            xAxis.data.Add(nombreTema);
        }

        // 3. Configurar Eje Y (Mejora Porcentual)
        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;
        //yAxis.showName = true; // Aseg�rate de que el nombre del eje se muestre
        //yAxis.name = "Mejora %"; // Establece el texto del nombre del eje

        // 4. A�adir Series (una serie por cada "Juego")
        foreach (DatosMejoraTemaPorJuego datosJuego in datosDeTodosLosJuegos)
        {
            // A�adir una nueva serie para este juego
            Serie serieJuego = chart.AddSerie<Bar>(datosJuego.nombreJuego); // El nombre del juego es el nombre de la serie
            serieJuego.barGap = 0.1f; // Espacio entre barras del mismo grupo (opcional)
            // serieJuego.barWidth = 20f; // Ancho de la barra (opcional)

            // A�adir datos a esta serie para cada tema
            foreach (string nombreTema in nombresDeTemasUnicos)
            {
                if (datosJuego.mejoraPorTema.TryGetValue(nombreTema, out float mejora))
                {
                    serieJuego.AddData(mejora);
                }
                else
                {
                    // Si un juego no tiene datos para un tema particular, a�ade 0 o un valor nulo
                    // XCharts puede manejar datos faltantes, o puedes poner un 0.
                    serieJuego.AddData(0); // O un valor que indique "sin datos" si XCharts lo soporta as�
                }
            }
        }

        // 5. A�adir Leyenda (Opcional, pero �til para saber qu� color corresponde a qu� juego)
        chart.EnsureChartComponent<Legend>().show = true;

        // 6. Refrescar la gr�fica para que se muestren los cambios
        chart.RefreshChart();
    }
}