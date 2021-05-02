using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    [SerializeField] private Transform targetTransform;

    [SerializeField] private CarController controller;


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
        bool isBreaking;
        //assegno il vaolore discreto con la barra spaziatrice
        if (actions.DiscreteActions[0]==0) { isBreaking = false; }
        else isBreaking = true;

        controller.GetInput(actions.ContinuousActions[0], actions.ContinuousActions[1], false);
        controller.updateControler();
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //in questo caso andiamo a usare le azioni generate dall'agente per modificare la sua posizone
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        continousActions[1] = Input.GetAxisRaw("Vertical");
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        
        //assegno il vaolore discreto con la barra spaziatrice
        if(Input.GetKey(KeyCode.Space)) { discreteActions[0] = 1; }
        else discreteActions[0] = 0;
    }

    
}
