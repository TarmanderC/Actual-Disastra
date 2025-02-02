using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class Enemy : MonoBehaviour
{
    public GameObject[] enemies = new GameObject[4];
    private BattleManager bm;
    private KnightBattle player;
    private bool isPlayerClose = false;
    public int goldAmount;
    public List<Collectable> itemAwards = new List<Collectable>();

    [SerializeField] private UnityEvent afterBattleDialogue;
    [SerializeField] private UnityEvent onEnemyDeathDialogue;
    [SerializeField] private UnityEvent onEnemyWinDialogue;

    public Character character;

    public bool hasDialogue;

    private GameObject DialogueBox;
    private DialogueActivator dialogueActivator;

    
    void Awake()
    {
        bm = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        player = GameObject.Find("Player").GetComponent<KnightBattle>();
        DialogueBox = GameObject.FindWithTag("DialogueBox");
        dialogueActivator = GetComponent<DialogueActivator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isPlayerClose && !hasDialogue) {
            TriggerBattle();
        }
    }

    public void TriggerBattle() {
        bm.StartBattle(player.currentParty, enemies, goldAmount, itemAwards, this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayerClose = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayerClose = false;
        }
    }

    public void CheckHealthAndTriggerDialogue()
    {
        float healthPercentage = (float)character.currentHealth / character.maxHealth;
        bool allPlayersDefeated = bm.players.All(player => player == null || player.GetComponent<Character>().currentHealth <= 0);

        if (healthPercentage == 0f)
        {
            onEnemyDeathDialogue.Invoke();

        }

        if (allPlayersDefeated)
        {
            onEnemyWinDialogue.Invoke();
        }

    }

    public void ShowAfterBattleDialogue()
    {
        afterBattleDialogue.Invoke();
    }
}
