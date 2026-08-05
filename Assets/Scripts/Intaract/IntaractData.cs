using UnityEngine;

[CreateAssetMenu(fileName = "InteractData", menuName = "Data/InteractData")]
public class InteractData : ScriptableObject
{
    [Header("UIに表示するデータ")]
    [SerializeField] InteractMarkType interactMarkType;
    [SerializeField] Sprite sprite;
    [SerializeField] string text;

    public InteractMarkType InteractMarkType => interactMarkType;
    public Sprite Sprite => sprite;
    public string Text => text;

    // ここにCommandデータを追加し、行いたい状態に応じてイベントを発火させることができるようにする。
}