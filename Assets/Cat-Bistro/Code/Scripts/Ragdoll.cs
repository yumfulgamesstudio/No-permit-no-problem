using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    [Header("Main NPC Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private Rigidbody mainRigidbody;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (mainCollider == null)
            mainCollider = GetComponent<Collider>();

        if (mainRigidbody == null)
            mainRigidbody = GetComponent<Rigidbody>();

        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        SetRagdoll(false);
    }

    public void SetRagdoll(bool active)
    {
        if (animator != null)
            animator.enabled = !active;

        if (mainCollider != null)
            mainCollider.enabled = !active;

        if (mainRigidbody != null)
            mainRigidbody.isKinematic = true;

        foreach (Collider cd in ragdollColliders)
        {
            if (cd != mainCollider)
                cd.enabled = active;
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != mainRigidbody)
            {
                rb.isKinematic = !active;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }

    public Rigidbody GetMainRagdollRigidbody()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != mainRigidbody)
                return rb;
        }

        return null;
    }
}