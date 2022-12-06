using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particlebehavior : MonoBehaviour
{
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    // Start is called before the first frame update
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();

    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("collision");
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        FirstPersonController fpc = other.GetComponent<FirstPersonController>();
        Rigidbody rb = other.GetComponent<Rigidbody>();
        int i = 0;

        while (i < numCollisionEvents)
        {
            if (fpc)
            {
                Vector3 pos = collisionEvents[i].intersection;
                Vector3 pusher = new Vector3((rb.position.x - pos.x)*1000, 0, (rb.position.z - pos.z) * 1000);
                rb.AddForce(pusher);
            }
            i++;
        }
    }

    void openSesame()
    {
        var emis = part.emission;
        emis.rateOverTime = 0;
        BoxCollider bx = GetComponentInChildren<BoxCollider>();
        bx.enabled = false;
    }
}
