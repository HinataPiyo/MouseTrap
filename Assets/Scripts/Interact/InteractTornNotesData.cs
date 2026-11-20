using UnityEngine;

[CreateAssetMenu(fileName = "InteractTornNotesData", menuName = "MouseTrap/InteractTornNotesData")]
public class InteractTornNotesData : InteractData
{
    [Header("まとめて破れたメモを取得するのでこちらで設定する")]
    [SerializeField] InteractData[] tornNotes;

    public InteractData[] TornNotes => tornNotes;
}