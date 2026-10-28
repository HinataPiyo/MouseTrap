using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactオブジェクトの状態をシーン遷移をまたいで保持する
/// </summary>
public static class InteractStateStore
{
    public struct State
    {
        public bool HasAcquiredItem;
        public bool IsEventCompleted;
        public bool IsDeactivated;
    }

    static readonly Dictionary<string, State> states = new Dictionary<string, State>();

    // GameSession.Reset から呼ばれる
    public static void Clear() => states.Clear();

    public static void Save(string key, State state) => states[key] = state;

    public static bool TryGet(string key, out State state) => states.TryGetValue(key, out state);

    /// <summary>
    /// シーン名とヒエラルキー上の位置からオブジェクトを識別するキーを作る
    /// </summary>
    public static string CreateKey(GameObject target)
    {
        var path = new System.Text.StringBuilder();
        for (Transform t = target.transform; t != null; t = t.parent)
        {
            path.Insert(0, $"/{t.name}#{t.GetSiblingIndex()}");
        }
        return $"{target.scene.name}{path}";
    }
}
