using UnityEngine;

[CreateAssetMenu(fileName = "InteractData", menuName = "Data/InteractData")]
public class InteractData : ScriptableObject
{
    [Header("UIに表示するデータ")]
    [SerializeField] Sprite sprite;
    [SerializeField] string text;
    [Header("取得後にオブジェクトを破棄するかどうかのフラグ")]
    [SerializeField] bool isDisabledAfterInteract;       // 取得後にオブジェクトを破棄するかどうかのフラグ
    [Header("取得後にパネルを表示するかどうかのフラグ")]
    [SerializeField] bool isShowPanel;      // 取得後にパネルを表示するかどうかのフラグ
    [Header("何度も取得可能かどうかのフラグ")]
    [SerializeField] bool isReusable;       // 何度も取得可能かどうかのフラグ

    [Header("会話データ")]
    [SerializeField] TalkData[] talkData;

    public Sprite Sprite => sprite;
    public string Text => text;
    public bool IsDisabledAfterInteract => isDisabledAfterInteract;
    public bool IsShowPanel => isShowPanel;
    public bool IsReusable => isReusable;
    public TalkData[] TalkData => talkData;
}