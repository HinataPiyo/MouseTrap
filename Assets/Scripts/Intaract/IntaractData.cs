using UnityEngine;

[CreateAssetMenu(fileName = "IntaractData", menuName = "Data/IntaractData")]
public class IntaractData : ScriptableObject
{
    [Header("UIに表示するデータ")]
    [SerializeField] IntaractMarkType intaractMarkType;
    [SerializeField] Sprite sprite;
    [SerializeField] string text;

    public IntaractMarkType IntaractMarkType => intaractMarkType;
    public Sprite Sprite => sprite;
    public string Text => text;

    // ここにCommandデータを追加し、行いたい状態に応じてイベントを発火させることができるようにする。
}