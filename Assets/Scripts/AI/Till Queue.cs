using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TillQueue : MonoBehaviour
{
    private Queue<GameObject> customerQueue;
    private Vector3 QueueFront;

    [SerializeField] private float gapForQueue;

    // Start is called before the first frame update
    void Start()
    {
        QueueFront = this.transform.position + Vector3.right * 2;
        customerQueue = new Queue<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToQueue(GameObject newPlayer)
    {
        customerQueue.Enqueue(newPlayer);
        UpdateTillQueue();
    }

    public void TillActivation()
    {
        //Causes the Customer to leave the Till
        customerQueue.Dequeue();
        
        //Next need to update positions of the customers in the queue
        UpdateTillQueue();
    }

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
}
