using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNameForUI : MonoBehaviour
{

    void Start()
    {
        Text label = GetComponent<Text>();
        if(label != null )
            label.text = SceneManager.GetActiveScene().name;    
    }
}
