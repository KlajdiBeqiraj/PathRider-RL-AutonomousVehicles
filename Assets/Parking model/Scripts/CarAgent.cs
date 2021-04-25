using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    [SerializeField] private Transform targetTransform;


    public override void OnEpisodeBegin()
    {
        //aggionare la poszione dell'oggetto nella scena per farlo spawnare random
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //vado a osservare sia la posizione del agente sia la posizione del target
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
