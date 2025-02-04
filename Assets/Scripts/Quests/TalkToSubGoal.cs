using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTalkToSubGoal", menuName = "Scriptable Objects/Sub Goals/Talk To Goal")]
public class TalkToSubGoal : QuestSubGoal
{
    public override void UpdateStatus(Inventory inventory) {
        IsCompleted();
    }

    public void complete() {
        currentCount = 1;
    }

    public override bool IsCompleted() {
        if (currentCount >= requiredCount) {
            completed = true;
        }
        Debug.Log("IsCompleted: " + completed);
        return currentCount > requiredCount;
    }

    private void OnEnable() {
        completed = false;
        currentCount = 0;
    }
}
