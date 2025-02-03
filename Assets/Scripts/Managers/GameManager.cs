using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public enum CurrentStage {
        firstChoice,
        secondChoice,
        finalChoice
    }
    public enum FirstDecision {
        None,
        SpareWindBoss,
        KillWindBoss
    }
    public enum SecondDecision {
        None,
        SpareFireBoss,
        KillFireBoss
    }
    public enum FinalEndings {
        None,
        TrueBadEnding,
        TrueGoodEnding,
        WindTownSavedNeutralEnding,
        FireTownSavedNeutralEnding
    }

    public GameObject goodEnding;
    public GameObject badEnding;
    public GameObject windTownSavedNeutralEnding;
    public GameObject fireTownSavedNeutralEnding;

    public GameObject Orion;
    public GameObject Lumi;

    public List<Enemy> finalBosses = new List<Enemy>();

    public BattleManager bm;
    public UIManager uiManager;
    public MenuController menuController;
    public TabController tabController;
    public CharacterTabController characterTabController;

    public KnightBattle knightBattle;
    public KnightMovement knightMovement;

    public List<Character> allPlayers;

    public Animator partyAnimator;

    public CurrentStage currentStage;
    public FirstDecision firstDecision;
    public SecondDecision secondDecision;
    public FinalEndings finalEndings;

    void Awake() {
        knightBattle = FindFirstObjectByType<KnightBattle>();
        knightMovement = FindFirstObjectByType<KnightMovement>();

        currentStage = CurrentStage.firstChoice;
        firstDecision = FirstDecision.None;
        secondDecision = SecondDecision.None;
        finalEndings = FinalEndings.None;

        goodEnding.SetActive(false);
        badEnding.SetActive(false);
        windTownSavedNeutralEnding.SetActive(false);
        fireTownSavedNeutralEnding.SetActive(false);
    }
    public void SetUpGM() {
        allPlayers = new List<Character>();
        UpdatePlayers();

        characterTabController.ActivateTab(0);
    }

    public void AddPlayer(GameObject character) {
        partyAnimator.SetTrigger("NewParty");

        foreach (var player in bm.playerBattle.currentParty) {
            if (player.name == character.name) {
                return;
            }
        }

        bm.playerBattle.currentParty.Add(character);
    }

    public void RemovePlayer(GameObject character) {
        for (int i = 0; i < bm.playerBattle.currentParty.Count; i++) {
            if (bm.playerBattle.currentParty[i].name == character.name) {
                bm.playerBattle.currentParty.RemoveAt(i);
                return;
            }
        }
    }

    public void UpdatePlayers() {
        allPlayers.Clear();
        for (int i = 0; i < bm.playerBattle.currentParty.Count; i++) {
            if (bm.playerBattle.currentParty[i] != null) {
                allPlayers.Add(bm.playerBattle.currentParty[i].GetComponent<Character>());
            }
        }
    }

    public void Test() {
        Debug.Log("Test");
    }

    void Update() {
        UpdatePlayers();
    }


    public void SpareDecision() {
        if (currentStage == CurrentStage.firstChoice) {
            firstDecision = FirstDecision.SpareWindBoss;
            currentStage = CurrentStage.secondChoice;
        } else if (currentStage == CurrentStage.secondChoice) {
            secondDecision = SecondDecision.SpareFireBoss;
            currentStage = CurrentStage.finalChoice;
        } else if (currentStage == CurrentStage.finalChoice) {
            finalEndings = FinalEndings.TrueBadEnding;
        }
    }

    public void KillDecision() {
        if (currentStage == CurrentStage.firstChoice) {
            firstDecision = FirstDecision.KillWindBoss;
            currentStage = CurrentStage.secondChoice;
        } else if (currentStage == CurrentStage.secondChoice) {
            secondDecision = SecondDecision.KillFireBoss;
            currentStage = CurrentStage.finalChoice;
        }
    }

    public void addEXP(int exp) {
        int levelsIncreased = allPlayers[0].characterData.UpdateExp(exp);
        UpdateEXP(levelsIncreased);
    }

    private void UpdateEXP(int levels) {
        for (int j = 0; j < levels; j++) {
            for (int i = 0; i < allPlayers.Count; i++) {
                allPlayers[i].characterData.baseAttack = (int) (allPlayers[i].characterData.baseAttack * 1.3);
                allPlayers[i].characterData.maxHealth = (int) (allPlayers[i].characterData.maxHealth * 1.1);
            }
        }

        Debug.Log($"GameManager: Player EXP increased by {levels} levels");
    }

    public void setFinalParty() {
        if (firstDecision == FirstDecision.SpareWindBoss && secondDecision == SecondDecision.SpareFireBoss) { // Spared all bosses
            
        } else if (firstDecision == FirstDecision.SpareWindBoss && secondDecision == SecondDecision.KillFireBoss) { // Spare only Wind Boss
            AddPlayer(Lumi);
        } else if (firstDecision == FirstDecision.KillWindBoss && secondDecision == SecondDecision.SpareFireBoss) { // Spare only Fire Boss
            AddPlayer(Orion);
        } else if (firstDecision == FirstDecision.KillWindBoss && secondDecision == SecondDecision.KillFireBoss) { // Killed all bosses
            AddPlayer(Orion);
            AddPlayer(Lumi);
        }
    }

    public void setFinalBoss() { // 1. Boss++, 2. Wind Spared Boss+, 3. Fire Spared Boss+, 4. Boss
        if (firstDecision == FirstDecision.SpareWindBoss && secondDecision == SecondDecision.SpareFireBoss) { // Spared all bosses
            finalBosses[0].TriggerBattle();
        } else if (firstDecision == FirstDecision.SpareWindBoss && secondDecision == SecondDecision.KillFireBoss) { // Spare only Wind Boss
            finalBosses[1].TriggerBattle();
        } else if (firstDecision == FirstDecision.KillWindBoss && secondDecision == SecondDecision.SpareFireBoss) { // Spare only Fire Boss
            finalBosses[2].TriggerBattle();
        } else if (firstDecision == FirstDecision.KillWindBoss && secondDecision == SecondDecision.KillFireBoss) { // Killed all bosses
            finalBosses[3].TriggerBattle();
        }
    }

    public void PlayTrueGoodEnding(KnightMovement knightMovement) {
        Debug.Log("YESS");
        goodEnding.SetActive(true);

        knightMovement.PlayDialogue(goodEnding.GetComponent<DialogueActivator>());
    }

    public void PlayTrueBadEnding(KnightMovement knightMovement) {
        Debug.Log("NOOOOOOOOOO");
        badEnding.SetActive(true);
        knightMovement.PlayDialogue(goodEnding.GetComponent<DialogueActivator>());
    }

    public void PlayWindTownSavedNeutralEnding(KnightMovement knightMovement) {
        Debug.Log("Wind Town Saved Neutral Ending");
        windTownSavedNeutralEnding.SetActive(true);
        knightMovement.PlayDialogue(goodEnding.GetComponent<DialogueActivator>());
    }

    public void PlayFireTownSavedNeutralEnding(KnightMovement knightMovement) {
        Debug.Log("Fire Town Saved Neutral Ending");
        fireTownSavedNeutralEnding.SetActive(true);
        knightMovement.PlayDialogue(goodEnding.GetComponent<DialogueActivator>());
    }
}
