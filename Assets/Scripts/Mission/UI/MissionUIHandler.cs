using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MissionUIHandler : MonoBehaviour
{
    Label missionText;

    void Awake()
    {
        missionText = GetComponent<UIDocument>().rootVisualElement.Q<Label>("mission-text");
    }

    public void SetMissionText(string text)
    {
        missionText.text = text;
    }
}