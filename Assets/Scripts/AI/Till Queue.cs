using cakeslice;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Collider))]
public class TillQueue : MonoBehaviour, IInteractable
{
    /// <summary>The queue that is made up by the customers</summary>
    private Queue<GameObject> customerQueue;
    /// <summary>The position that is the front of the Queue</summary>
    private Vector3 QueueFront;

    /// <summary>The Gap between each customer in the queue</summary>
    [SerializeField] private float gapForQueue = 2;

    public string interactionPrompt => customerQueue.Count == 0 ? "No Customers to Serve" : "Press F to Serve Customer";
    public bool interactable => customerQueue.Count > 0;

    // Start is called before the first frame update
    void Start()
    {
        QueueFront = this.transform.position + Vector3.right * 2;
        customerQueue = new Queue<GameObject>();
    }

    /// <summary>
    /// Adds a customer to the queue
    /// </summary>
    /// <param name="newCustomer">The customer to be added to the queue</param>
    public void AddToQueue(GameObject newCustomer)
    {
        customerQueue.Enqueue(newCustomer);
        UpdateTillQueue();
    }

    /// <summary>
    /// The Till gets activated removing the first item in the queue and adjusting positions
    /// </summary>
    public void TillActivation()
    {
        //Returns from function if queue is empty
        if(customerQueue.Count == 0)
        {
            return;
        }
        
        customerQueue.Peek().GetComponent<CustomerAI>().LeftTill();


        //Causes the Customer to leave the Till
        customerQueue.Dequeue();
        
        //Next need to update positions of the customers in the queue
        UpdateTillQueue();
    }

    /// <summary>
    /// Loops through the queue and updates the positions of all of the cutomers in it
    /// </summary>
    private void UpdateTillQueue()
    {
        //newPlayer.SendMessage("UpdateTillQueue", QueueFront)
        int position = 0;
        foreach(GameObject customer in customerQueue)
        {
            Vector3 queuePos = QueueFront + Vector3.forward * position * gapForQueue;
            customer.GetComponent<CustomerAI>().UpdateDestination(queuePos);
            position++;
        }
    }

    public void PrimaryInteract(Interactor interactor)
    {
        TillActivation();
    }
}
