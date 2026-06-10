using UnityEngine;
using UnityEngine.UI;

public class Car_Interact : MonoBehaviour, IInteractable, IHighlightable
{
    private Car_Controller carController;
    private CharacterController playerCharacterController;
    private Transform player;
    private Outline outline;

    private float defaultPlayerScale;

    [Header("Exit details")]
    [SerializeField] private Transform exitPoint;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        SetHighlighted(false);
    }

    private void Start()
    {
        carController = GetComponentInParent<Car_Controller>();
        player = GameManager.Instance.player.transform;
        playerCharacterController = player.GetComponent<CharacterController>();

        if (exitPoint != null)
        {
            MeshRenderer renderer = exitPoint.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.enabled = false;

            SphereCollider collider = exitPoint.GetComponent<SphereCollider>();
            if (collider != null)
                collider.enabled = false;
        }
    }

    public void Interact(Transform interactorTransform)
    {
        GetIntoTheCar();
    }

    public void InteractAlternate(Transform interactorTransform)
    {
        throw new System.NotImplementedException();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void SetHighlighted(bool highlighted)
    {
        if (outline != null)
            outline.enabled = highlighted;
    }

    private void GetIntoTheCar()
    {
        ControlsManager.instance.SwitchToCarControls(this);
        carController.ActivateCar(true);

        defaultPlayerScale = player.localScale.x;

        if (playerCharacterController != null)
            playerCharacterController.enabled = false;

        player.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        player.parent = transform;
        player.localPosition = Vector3.up / 2;
    }

    public void GetOutOfTheCar()
    {
        if (carController.carActive == false)
            return;

        carController.ActivateCar(false);

        player.parent = null;

        if (exitPoint != null)
            player.position = exitPoint.position;
        else
            player.position = transform.position + transform.right * 2f;

        player.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale);

        if (playerCharacterController != null)
            playerCharacterController.enabled = true;

        ControlsManager.instance.SwitchToCharacterControls();
    }

    private void OnDrawGizmos()
    {
        if (exitPoint != null)
        {
            Gizmos.DrawWireSphere(exitPoint.position, 0.2f);
        }
    }
}