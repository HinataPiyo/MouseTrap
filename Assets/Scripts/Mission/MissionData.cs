using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "MouseTrap/MissionData")]
public class MissionData : ScriptableObject
{
    [SerializeField] string missionTitle;
    [SerializeField] bool missionCompleted;

    public void SetMissionCompleted(bool completed)
    {
        missionCompleted = completed;
    }

    public string GetMissionTitle()
    {
        return missionTitle;
    }

    public bool IsMissionCompleted()
    {
        return missionCompleted;
    }
}