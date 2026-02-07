using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {
    
    
    [NonSerialized] public int num2 = 10;
    private int _sum = 0;
    [SerializeField] private int pr_sum = 0;
    public int num = 100;

    public int[] numbers = new int [3];

    public List<string> words = new List<string>();

     void Awake() {
        Debug.Log("Awake");
    }

    void Update() {
        Debug.Log("Update");
    }

    void LateUpdate() {
        Debug.Log("LateUpdate");
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
    }

    void Start()
    {
        Debug.Log("Hello there");
    }

    void OnDestroy() {
        Debug.Log("On Destroy");
    }

    private void OnEnable() {
        Debug.Log("OnEnable");
    }

}

    

