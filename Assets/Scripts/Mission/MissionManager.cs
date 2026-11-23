using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MissionNames
{
    // --- 汎用 ---
    public const string FindKey = "カギを見つける";
    public const string ChangeRoom = "部屋を移動する";

    // --- 職員室 ---
    public const string CheckLocker = "ロッカーを確認する";
    public const string TalkToBoy = "少年と話す";
    public const string ExploreStaffroom = "職員室を探索する";

    // --- 図書室 ---
    public const string CollectScatteredBooks = "散らばった本を集める";
    public const string FindGlassesBoy = "眼鏡少年を探す";
    public const string CheckBookshelf = "本棚を確認する";
    public const string ReturnBooks = "本を戻す";
    public const string FindeBook = "本を見つける";
    public const string LeaveLibrary = "次の部屋へ移動する";


    static readonly HashSet<string> DefinedNames = new HashSet<string>
    {
        CollectScatteredBooks,
        CheckBookshelf,
        ReturnBooks,
        FindeBook,
        FindGlassesBoy,
        CheckLocker,
        ChangeRoom,
        TalkToBoy,
        FindKey,
        ExploreStaffroom,
        LeaveLibrary,
    };

    public static bool IsDefined(string title)
    {
        return DefinedNames.Contains(title);
    }

    public static bool IsStaffroomMission(string title)
    {
        return title == CheckLocker || title == TalkToBoy || title == FindKey
            || title == ExploreStaffroom || title == ChangeRoom;
    }
}

public class MissionManager : SceneSingleton<MissionManager>
{
    static readonly WaitForSeconds MissionTransitionDelay = new WaitForSeconds(3f);

    [SerializeField] MissionUIHandler missionUIHandler;

    void Start()
    {
        if (Room.GetCurrentRoom(gameObject.scene.name) == Room.Name.Library &&
            (string.IsNullOrEmpty(GameSession.CurrentMissionTitle) ||
             MissionNames.IsStaffroomMission(GameSession.CurrentMissionTitle)))
        {
            SetMission(MissionNames.FindGlassesBoy);
            return;
        }

        if (!string.IsNullOrEmpty(GameSession.CurrentMissionTitle))
        {
            RefreshMissionUI();
            if (IsMissionCompleted(GameSession.CurrentMissionTitle) &&
                GameSession.MissionNextTitles.TryGetValue(GameSession.CurrentMissionTitle, out string nextTitle))
            {
                StartCoroutine(AdvanceToNextIncomplete(GameSession.CurrentMissionTitle, nextTitle));
            }
        }
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

        HashSet<string> visited = new HashSet<string>();
        while (IsMissionCompleted(title) && GameSession.MissionNextTitles.TryGetValue(title, out string nextTitle))
        {
            if (!visited.Add(title))
            {
                Debug.LogWarning($"A cycle was found in the mission sequence at '{title}'.", this);
                return;
            }

            title = nextTitle;
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

        SetMission(nextTitle);
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