using UnityEngine;
public class CharacterBaseData : ScriptableObject
{
    [SerializeField] Character.Name characterNameEnum;
    [SerializeField] string characterName;
    [Header("キャラアイコン(0 = アクティブ, 1 = 非アクティブ)")]
    [SerializeField] Sprite[] characterIcon_Default;
    [SerializeField] Sprite[] characterIcon_Numbered;
    [Header("立ち絵")]
    [SerializeField] Sprite character_Standing_Sprite;
    [Header("アニメーション")]
    [SerializeField] RuntimeAnimatorController animation;
    [Header("特技")]
    [SerializeField] Sprite specialSkillIcon;
    [SerializeField] string specialSkillDescription;

    public Character.Name CharacterNameEnum => characterNameEnum;
    public string CharacterName => characterName;
    public Sprite[] CharacterIcon_Default => characterIcon_Default;
    public Sprite[] CharacterIcon_Numbered => characterIcon_Numbered;
    public Sprite CharacterStandingSprite => character_Standing_Sprite;
    public RuntimeAnimatorController Animation => animation;
    public Sprite SpecialSkillIcon => specialSkillIcon;
    public string SpecialSkillDescription => specialSkillDescription;
}