using cakeslice;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Collider))]
public class TillQueue : MonoBehaviour, IInteractable
{
    /// <summary>The queue that is made up by the customers</summary>
    private Queue<CustomerAI> customerQueue = new Queue<CustomerAI>();
    /// <summary>The position that is the front of the Queue</summary>
    [SerializeField] private Vector3 queueOffset;
    /// <summary> The step direction of the queue from <see cref="queueOffset"/> </summary>
    [SerializeField] private Vector3 queueDirection = Vector3.forward * 2;
    /// <summary> The absolute position of the front of the queue </summary>
    private Vector3 queueFront => transform.position + queueOffset;

    public bool outline => customerQueue.Count > 0 && customerQueue.Peek().finishedWalking;
    public string interactionPrompt => customerQueue.Count > 0 ? customerQueue.Peek().finishedWalking ? "Press F to Serve Customer" : "Waiting for Customer" : "No Customers to Serve";

    /// <summary>
    /// Adds a customer to the queue
    /// </summary>
    /// <param name="newCustomer">The customer to be added to the queue</param>
    public void AddToQueue(CustomerAI newCustomer)
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
        if (customerQueue.Count == 0) return;

        //Causes the Customer to leave the Till
        customerQueue.Dequeue().LeaveShop();
        
        //Next need to update positions of the customers in the queue
        UpdateTillQueue();
    }

    /// <summary>
    /// Loops through the queue and updates the positions of all of the cutomers in it
    /// </summary>
    private void UpdateTillQueue()
    {
        int position = 0;
        foreach (CustomerAI customer in customerQueue)
        {
            Vector3 queuePos = queueFront + position * queueDirection;
            customer.SetDestination(queuePos);
            position++;
        }
    }

    public void PrimaryInteract(Interactor interactor)
    {
        if (outline) TillActivation();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Mesh grid = new GridMesh().Build(new Vector2Int(1, 5), Vector2.one * queueDirection.magnitude, 0.4f);
        Vector3 position = queueFront + queueDirection * 5 * 0.5f - queueDirection * 0.5f;
        Gizmos.DrawMesh(grid, position, Quaternion.LookRotation(queueDirection));
    }
#endif
}
