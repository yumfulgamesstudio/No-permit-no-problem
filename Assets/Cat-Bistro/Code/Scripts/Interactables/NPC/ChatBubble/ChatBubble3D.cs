using TMPro;
using UnityEngine;

public class ChatBubble3D : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;

    public static Transform Create(
        Transform parent,
        Vector3 localPosition,
        Transform chatBubblePrefab,
        string text
    )
    {
        if (parent == null)
        {
            Debug.LogError("ChatBubble3D.Create: parent is null!");
            return null;
        }

        if (chatBubblePrefab == null)
        {
            Debug.LogError("ChatBubble3D.Create: chatBubblePrefab is null! Assign it in the Inspector.");
            return null;
        }

        Transform chatBubbleTransform = Instantiate(chatBubblePrefab, parent);
        chatBubbleTransform.localPosition = localPosition;

        ChatBubble3D chatBubble = chatBubbleTransform.GetComponent<ChatBubble3D>();

        if (chatBubble == null)
        {
            Debug.LogError("ChatBubble3D.Create: Prefab does not have ChatBubble3D component attached.");
            Destroy(chatBubbleTransform.gameObject);
            return null;
        }

        chatBubble.Setup(text);

        Destroy(chatBubbleTransform.gameObject, 6f);

        return chatBubbleTransform;
    }

    public void Setup(string text)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = text;
        }
        else
        {
            Debug.LogError("ChatBubble3D: textMeshPro is not assigned in the Inspector.");
        }
    }
}