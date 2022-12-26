using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinCondition : MonoBehaviour
{
    [SerializeField] Transform enemyTransform;

    [SerializeField] HordeSpawner triggerOne;
    [SerializeField] HordeSpawner triggerTwo;

    [SerializeField] TMP_Text numberText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        numberText.text = "Remaining Enemies: " + enemyTransform.childCount.ToString();

        if (triggerOne.triggered && triggerTwo.triggered)
        {
            if (enemyTransform.childCount == 0)
            {
                Invoke("WinScene", .3f);
            }
        }
    }

    private void WinScene()
    {
        SceneManager.LoadScene("WinScene");
        Cursor.lockState = CursorLockMode.None;
    }
}
