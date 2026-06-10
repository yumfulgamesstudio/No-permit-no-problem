using System;
using UnityEngine;
using UnityEngine.Events;

public class NPCInteractable_Grandma : MonoBehaviour, IInteractable, IHighlightable
{
    public event Action OnDialogueFinished;

    [SerializeField] private Transform chatBubblePrefab;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string[] dialogueLines;

    public UnityEvent onInteraction;

    private Outline outline;
    private Animator animator;
    private NPCHeadLookAt npcHeadLookAt;

    private static Transform currentChatBubble;

    private int dialogueIndex = 0;
    private bool dialogueFinished = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        npcHeadLookAt = GetComponent<NPCHeadLookAt>();
        outline = GetComponent<Outline>();

        SetHighlighted(false);
    }

    public void Interact(Transform interactorTransform)
    {
        if (dialogueFinished)
        {
            return;
        }

        if (currentChatBubble != null)
        {
            Destroy(currentChatBubble.gameObject);
            currentChatBubble = null;
        }

        string dialogueText = GetNextDialogueLine();

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

    private string GetNextDialogueLine()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            FinishDialogue();
            return "...";
        }

        string dialogueLine = dialogueLines[dialogueIndex];

        dialogueIndex++;

        if (dialogueIndex >= dialogueLines.Length)
        {
            FinishDialogue();
        }

        return dialogueLine;
    }

    private void FinishDialogue()
    {
        if (dialogueFinished)
        {
            return;
        }

        dialogueFinished = true;
        OnDialogueFinished?.Invoke();
    }

    public void ResetDialogue()
    {
        dialogueIndex = 0;
        dialogueFinished = false;
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
