using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private GameObject button = null;
    
    public void ChangeName()
    {
        PlayerPrefs.SetString("Name", nameInputField.text);
        button.SetActive(false);
    }
}
