using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticButtonSizer : MonoBehaviour {

    public float childWidth = 35f;
    
    // Start is called before the first frame update
    void Start() {
        ResizeButtons();
    }

    // Update is called once per frame
    void Update() {
    }

    void ResizeButtons() {
        Debug.Log("Resizing child buttons");
        var sizeDelta = GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = transform.childCount * childWidth;
        
        GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}