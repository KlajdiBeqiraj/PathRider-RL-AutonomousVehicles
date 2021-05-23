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

    public Vector3 position;
    public Vector3 forward; 


    public override void Initialize()
    {
        rigidbody= GetComponent<Rigidbody>();
        controller= GetComponent<CarController>();
        csPark = GetComponent<ChoosePark>();
    }


    public override void OnEpisodeBegin()
    {
        //aggionare la poszione dell'oggetto nella scena per farlo spawnare random
        float roty = Random.Range(0, 360);
        this.transform.rotation = Quaternion.Euler(0, roty, 0);

        this.transform.localPosition = new Vector3(Random.Range(11.5f, 13f), 6.402f, Random.Range(-7.5f, 9f));
        rigidbody.velocity = new Vector3(0, 0, 0);

        csPark.chooseSlot(this.transform.position);
        goalParking = csPark.nearestSlot;


        float posz = goalParking.transform.position.z + Random.Range(-2, 2);

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, posz);

        controller.GetInput(0, 0, 0, 0);

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

        controller.GetInput(Mathf.Abs(actions.ContinuousActions[1]), actions.ContinuousActions[0], Mathf.Abs(actions.ContinuousActions[2]), actions.DiscreteActions[0]);
        controller.updateController();
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //in questo caso andiamo a usare le azioni generate dall'agente per modificare la sua posizone
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;


        float accPedal = Mathf.Clamp(System.Convert.ToSingle(Input.GetKey(KeyCode.W)) * Time.deltaTime * 100.0f, 0f, 100f);
        float breakPedal = Mathf.Clamp(System.Convert.ToSingle(Input.GetKey(KeyCode.Space)) * Time.deltaTime * 100.0f, 0f, 100f);

        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = accPedal;
        continousActions[2] = breakPedal;

        discreteActions[0] = System.Convert.ToInt32(Input.GetKey(KeyCode.E));

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

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "car")
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
