using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelScreen : MonoBehaviour
{
    [SerializeField] Button[] levelButtons;

    void Start()
    {
        foreach (Button button in levelButtons)
        {
            button.onClick.AddListener(() => OnLevelSelected(button.name));
        }

    }

    void OnLevelSelected(string level)
    {
        Debug.Log("Level selected: " + level);
        FindObjectOfType<GameManager>().StartQuizWithLevel(level);
    }

    
}
