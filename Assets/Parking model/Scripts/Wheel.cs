using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    public bool isWheelInPark;

    // Start is called before the first frame update
    void Start()
    {
        isWheelInPark = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "park")
        {
            isWheelInPark = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "park")
        {
            isWheelInPark = false;
        }
    }
}
