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

    public override void OnActionReceived(ActionBuffers actions) //questa funzione è utlizzata per eseguire le azioni 
    {

        //definisco due variabili per lo spostamento
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        //definisco una variabile che mi definsce la velocità dell'agente
        float moveSpeed = 2.5f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //in questo caso andiamo a usare le azioni generate dall'agente per modificare la sua posizone
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;

        continousActions[1] = Input.GetAxisRaw("Vertical");
        continousActions[0] = Input.GetAxisRaw("Horizontal");

    }

    
}
