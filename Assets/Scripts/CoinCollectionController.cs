using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class CoinCollectionController : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform coinContainer;
    private string coinsEndpoint = "https://financeapp-backend-production.up.railway.app/api/v1/coins/user";

    void Start()
    {
        StartCoroutine(FetchCoinsData());
    }

    private IEnumerator FetchCoinsData()
    {
        int userId = PlayerData.GetUserId();
        string url = $"{coinsEndpoint}/{userId}";
        Debug.Log("Fetching coin data from: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Datos de monedas recibidos: " + jsonResponse);

                List<CoinData> coins = JsonUtility.FromJson<CoinDataWrapper>("{\"coins\":" + jsonResponse + "}").coins;
                DisplayCoins(coins);

            }
            else
            {
                Debug.LogError("Error al obtener datos de monedas: " + request.error);
            }
        }
    }

    private void ActivateAllComponents(GameObject obj)
    {
        obj.SetActive(true);

        MonoBehaviour[] behaviors = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour behavior in behaviors)
        {
            if (behavior != null)
                behavior.enabled = true;
        }

        Image image = obj.GetComponent<Image>();
        if (image != null) image.enabled = true;

        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        if (text != null) text.enabled = true;

        foreach (Transform child in obj.transform)
        {
            ActivateAllComponents(child.gameObject);
        }
    }

    private void DisplayCoins(List<CoinData> coins)
    {
        foreach (Transform child in coinContainer)
        {
            Destroy(child.gameObject);
        }
       
        foreach (CoinData coin in coins)
        {
            GameObject coinObject = Instantiate(coinPrefab, coinContainer);
            coinObject.transform.SetAsLastSibling();

            ActivateAllComponents(coinObject);

            Image coinImage = coinObject.transform.Find("Moneda").GetComponent<Image>();
            TextMeshProUGUI coinName = coinObject.transform.Find("Nombre").GetComponent<TextMeshProUGUI>();

            if (coinImage == null || coinName == null)
            {
                Debug.LogError("No se encontraron los componentes necesarios en el prefab.");
                continue;
            }

            StartCoroutine(LoadImage(coin.imagen_url, coinImage));
            coinName.text = $"{coin.nombre}";
        }
    }

    private IEnumerator LoadImage(string url, Image image)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                //image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                if (texture != null)
                {
                    image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.LogError($"La textura descargada desde la URL es nula: {url}");
                    image.sprite = Resources.Load<Sprite>("moneda");
                }
            }
            else
            {
                Debug.LogError($"Error al cargar la imagen desde la URL: {url}. Error: {request.error}");
                //Debug.LogError("Error al cargar la imagen: " + request.error);
                //image.sprite = Sprites.Load<Sprite>("moneda");
                image.sprite = Resources.Load<Sprite>("moneda");
            }
        }
    }

    [System.Serializable]
    public class CoinData
    {
        public int id;
        public string nombre;
        public string imagen_url;
        public int cantidad;
    }

    [System.Serializable]
    public class CoinDataWrapper
    {
        public List<CoinData> coins;
    }
}
