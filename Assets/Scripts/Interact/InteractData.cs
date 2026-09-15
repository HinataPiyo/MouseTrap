using UnityEngine;

[CreateAssetMenu(fileName = "InteractData", menuName = "Data/InteractData")]
public class InteractData : ScriptableObject
{
    [Header("UIに表示するデータ")]
    [SerializeField] InteractMarkType interactMarkType;
    [SerializeField] Sprite sprite;
    [SerializeField] string text;
    [Tooltip("取得後にオブジェクトを破棄するかどうかのフラグ")]
    [SerializeField] bool isDisabledAfterInteract;       // 取得後にオブジェクトを破棄するかどうかのフラグ
    [Tooltip("取得後にパネルを表示するかどうかのフラグ")]
    [SerializeField] bool isShowPanel;      // 取得後にパネルを表示するかどうかのフラグ
    [Tooltip("何度も取得可能かどうかのフラグ")]
    [SerializeField] bool isReusable;       // 何度も取得可能かどうかのフラグ

    System.Action onInteractAction;         // Interact時に行う処理を登録するためのAction

    public InteractMarkType InteractMarkType => interactMarkType;
    public Sprite Sprite => sprite;
    public string Text => text;
    public bool IsDisabledAfterInteract => isDisabledAfterInteract;
    public bool IsShowPanel => isShowPanel;
    public bool IsReusable => isReusable;
    public System.Action OnInteractAction { get => onInteractAction; set => onInteractAction = value; }
}