using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float duration = 2.5f;
    public Vector3 offset = new Vector3(-3f, 1.5f, 0);

    private Transform target;
    private Queue<string> messageQueue = new Queue<string>();
    private bool isTyping = false;

    public void Initialize(Transform followTarget)
    {
        target = followTarget;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    public void ShowMessages(List<string> messages)
    {
        foreach (string message in messages)
        {
            messageQueue.Enqueue(message);
        }

        if (!isTyping)
        {
            gameObject.SetActive(true);
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isTyping = true;

        while (messageQueue.Count > 0)
        {
            string nextMessage = messageQueue.Dequeue();
            yield return StartCoroutine(TypeText(nextMessage));
            yield return new WaitForSeconds(duration);
        }

        gameObject.SetActive(false);
        isTyping = false;
    }

    private IEnumerator TypeText(string message)
    {
        messageText.text = "";
        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(0.04f);
        }
    }
}
