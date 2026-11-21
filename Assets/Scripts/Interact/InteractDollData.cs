using UnityEngine;

[CreateAssetMenu(fileName = "InteractDollData", menuName = "MouseTrap/InteractDollData")]
public class InteractDollData : InteractData
{
    [Header("人形特有の情報")]
    [SerializeField] Character.Doll dollType;
    [SerializeField] Sprite dollObject;
    [SerializeField] Sprite dollUI;

    public Character.Doll DollType => dollType;
    public Sprite ObjectSprite => dollObject;
    public Sprite UISprite => dollUI;
}