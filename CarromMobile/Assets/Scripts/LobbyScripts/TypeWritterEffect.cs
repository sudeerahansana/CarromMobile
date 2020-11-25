using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypeWritterEffect : MonoBehaviour
{
    private string text = "Enter Room ID ";
    private string currentText;
    private TextMeshProUGUI textField;
     private void OnEnable()
    {
        textField = GetComponent<TextMeshProUGUI>();
        //Invoke("StartAnime",1f);
        StartAnime();
    }
    private void StartAnime()
    {
        StartCoroutine(ShowText());
    }
    IEnumerator ShowText()
    {
        for (int i = 0; i < text.Length; i++)
        {
            currentText = text.Substring(0, i);
            textField.text= currentText;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
