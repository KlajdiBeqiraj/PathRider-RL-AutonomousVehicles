# Esame-Edge-Computing

## 0.Vehicle model: 
In addition to these standard library components , for our problem we had to insert an additional block that can model the behavior of the vehicle called CarController. 

## 1.	Vehicle model: 
The controller (file: CarController) acts on steering, torque (percentage, set via throttle pressure), brake (percentage), reverse (bool, reverse). The controller is set to pass this information to the wheel colliders. For the wheels we use a collider called Wheel collider that designed for ground vehicles. It has collision detection , wheel physics and a tire friction model based on skidding.

## 2.	Agent:
 
The agent (PathCarAgent , ParkingAgent or HorizontalParkinAgnet depending on the type of scenario we are using) is a C# class that is derived directly from the "Agent" class of the ML-Agents library. To configure the agent we need to enter the variables it can measure in the agent file in the "CollectObservations()" function. The inputs provided by lidar or camera are automatically handled by Unity if we have to formally add them in the observations function. This happens if the sensors or camera are placed in the Unityt object that contains the agent or in its "Child":
 

While in the "OnActionReceived" function you pass the agent's outputs to the machine control system. While in the "OnEpisodeBegin" function it is defined how agent should be initialized once a new episode starts.

## 3.	Behavior paramteres: 
Once the agent file is configured we need to configure Behavior parameters in Unity. 
 

It is important to define the name of the agent that will be inserted later in the configuration file. 
The space size parameter defines the number of inputs given to the agent, where these vectors are saved in the C# file as float. 
 
The stacked vectors parameter defines how many input vectors are passed to the network. If this parameter is defined to be 1 then we only pass inputs at that time instant. If instead we have more than one then we take into account also steps before the current one. 


 

In addition we have the "Actions" parameter that defines the size of the agent output. In this case, as said, there are three continuous signals (accelerator pedal in percentage, brake pedal in percentage, steering angle) and a discrete parameter with two possible values (reverse, which tells if the car is moving forward or backward).  

While the Behavior type (default, training, if you do inferencing it is more convenient from the command line) is a parameter that defines how the agent is used.



## 4.	Rewards: 
For rewards the command "AddReward" within the agent flie is used. In the ML-agents documentation it is recommended to add a small negative reward at each step to speed up the convergence of the training. 

## 5.	Config file: 
In order to configure the neural network for our problem we need to set a set of variables that are in a ".yaml" file. In order to explore these parameters let's explore the PathAgent configuration file. 
behaviors:
    Autopark:
        trainer_type: ppo
  max_steps: 1.0e7
        time_horizon: 500 
        summary_freq: 10000

        hyperparameters:
            batch_size: 1024 
            buffer_size: 4048
            learning_rate_schedule: linear
            learning_rate: 1.0e-3

            beta: 1e-5 
            epsilon: 0.2  
            lambd: 0.93 
            num_epoch: 3 
            
        network_settings:
            hidden_units: 512
            normalize: true
            num_layers: 3
            vis_encode_type: simple

            memory:
                memory_size: 128
                sequence_length: 64

        reward_signals:
            #curiosity module
            curiosity
                strength: 0.1
                gamma: 0.995
                learning_rate: 3.0e-4

It is important to insert the name that in this case is Autopark consistent with what we have entered in the Behavior parameter under Behavior name. 
Below we are going to describe what the various parameters are and what is the typical range recommended by the library documentation. 

#### GENERAL PARAMETERS: 
1) trainer_type : the approach type used , in this case we are using ppo
2) summary_freq (def 50000): number of experiments before displaying mean reward and variance statistics.
3) time_horizon (64): number of steps the agent collects before adding to experience buffer. 
4) Max step : number of maximum simulations 



