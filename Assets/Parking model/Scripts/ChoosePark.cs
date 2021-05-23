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

    public ParkingSlot[] freeSlot;

    //private int[] freeSlot_index;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private ParkingSlot[] manageParkedCar()
    {
        
        for (int i = 0; i < parkingSlots.Length; i++)
        {
            if(parkingSlots[i].parkedCar==null)
            {
                return parkingSlots;
            }
            parkingSlots[i].occupiesParking();
        }

        //int n_freeSlot = Random.Range(1, 7);
        int n_freeSlot = Random.Range(1, 7);
        freeSlot = new ParkingSlot[n_freeSlot];

        for (int i= 0; i<n_freeSlot;i++)
        {
            int indx_slot = Random.Range(0,parkingSlots.Length);

            parkingSlots[indx_slot].freeParking();

            freeSlot[i] = parkingSlots[indx_slot];
        }

        return freeSlot;

    }

    public void chooseSlot(Vector3 agentPos)
    {
        ParkingSlot[] freeSlot = manageParkedCar();

        if (instanceIndicator!=null)
        {
            GameObject.Destroy(instanceIndicator);
        }

        int bestIndex = 0;
        float minDist = 1000000000; 

        for(int i=0; i< freeSlot.Length; i++)
        {
            //prendo la distanza tra lo slot i-esimo e l'agente 
            float currentDist = Vector3.Distance(agentPos, freeSlot[i].transform.position);

            if (minDist>currentDist)
            {
                minDist = currentDist;
                bestIndex = i;
            }
        }

        nearestSlot = freeSlot[bestIndex];

        Vector3 indicatorPos = nearestSlot.transform.localPosition;// + new Vector3(0, 0.5f, 0.8f);

        instanceIndicator = GameObject.Instantiate(prefabIndicator,new Vector3(0,0,0), Quaternion.identity).gameObject;

        instanceIndicator.transform.SetParent(Env.transform);

        instanceIndicator.transform.localPosition = indicatorPos;
        
    }


}
