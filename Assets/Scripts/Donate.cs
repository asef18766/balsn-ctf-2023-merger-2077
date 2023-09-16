using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Donate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            string formURL = "https://forms.gle/HJtnEbVJufuaRxAg9";
            Application.OpenURL(formURL);
        });
    }
}
