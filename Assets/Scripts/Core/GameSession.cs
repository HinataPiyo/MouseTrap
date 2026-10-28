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
        InteractStateStore.Clear();
        FadeOutPending = false;
    }
}
