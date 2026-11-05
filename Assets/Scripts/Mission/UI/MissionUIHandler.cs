using UnityEngine;
using UnityEngine.UIElements;

public class MissionUIHandler : MonoBehaviour
{
    Label missionText;

    void Awake()
    {
        missionText = GetComponent<UIDocument>().rootVisualElement.Q<Label>("mission-text");
    }

    public void SetMissionText(string text, bool isCompleted = false)
    {
        missionText.text = isCompleted ? $"{text} ✔" : text;
    }
}