using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MissionNames
{
    public const string FirstGetMemo = "最初のメモを取る";
    public const string CheckLocker = "ロッカーを確認する";
    public const string CollectScatteredBooks = "図書室に散らばった本を集める";
    public const string FindKey = "カギを見つける";
    public const string TalkToBoy = "少年に話す";
    public const string ChangeRoom = "部屋を移動する";

    static readonly HashSet<string> DefinedNames = new HashSet<string>
    {
        CollectScatteredBooks,
        FirstGetMemo,
        CheckLocker,
        ChangeRoom,
        TalkToBoy,
        FindKey,
    };

    public static bool IsDefined(string title)
    {
        return DefinedNames.Contains(title);
    }
}

public class MissionManager : SceneSingleton<MissionManager>
{
    static readonly WaitForSeconds MissionTransitionDelay = new WaitForSeconds(3f);

    [SerializeField] MissionUIHandler missionUIHandler;

    void Start()
    {
        if (!string.IsNullOrEmpty(GameSession.CurrentMissionTitle))
            RefreshMissionUI();
    }

    /// <summary>
    /// ミッションを登録して表示する
    /// </summary>
    public void SetMission(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Debug.LogWarning("Cannot register a mission with an empty title.", this);
            return;
        }

        if (!MissionNames.IsDefined(title))
        {
            Debug.LogWarning($"Mission title '{title}' is not defined in MissionNames.", this);
            return;
        }

        GameSession.RegisteredMissions.Add(title);
        GameSession.CurrentMissionTitle = title;
        RefreshMissionUI();
    }

    /// <summary>
    /// 定義済みミッションをタイトルで検索し、完了状態にする
    /// </summary>
    public void CompleteMission(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Debug.LogWarning("Cannot complete a mission with an empty title.", this);
            return;
        }

        if (!MissionNames.IsDefined(title))
        {
            Debug.LogWarning($"Mission title '{title}' is not defined in MissionNames.", this);
            return;
        }

        GameSession.RegisteredMissions.Add(title);
        GameSession.CompletedMissions.Add(title);
        if (GameSession.CurrentMissionTitle == title)
            RefreshMissionUI();
    }

    /// <summary>
    /// ミッションを完了して次を設定する
    /// </summary>
    /// <param name="completedTitle">完了するミッション</param>
    /// <param name="nextTitle">次のミッション</param>
    public void CompleteMissionAndSetNext(string completedTitle, string nextTitle)
    {
        if (string.IsNullOrWhiteSpace(nextTitle) || !MissionNames.IsDefined(nextTitle))
        {
            Debug.LogWarning($"Mission title '{nextTitle}' is not defined in MissionNames.", this);
            return;
        }

        CompleteMission(completedTitle);
        if (!GameSession.CompletedMissions.Contains(completedTitle))
            return;

        GameSession.MissionNextTitles[completedTitle] = nextTitle;

        // Completing a later mission records its result without replacing the active mission.
        if (GameSession.CurrentMissionTitle == completedTitle)
            StartCoroutine(AdvanceToNextIncomplete(completedTitle, nextTitle));
    }

    /// <summary>
    /// 完了済みの後続ミッションを飛ばし、最初の未完了ミッションを設定する
    /// </summary>
    IEnumerator AdvanceToNextIncomplete(string completedTitle, string nextTitle)
    {
        yield return MissionTransitionDelay;

        if (GameSession.CurrentMissionTitle != completedTitle)
            yield break;

        string nextTitleToSet = nextTitle;
        HashSet<string> visited = new HashSet<string> { completedTitle };

        while (GameSession.CompletedMissions.Contains(nextTitleToSet))
        {
            if (!visited.Add(nextTitleToSet))
            {
                Debug.LogWarning($"A cycle was found in the mission sequence at '{nextTitleToSet}'.", this);
                yield break;
            }

            if (!GameSession.MissionNextTitles.TryGetValue(nextTitleToSet, out string followingTitle))
            {
                Debug.LogWarning($"No next mission is defined after completed mission '{nextTitleToSet}'.", this);
                yield break;
            }

            nextTitleToSet = followingTitle;
        }

        SetMission(nextTitleToSet);
    }

    public bool IsMissionCompleted(string title)
    {
        return GameSession.CompletedMissions.Contains(title);
    }

    void RefreshMissionUI()
    {
        string title = GameSession.CurrentMissionTitle;
        bool isCompleted = !string.IsNullOrEmpty(title) && IsMissionCompleted(title);
        missionUIHandler.SetMissionText(title ?? string.Empty, isCompleted);
    }
}