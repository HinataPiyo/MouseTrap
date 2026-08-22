using UnityEngine;

[CreateAssetMenu(fileName = "InteractData", menuName = "Data/InteractData")]
public class InteractData : ScriptableObject
{
    [Header("UIに表示するデータ")]
    [SerializeField] InteractMarkType interactMarkType;
    [SerializeField] Sprite sprite;
    [SerializeField] string text;
    [SerializeField] bool isDisabledAfterInteract;       // 取得後にオブジェクトを破棄するかどうかのフラグ
    [SerializeField] bool isShowPanel;      // 取得後にパネルを表示するかどうかのフラグ
    System.Action onInteractAction;         // Interact時に行う処理を登録するためのAction

    public InteractMarkType InteractMarkType => interactMarkType;
    public Sprite Sprite => sprite;
    public string Text => text;
    public bool IsDisabledAfterInteract => isDisabledAfterInteract;
    public bool IsShowPanel => isShowPanel;
    public System.Action OnInteractAction { get => onInteractAction; set => onInteractAction = value; }
}