#### HYPERPARAMETERS: 
1) Learning rate (typically 3e-4): learning rate , corresponds to the size of a step in the gradient descent 
2) Bath size (with continuous PPO typically 512->5120): number of iterations for gradient descent. Less than buffer size. 
3) Buffer size (for PPO ranges from 2048- 409600): Corresponds to how many experiences should be collected before doing any learning or updating of the model. This should be several times larger than batch_size. 
4) learning_rate_schedule: (typically for ppo is linear): Determines how the learning rate changes over time. For PPO, it is recommended to decay the learning rate to max_steps so that learning converges more steadily. However, for some cases (e.g. training for an unknown amount of time) this feature can be disabled.


#### PPO HYPERPARAMETERS: 
1) Beta (Range 1e-4 - 1e-2 ): This ensures that agents correctly explore the action space during training. Increasing this value ensures that more random actions are taken. This should be adjusted so that the entropy (measured by TensorBoard) slowly decreases along with the increase in reward. If the entropy drops too quickly, increase the beta. If entropy drops too slowly, decrease beta.
2) Epsilon (range 0.1 to 0.3): Influences how fast the policy can evolve during training. Corresponds to the acceptable threshold of divergence between the old and new policy during the gradient descent update. Setting this value small will result in more stable updates, but will also slow down the training process.
3) Lambd (range 0.9-0.95) : Low values correspond to relying more on the current value estimate (which can be high bias), and high values correspond to relying more on the actual rewards received in the environment (which can be high variance).
4) Num epoch (range 3 ->7 ): Number of passes to make through the experience buffer when performing gradient descent optimization.The larger the batch_size, the larger it is acceptable to do so. Decreasing this value will ensure more stable updates, at the cost of slower learning.

#### Curiosity:
1) Strength (range 0.001 -0.1): Magnitude of the curiosity reward generated by the intrinsic curiosity module. This should be scaled to ensure that it is large enough not to be overwhelmed by extrinsic reward signals in the environment. Similarly, it should not be too large to overwhelm the extrinsic reward signal.
2) Range (range 0.8 -0.995): a discount factor for future rewards 
3) Learning rate : Learning rate used to update the intrinsic curiosity module. This should typically be decreased if training is unstable, and curiosity loss is unstable.

#### Memory: 
1) Memory_size (range 32 -256) : Memory size that an agent must maintain. To use an LSTM, training requires a sequence of experiences instead of single experiences. Corresponds to the size of the floating point number array used to store the hidden state of the policy's recurrent neural network. This value should be a multiple of 2, and should scale with the amount of information the agent is expected to need to remember to successfully complete the task.
2) Seqeunce lenght (range 4 -128): Defines how long the sequences of experiences should be during training. Note that if this number is too small, the agent will not be able to remember things for longer periods of time. If this number is too large, the neural network will take longer to train.

Information on other parameters can be found at the following links: 
1. https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Training-ML-Agents.md#training-with-mlagents-learn 
2. https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Training-Configuration-File.md#self-play


## 6.	Interaction with ML-Agents:
For the training phase you have to open a command prompt in the project folder and use the following command: 
"mlagents-learn NAME_FILE_CONFIG.yaml --run-id=ID_SIMULATION".
And add the following commands: 
1. "--resume": to continue a simulation already started 
2. "--force": to reset a simulation already started and to start the training
3. "--initialize": initialize the current model with a previously trained model 
4. "--inference": you go to use the trained model to make of the inference 

For the phase of formation more models of the same environment are inserted in order to be able to accelerate the convergence. 

For the phase of training more models of the same environment are inserted in order to accelerate the convergence. 
 

Once made to start the simulation with the command previously made to see, the results of the simulations made using TensorBoard can be seen. In order to open it the prompt on the folder of the project and go to insert the command: 
"tensorboard --logdir results --port 6006".

The training produces an encoded model in an .onnx file, which is transformable to Tensorflow. This allows you to both view the model, and possibly modify it. https://github.com/onnx/onnx-tensorflow.
More information in this book https://learning.oreilly.com/library/view/learn-unity-ml-agents/9781789138139/
