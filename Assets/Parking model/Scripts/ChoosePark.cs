using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosePark : MonoBehaviour
{
    // Start is called before the first frame update
    public ParkingSlot[] parkingSlots;

    public ParkingSlot nearestSlot;

    public GameObject prefabIndicator;

    public GameObject instanceIndicator;

    public GameObject Env; 


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void chooseSlot(Vector3 agentPos)
    {
        if(instanceIndicator!=null)
        {
            GameObject.Destroy(instanceIndicator);
        }

        int bestIndex = 0;
        float minDist = 1000000000; 

        for(int i=0; i< parkingSlots.Length; i++)
        {
            //prendo la distanza tra lo slot i-esimo e l'agente 
            float currentDist = Vector3.Distance(agentPos, parkingSlots[i].transform.localPosition);

            if (minDist>currentDist)
            {
                minDist = currentDist;
                bestIndex = i;
            }
        }

        nearestSlot = parkingSlots[bestIndex];

        Vector3 indicatorPos = nearestSlot.transform.localPosition + new Vector3(0, 0.5f, 0.8f);

        instanceIndicator = GameObject.Instantiate(prefabIndicator,new Vector3(0,0,0), Quaternion.identity).gameObject;

        instanceIndicator.transform.SetParent(Env.transform);

        instanceIndicator.transform.localPosition = indicatorPos;
        
    }


}
