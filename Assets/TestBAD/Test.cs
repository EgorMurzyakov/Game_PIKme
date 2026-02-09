using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class Test : MonoBehaviour {
    
    /*
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
    }*/

    public GameObject obj;

    public Transform target;

    public Light _light;

    public Transform[] transforms = new Transform[3];

    public float speed = 2.0f;

    private void Start()
    {
        // obj.SetActive(false);
        // obj.GetComponent<Transform>().position = new UnityEngine.Vector3(10, 0, 5);


        // target.position =  new UnityEngine.Vector3(10, 5, 5);
        // _light.intensity = 0.5f;
    }


    private void Update()
    {
        for(int i = 0; i < transforms.Length; i++)
        {   
            if (transforms[i] == null)
                continue;
            transforms[i].Translate(new UnityEngine.Vector3(0, 0, 2.0f) * speed * Time.deltaTime);

            float posZ = transforms[i].position.z;
            if(posZ > 15f)
                Destroy(transforms[i].gameObject);
        }
    }


}

    

