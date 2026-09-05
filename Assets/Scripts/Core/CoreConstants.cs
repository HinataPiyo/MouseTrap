public static class Layer
{
    public const string Interactable = "Interactable";
}

public static class Character
{
    public enum Name { None = -1, Girl, Boy, Glasses_Boy, Delinquent_Boy, Girl_TwinTail }

    public static string GetName(Name name)
    {
        switch (name)
        {
            case Name.Girl: return "少女";
            case Name.Boy: return "少年";
            case Name.Glasses_Boy: return "眼鏡少年";
            case Name.Delinquent_Boy: return "不良少年";
            case Name.Girl_TwinTail: return "ツインテールの少女";
            default: return "不明";
        }
    }
}