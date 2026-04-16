using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Dialogue dialogueData;
    public Dialogue[] dialogueLines;
    private Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();

    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portrait;
    
    public Animator portraitAnimator;

    AudioSource audioSource;
    Animator animator;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private Coroutine typingCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = dialoguePanel.GetComponent<Animator>();
    }
    public bool CanDialogue()
    {
        return !isDialogueActive;
    }
    //temp for testing
    void Start()
    {
        Dialogue();
        QueueDialogue(dialogueLines);
    }
    public void QueueDialogue(Dialogue[] newDialogues)
    {
        foreach (Dialogue dialogue in newDialogues)
        {
            QueueDialogue(dialogue);
        }
    }
    public void QueueDialogue(Dialogue newDialogue)
    {
        if (!isDialogueActive)
        {
            Dialogue(newDialogue);
        }
        else
        {
            dialogueQueue.Enqueue(newDialogue);
        }
    }
    public void SetDialogue(Dialogue newDialogue)
    {
        dialogueData = newDialogue;
    }
    //call from other places with correct dialogue
    public void Dialogue(Dialogue newDialogue)
    {
        SetDialogue(newDialogue);
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
    void Clear() {
        dialogueText.SetText("");
        nameText.SetText("");
    }
    
    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        Clear();
        //can set portarit before because static is over it
        portrait.sprite = dialogueData.characterData.portrait;
        nameText.SetText(dialogueData.characterData.characterName);

        //set other character specific info
        portraitAnimator.runtimeAnimatorController = dialogueData.characterData.animatorController;
        StartCoroutine(WaitForAnimation("Appear", true));
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            typingCoroutine = StartCoroutine(TypeLine());
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
        portraitAnimator.SetBool("isTalking", true);
        foreach (char letter in line.ToCharArray())
        {
            PlayVoice();
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        portraitAnimator.SetBool("isTalking", false);
        isTyping = false;

        yield return new WaitForSeconds(dialogueData.autoProgressDelay);
        NextLine();
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        Clear();
        StartCoroutine(WaitForAnimation("Disappear"));
        //unpause game if paused
    }

    private IEnumerator WaitForAnimation(string triggerName, bool start = false)
    {
        animator.SetTrigger(triggerName);
        yield return null;
        // Wait until the animation has finished
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(triggerName))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // Animation finished, proceed with next actions
        if (start) {
            typingCoroutine = StartCoroutine(TypeLine());
            StartCoroutine(BlinkRoutine());
        }
        if(!start) {
            isDialogueActive = false;
            if (dialogueQueue.Count > 0)
            {
                Dialogue next = dialogueQueue.Dequeue();
                Dialogue(next);
            }
        }
    }
    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            portraitAnimator.SetTrigger("Blink");
        }
    }
    private void PlayVoice()
    {
        if (dialogueData.characterData.voiceSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f) * dialogueData.characterData.voicePitch;
            audioSource.PlayOneShot(dialogueData.characterData.voiceSound);
        }
    }
}