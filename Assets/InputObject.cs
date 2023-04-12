using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputObject : MonoBehaviour
{
    public string GetNameInput()
    {
        return GetComponentInChildren<TMP_InputField>().text;

    }
}
