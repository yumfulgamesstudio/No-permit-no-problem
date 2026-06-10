using UnityEngine;

[CreateAssetMenu(fileName = "New Talk To Grandma Task", menuName = "Missions/Talk To Grandma")]
public class Task_TalkToGrandma : Task
{
    private NPCInteractable_Grandma grandmaNpc;
    private bool dialogueFinished;

    public override void StartTask()
    {
        dialogueFinished = false;

        InGameUI.Instance.UpdateTaskInfo(
            "Find grandma and talk to her",
            null
        );

        GrandmaNPC grandmaMarker = FindFirstObjectByType<GrandmaNPC>();

        grandmaNpc = grandmaMarker.GetComponent<NPCInteractable_Grandma>();

        grandmaNpc.ResetDialogue();

        grandmaNpc.OnDialogueFinished -= GrandmaDialogueFinished;
        grandmaNpc.OnDialogueFinished += GrandmaDialogueFinished;
    }

    public override bool TaskCompleted()
    {
        return dialogueFinished;
    }

    public override void EndTask()
    {
        if (grandmaNpc != null)
        {
            grandmaNpc.OnDialogueFinished -= GrandmaDialogueFinished;
        }
    }

    private void GrandmaDialogueFinished()
    {
        dialogueFinished = true;

        if (grandmaNpc != null)
        {
            grandmaNpc.OnDialogueFinished -= GrandmaDialogueFinished;
        }
    }
}