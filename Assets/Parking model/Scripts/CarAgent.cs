using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private CarController controller;
    [SerializeField] private ParkingSlot goalParking;
    [SerializeField] private ChoosePark csPark;
    public bool isBreaking;

    public Vector3 position;
    public Vector3 forward; 


    public override void Initialize()
    {
        rigidbody= GetComponent<Rigidbody>();
        controller= GetComponent<CarController>();
        csPark = GetComponent<ChoosePark>();


        //creo dei bool che mi indicano quando le ruote sono all'interno del posteggio 
        isBreaking = false;
    }


    public override void OnEpisodeBegin()
    {
        //aggionare la poszione dell'oggetto nella scena per farlo spawnare random
        float roty = Random.Range(0, 360);
        this.transform.rotation = Quaternion.Euler(0, roty, 0);

        this.transform.localPosition = new Vector3(Random.Range(9.5f, 13f), 6.402f, Random.Range(-7.5f, 9f));
        rigidbody.velocity = new Vector3(0, 0, 0);

        csPark.chooseSlot(this.transform.localPosition);
        goalParking = csPark.nearestSlot;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //vado a osservare sia la posizione del agente sia la posizione del target

        Vector3 dirToTarget = (goalParking.transform.position - transform.position).normalized;
        sensor.AddObservation(transform.position.normalized);
        sensor.AddObservation(
            this.transform.InverseTransformPoint(goalParking.transform.position));
        sensor.AddObservation(
            this.transform.InverseTransformVector(rigidbody.velocity.normalized));
        sensor.AddObservation(
            this.transform.InverseTransformDirection(dirToTarget));
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(transform.right);
        // sensor.AddObservation(StepCount / MaxStep);
        float velocityAlignment = Vector3.Dot(dirToTarget, rigidbody.velocity);
        AddReward(0.001f * velocityAlignment);

    }

    public override void OnActionReceived(ActionBuffers actions) //questa funzione è utlizzata per eseguire le azioni 
    {

        //assegno il vaolore discreto con la barra spaziatrice
        //if (actions.DiscreteActions[0] == 0) { isBreaking = false; }
        //else isBreaking = true;

        controller.GetInput(actions.ContinuousActions[0], actions.ContinuousActions[1], isBreaking);
        controller.updateController();
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //in questo caso andiamo a usare le azioni generate dall'agente per modificare la sua posizone
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        //continousActions[2] = Input.GetAxis("Jump");
        continousActions[1] = Input.GetAxisRaw("Vertical");
        continousActions[0] = Input.GetAxisRaw("Horizontal");


        //assegno il vaolore discreto con la barra spaziatrice
        //if (Input.GetKey(KeyCode.Space)) { discreteActions[0] = 1; }
        //else discreteActions[0] = 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag=="wall")
        {
            AddReward(-0.01f);
            EndEpisode();
        }
        else if (other.tag == "borderPark")
        {
            AddReward(-0.01f);
            EndEpisode();
        }

    }


    public IEnumerator JackpotReward(float bonus)
    {
        if (bonus > 0.2f)
            Debug.LogWarning("Jackpot hit! " + bonus);
        AddReward(0.2f + bonus);
        yield return new WaitForEndOfFrame();
        EndEpisode();
    }

}
