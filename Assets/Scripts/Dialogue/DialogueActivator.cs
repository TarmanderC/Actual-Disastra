using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    private GameManager gameManager;
    [SerializeField] private DialogueResponseEvents responseEvents;
    public bool isCutscene;
    private Collider2D dialogueCollider;

    [Header("Activation Conditions (Optional)")]
    public bool requiresDecisionCheck = false;
    public GameManager.FirstDecision requiredFirstDecision = GameManager.FirstDecision.None;
    public GameManager.SecondDecision requiredSecondDecision = GameManager.SecondDecision.None;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        dialogueCollider = GetComponent<Collider2D>(); // Get the collider on this object
    }

    public void DisableDialogue()
    {
        if (dialogueCollider != null)
        {
            dialogueCollider.enabled = false; // Disable the trigger collider
        }
    }


    public void UpdateDialogueObject(DialogueObject dialogueObject) {
        this.dialogueObject = dialogueObject;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && other.TryGetComponent(out KnightMovement player)) {
            player.Interactable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && other.TryGetComponent(out KnightMovement player)) {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this) {
                player.Interactable = null;
            }
        }
    }

    public void Interact(KnightMovement player) {
        
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>()) {
            if (responseEvents != null && responseEvents.DialogueObject == dialogueObject) {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        if (CanActivateDialogue()) {
            this.gameObject.SetActive(true);
            player.DialogueUI.ShowDialogue(dialogueObject);
        } else {
            this.gameObject.SetActive(false);
        }
    }

    private bool CanActivateDialogue()
    {
        if (!requiresDecisionCheck) return true;

        return (gameManager.firstDecision == requiredFirstDecision || requiredFirstDecision == GameManager.FirstDecision.None) &&
            (gameManager.secondDecision == requiredSecondDecision || requiredSecondDecision == GameManager.SecondDecision.None);
    }
}
