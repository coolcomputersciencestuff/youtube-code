using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrafficSystem : MonoBehaviour
{
    public List<Car> cars;
    public List<GameObject> lights;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // cars[i].route

        // the shortest distance between a car and the traffic lights
        for(int i=0; i<cars.Count; i++)
        {
            if(cars[i].route=="")
                continue;
            int j = Int32.Parse(cars[i].route.Replace("R", ""))-1;
            Debug.Log(Vector3.Distance(cars[i].gameObject.transform.position, lights[j].transform.position));
        }
    }
}
