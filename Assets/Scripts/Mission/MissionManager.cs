using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [SerializeField] MissionUIHandler missionUIHandler;

    void Awake()
    {
        if(Instance == null) Instance = this;
    }

    /// <summary>
    /// missionのデータを設定する
    /// </summary>
    /// <param name="missionData"></param>
    public void SetMission(MissionData missionData)
    {
        if (missionData == null) return;

        missionUIHandler.SetMissionText(missionData.GetMissionTitle());
    }

    /// <summary>
    /// missionを完了状態にする
    /// </summary>
    /// <param name="missionData"></param>
    public void CompleteMission(MissionData missionData)
    {
        if (missionData == null) return;

        missionData.SetMissionCompleted(true);
    }
}