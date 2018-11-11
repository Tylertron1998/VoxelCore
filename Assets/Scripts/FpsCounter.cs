using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	float deltaTime = 0.0f;

	private TextMeshProUGUI _text; 
	
    void Start()
    {
	    _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
	    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	    _text.text = $"{Mathf.Round(1.0f / deltaTime)}fps";
    }
}
