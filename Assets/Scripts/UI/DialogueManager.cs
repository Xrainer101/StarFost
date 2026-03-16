using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Dialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portrait;
    AudioSource audioSource;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public bool CanDialogue()
    {
        return !isDialogueActive;
    }
    void Start()
    {
        Dialogue();
    }
    public void Dialogue()
    {
        if (dialogueData == null /* || game paused */) 
        {   
            return;
        }

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }
    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        nameText.SetText(dialogueData.characterName);
        portrait.sprite = dialogueData.portrait;
        
        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());

    }
    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");
        string line = dialogueData.dialogueLines[dialogueIndex];
        audioSource.PlayOneShot(dialogueData.voiceSound, dialogueData.voicePitch);
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        isTyping = false;

        yield return new WaitForSeconds(dialogueData.autoProgressDelay);
        NextLine();
    }
    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        //unpause game if paused
    }
}
