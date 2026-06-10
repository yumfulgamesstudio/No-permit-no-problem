using UnityEngine;
using UnityEngine.Events;

public class NPCInteractable_Regular : MonoBehaviour, IInteractable, IHighlightable
{
    [SerializeField] private Transform chatBubblePrefab;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string[] dialogueLines;

    public UnityEvent onInteraction;

    private Outline outline;
    private Animator animator;
    private NPCHeadLookAt npcHeadLookAt;

    private static Transform currentChatBubble;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        npcHeadLookAt = GetComponent<NPCHeadLookAt>();
        outline = GetComponent<Outline>();

        SetHighlighted(false);
    }

    public void Interact(Transform interactorTransform)
    {
        if (currentChatBubble != null)
        {
            Destroy(currentChatBubble.gameObject);
            currentChatBubble = null;
        }

        string dialogueText = GetRandomDialogueLine();

        currentChatBubble = ChatBubble3D.Create(
            transform,
            new Vector3(0f, 2.1f, 0f),
            chatBubblePrefab,
            dialogueText
        );

        float playerHeight = 2f;

        if (npcHeadLookAt != null)
        {
            npcHeadLookAt.LookAtPosition(interactorTransform.position + Vector3.up * playerHeight);
        }

        onInteraction.Invoke();
    }

    private string GetRandomDialogueLine()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            return "...";
        }

        int randomIndex = Random.Range(0, dialogueLines.Length);
        return dialogueLines[randomIndex];
    }

    public void SetHighlighted(bool highlighted)
    {
        if (outline != null)
        {
            outline.enabled = highlighted;
        }
    }

    public void InteractAlternate(Transform interactorTransform)
    {
        // No alternate interaction for NPC
    }

    public Transform GetTransform()
    {
        return transform;
    }
}