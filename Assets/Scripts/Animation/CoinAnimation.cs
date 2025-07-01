using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinAnimation : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField][Range(0, 1)] private float glowChance = 0.4f;
    [SerializeField] private float glowIntensity = 0.8f;
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private Color glowColor = Color.yellow;

    [Header("Referencias")]
    [SerializeField] private Image background;
    [SerializeField] private Image coinImage;
    [SerializeField] private Image glowImage;
    [SerializeField] private TextMeshProUGUI nameText;

    private Color originalCoinColor;
    private bool shouldGlow = false;

    private void Awake()
    {
        originalCoinColor = coinImage.color;
        shouldGlow = (Random.value <= glowChance);

        if (glowImage != null)
        {
            glowImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
            glowImage.transform.localScale = Vector3.one * 1.2f;
        }
    }

    private void Start()
    {
        ShowCoin();

        if (shouldGlow)
        {
            StartCoroutine(GlowEffect());
        }
    }

    private void ShowCoin()
    {
        coinImage.color = originalCoinColor;
    }

    private IEnumerator GlowEffect()
    {
        while (true)
        {
            float time = Time.time * glowSpeed;
            float glowValue = (Mathf.Sin(time) + 1f) * 0.5f;

            Color targetColor = Color.Lerp(originalCoinColor, glowColor, glowValue * glowIntensity);
            coinImage.color = targetColor;

            if (glowImage != null)
            {
                Color glowAlpha = new Color(glowColor.r, glowColor.g, glowColor.b, glowValue * glowIntensity * 0.5f);
                glowImage.color = glowAlpha;

                float scaleMultiplier = 1.2f + (glowValue * 0.1f);
                glowImage.transform.localScale = Vector3.one * scaleMultiplier;
            }

            yield return null;
        }
    }

    public void StopGlow()
    {
        StopAllCoroutines();
        coinImage.color = originalCoinColor;
        if (glowImage != null)
        {
            glowImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
            glowImage.transform.localScale = Vector3.one * 1.2f;
        }
    }
}