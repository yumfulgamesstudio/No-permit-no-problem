using UnityEngine;
using UnityEngine.AI;

public class Car_DamageZone : MonoBehaviour
{
    private Car_Controller carController;

    [SerializeField] private float minSpeedToDamage = 1.5f;
    [SerializeField] private float impactForce = 900f;
    [SerializeField] private float upwardsMultiplier = 0.5f;

    private void Awake()
    {
        carController = GetComponentInParent<Car_Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (carController == null || carController.rb == null)
            return;

        if (carController.rb.linearVelocity.magnitude < minSpeedToDamage)
            return;

        // First check if we hit a ragdoll
        Ragdoll ragdoll = other.GetComponentInParent<Ragdoll>();

        if (ragdoll != null)
        {
            ragdoll.SetRagdoll(true);

            Rigidbody ragdollRb = ragdoll.GetMainRagdollRigidbody();

            if (ragdollRb != null)
            {
                ApplyForce(ragdollRb);
            }

            return;
        }

        // If it is not a ragdoll, hit any Rigidbody
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null)
        {
            ApplyForce(rb);
        }
    }

    private void ApplyForce(Rigidbody rigidbody)
    {
        NavMeshAgent agent = rigidbody.GetComponentInParent<NavMeshAgent>();

        if (agent != null)
        {
            agent.enabled = false;
        }

        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.None;

        Vector3 forceDirection = carController.rb.linearVelocity.normalized;
        forceDirection.y = upwardsMultiplier;

        rigidbody.AddForce(forceDirection.normalized * impactForce, ForceMode.Impulse);
    }
}