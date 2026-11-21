using System.Collections.Generic;

public static class Layer
{
    public const string Interactable = "Interactable";
    public const string Player = "Player";
}

public static class Character
{
    public enum Name { None = -1, Girl, Boy, Glasses_Boy, Delinquent_Boy, Girl_TwinTail, FireSprit }
    // 猫→犬→ウサギ→ネズミ→熊
    public enum Doll { None = -1, Cat, Dog, Rabbit, Mouse, Bear }

    public static string GetName(Name name)
    {
        switch (name)
        {
            case Name.Girl: return "少女";
            case Name.Boy: return "少年";
            case Name.Glasses_Boy: return "眼鏡少年";
            case Name.Delinquent_Boy: return "不良少年";
            case Name.Girl_TwinTail: return "ツインテールの少女";
            case Name.FireSprit: return "火のこ";
            default: return "不明";
        }
    }
}

public static class Room
{
    public static float DefaultCameraSize = 1.25f;
    public enum Name { None = -1, Staffroom, Library, Classroom, Privateroom }

    public static Dictionary<Name, string> RoomNameWithSceneName = new Dictionary<Name, string>
    {
        { Name.Staffroom, "StaffroomScene" },
        { Name.Library, "LibraryScene" },
        { Name.Classroom, "ClassroomScene" },
        { Name.Privateroom, "PrivateroomScene" }
    };

    public static Dictionary<Name, string> RoomNameWithDisplayName = new Dictionary<Name, string>
    {
        { Name.Staffroom, "しょくいんしつ" },
        { Name.Library, "としょしつ" },
        { Name.Classroom, "きょうしつ" },
        { Name.Privateroom, "こしつ" }
    };

    /// <summary>
    /// 現在のシーン名から対応する部屋の列挙型を取得する
    /// </summary>
    /// <param name="sceneName">現在のシーン名</param>
    /// <returns>対応する部屋の列挙型。該当がなければ Name.None を返す。</returns>
    public static Name GetCurrentRoom(string sceneName)
    {
        foreach (var kvp in RoomNameWithSceneName)
        {
            if (kvp.Value == sceneName)
            {
                return kvp.Key;
            }
        }
        return Name.None;
    }
}