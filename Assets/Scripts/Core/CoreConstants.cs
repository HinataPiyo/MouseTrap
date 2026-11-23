using System.Collections.Generic;

public static class Layer
{
    public const string Interactable = "Interactable";
    public const string Player = "Player";
}

public static class Character
{
    public enum Name { None = -1, Girl, Boy, Glasses_Boy, Delinquent_Boy, Girl_TwinTail, FireSprit, Unknown }
    // 猫→犬→ウサギ→ネズミ→熊
    public enum Doll { None = -1, Cat, Dog, Rabbit, Mouse, Bear }
    public enum PortraitOrder
    {
        Teacher,          // 先生
        GlassesBoy,       // 眼鏡の少年
        BandageBoy,       // 絆創膏の子
        DollGirl,         // 人形好きの女の子
        ReturnedBoy       // 卒業後に戻ってきた子
}

    public static string GetName(Name name)
    {
        switch (name)
        {
            case Name.Girl: return "少女";
            case Name.Boy: return "少年";
            case Name.Glasses_Boy: return "眼鏡少年";
            case Name.Delinquent_Boy: return "不良少年";
            case Name.Girl_TwinTail: return "ツインテールの少女";
            case Name.FireSprit: return "火のコ";
            case Name.Unknown: return "？？？";
            default: return "";
        }
    }
}

public static class Room
{
    public static float DefaultCameraSize = 1.25f;
    public static readonly bool IsDemoBuild = true;
    public enum Name { None = -1,Title, Staffroom, Library, Classroom, Privateroom, DeansOfficeScene, EndingScene }

    public static Dictionary<Name, string> RoomNameWithSceneName = new Dictionary<Name, string>
    {
        { Name.Title, "TitleScene" },
        { Name.Staffroom, "StaffroomScene" },
        { Name.Library, "LibraryScene" },
        { Name.Classroom, "ClassroomScene" },
        { Name.Privateroom, "PrivateroomScene"},
        { Name.DeansOfficeScene, "Dean'sOfficeScene"},
        { Name.EndingScene, "EndingScene"}
    };

    public static Dictionary<Name, string> RoomNameWithDisplayName = new Dictionary<Name, string>
    {
        { Name.Title, "たいとる" },
        { Name.Staffroom, "しょくいんしつ" },
        { Name.Library, "としょしつ" },
        { Name.Classroom, "きょうしつ" },
        { Name.Privateroom, "こしつ" },
        { Name.DeansOfficeScene, "いんちょうしつ" },
        { Name.EndingScene, "おわり" }
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