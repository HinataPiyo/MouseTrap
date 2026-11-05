using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーンをまたいで保持するゲーム進行状態。
/// MonoBehaviourではなく通常のstaticデータとして持つため、シーンの読み込み・破棄の影響を受けない。
/// セーブデータを作る場合は、このクラスの内容を保存・復元する。
/// </summary>
public static class GameSession
{
    /// <summary>解放済み(操作可能)のキャラクター</summary>
    public static HashSet<Character.Name> UnlockedCharacters { get; } = new HashSet<Character.Name>();

    /// <summary>現在操作しているキャラクター</summary>
    public static Character.Name CurrentCharacter { get; set; } = Character.Name.None;

    /// <summary>移動可能な部屋</summary>
    public static HashSet<Room.Name> UnlockedRooms { get; } = new HashSet<Room.Name>();

    /// <summary>所持アイテム</summary>
    public static List<Inventory.ItemData> InventoryItems { get; } = new List<Inventory.ItemData>();

    /// <summary>登録済みミッション</summary>
    public static HashSet<string> RegisteredMissions { get; } = new HashSet<string>();

    /// <summary>完了済みミッション</summary>
    public static HashSet<string> CompletedMissions { get; } = new HashSet<string>();

    /// <summary>各ミッションの次に進むミッション</summary>
    public static Dictionary<string, string> MissionNextTitles { get; } = new Dictionary<string, string>();

    /// <summary>現在表示しているミッション</summary>
    public static string CurrentMissionTitle { get; set; }

    /// <summary>部屋移動直後の最初のシーンで、フェードアウトを再生するかどうか</summary>
    public static bool FadeOutPending { get; set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Reset()
    {
        UnlockedCharacters.Clear();
        CurrentCharacter = Character.Name.None;

        UnlockedRooms.Clear();
        UnlockedRooms.Add(Room.Name.Staffroom);
        UnlockedRooms.Add(Room.Name.Library);       //! テスト

        InventoryItems.Clear();
        RegisteredMissions.Clear();
        CompletedMissions.Clear();
        MissionNextTitles.Clear();
        CurrentMissionTitle = null;
        InteractStateStore.Clear();
        FadeOutPending = false;
    }
}
