using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] Dialogue[] dialogue;
    [SerializeField] DialogueManager dM;
    bool hasTriggered = false;

    void Start()
    {
        dM = DialogueManager.dialogueManager;
    }
    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;
        if(!hasTriggered) {
            hasTriggered = true;
            dM.QueueDialogue(dialogue);
        }
    }
}
