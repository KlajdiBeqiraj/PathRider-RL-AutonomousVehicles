using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMAterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;



    //Ci interessa che ogni volta che finisce un episodio ne inizia un altro 
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-2.5f, 2.5f ), 2 , Random.Range(0f, 9f)) ;
        targetTransform.localPosition = new Vector3(Random.Range(-2.5f, 2.5f ), 1 , Random.Range(-8f, 0f)) ;

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

        transform.localPosition += new Vector3(moveX, 0 , moveZ)*Time.deltaTime*moveSpeed; 
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //in questo caso andiamo a usare le azioni generate dall'agente per modificare la sua posizone
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;



        continousActions[1] = Input.GetAxisRaw("Vertical");
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            //qui viene impostata la ricompensa per il renforcemente learning
            SetReward(+1f);
            floorMeshRenderer.material = winMaterial; 
            EndEpisode(); // nel renforcement esistono gli episodi che dicono quando unasimulazione arriva al termine 
        }


        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            //qui viene impostata la ricompensa per il renforcemente learning
            SetReward(-1f);
            floorMeshRenderer.material = loseMAterial;
            EndEpisode(); // nel renforcement esistono gli episodi che dicono quando unasimulazione arriva al termine 
        }

    }
}
