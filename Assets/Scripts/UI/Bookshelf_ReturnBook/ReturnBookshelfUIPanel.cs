using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// 取得している本を本棚に返すためのUIパネル
/// </summary>
public class ReturnBookshelfUIPanel : PanelView<ReturnBookshelfUIPanel>
{

    [SerializeField] Sprite returnPositionUI;
    [SerializeField] LastBook lastBook;
    [SerializeField] TalkData successTalk;
    UIDocument uiDoc;
    List<Button> returnNumberButtons = new List<Button>();
    List<Button> selecterBookButtons = new List<Button>();

    static readonly LibraryBookId[] bookOrder = (LibraryBookId[])System.Enum.GetValues(typeof(LibraryBookId));
    InteractBookData[] returnedBooks;

    InteractBookData selectedBook;
    Button selectButton;
    PlayerInputController playerInput;
    public event System.Action OnBooksReturned;
    public int OpenSessionId { get; private set; }

    public bool IsSessionOpen(int sessionId) => isVisible && OpenSessionId == sessionId;

    bool isSuccess;

    protected override void OnSingletonAwake()
    {
        uiDoc = GetComponent<UIDocument>();
        returnNumberButtons = uiDoc.rootVisualElement.Q("return-number-buttons").Query<Button>().ToList();
        selecterBookButtons = uiDoc.rootVisualElement.Q("book-selecter-buttons").Query<Button>().ToList();
        returnedBooks = new InteractBookData[returnNumberButtons.Count];

        // 下端の本選択ボタンにクリックイベントを設定
        for (int i = 0; i < selecterBookButtons.Count; i++)
        {
            int index = i;

            // 該当の本を取得し保持
            selecterBookButtons[i].clicked += () =>
            {
                selectedBook = Inventory.Instance.GetBook(index);
                selectButton = selecterBookButtons[index];
                AudioManager.Instance.PlaySE("アイテム選択");
            };
        }

        // 本棚の空きボタン
        for (int i = 0; i < returnNumberButtons.Count; i++)
        {
            int index = i;      // クロージャ対策
            returnNumberButtons[i].clicked += () =>
            {
                // 既に本が置かれている場合は戻す
                if(returnedBooks[index] != null)
                {
                    InteractBookData[] books = Inventory.Instance.GetBook();
                    int returnedBookIndex = System.Array.IndexOf(books, returnedBooks[index]);
                    if(returnedBookIndex < 0 || returnedBookIndex >= selecterBookButtons.Count)
                    {
                        Debug.LogError("Could not find the placed book in the inventory.", this);
                        return;
                    }

                    selecterBookButtons[returnedBookIndex].style.display = DisplayStyle.Flex;

                    returnNumberButtons[index].style.backgroundImage = new StyleBackground(returnPositionUI);
                    returnNumberButtons[index].text = (index + 1).ToString();
                    returnedBooks[index] = null;
                    AudioManager.Instance.PlaySE("キャンセル");
                    return;
                }

                if(selectedBook == null || selectButton == null) return;

                selectButton.style.display = DisplayStyle.None;
                returnNumberButtons[index].style.backgroundImage = new StyleBackground(selectedBook.UI);
                returnNumberButtons[index].text = "";
                returnedBooks[index] = selectedBook;
                selectedBook = null;
                selectButton = null;

                AudioManager.Instance.PlaySE("置く");

                // 全て埋まったか確認
                if (!System.Array.TrueForAll(returnedBooks, book => book != null)) return;

                // 火のコと遭遇していなければ成否がわからない
                if(!lastBook.IsEventCompleted) return;

                // 火のコが成否を判定してくれる設定
                isSuccess = CheckSuccess();
                if (!isSuccess)
                {
                    Reset();
                    return;
                }

                Debug.Log("<color=green>Success</color>");

                // インベントリから削除
                foreach (InteractBookData book in returnedBooks)
                    Inventory.Instance.RemoveItem(book);

                // 成功時の処理
                OnBooksReturned?.Invoke();
                RoomChangeUI.Instance.PermitChange();
                IsInteractable(false);
                StartCoroutine(ShowSuccessTalk());
                // ミッション完了を通知(本を戻す→部屋を移動する)
                MissionManager.Instance.CompleteMissionAndSetNext(
                    MissionNames.ReturnBooks,
                    MissionNames.LeaveLibrary);
                if (Room.IsDemoBuild)
                    SceneChangeManager.Instance.CanMoveRoom(Room.Name.EndingScene, true);
            };
        }

        Hide();
    }

    void Start()
    {
        playerInput = PlayerInputController.Instance;
        playerInput.OnInputESCKeyDisabled += Hide;
    }

    IEnumerator ShowSuccessTalk()
    {
        int sessionId = OpenSessionId;
        yield return TalkPanelController.Instance.Writing(successTalk);

        if (IsSessionOpen(sessionId) && playerInput.CurrentState == StateType.None)
            playerInput.ChangeState(StateType.Interact);
    }

    protected override void OnSingletonDestroy()
    {
        if(playerInput != null)
            playerInput.OnInputESCKeyDisabled -=  Hide;
    }

    public bool Show()
    {
        OpenSessionId++;
        isVisible = true;
        uiDoc.rootVisualElement.style.display = DisplayStyle.Flex;
        GameUIHandler.Instance.SetGimmickActive(true);

        if(!isSuccess) Reset();

        // 本を4冊所持していたら(会話を進行させる)
        return CheckHasFourBooks();
    }

    // Signal用
    public void OnSignalShow()
    {
        Show();
    }

    public void Hide()
    {
        isVisible = false;
        if(!isSuccess)
        {
            Reset();
            IsInteractable(true);
        }
        
        GameUIHandler.Instance.SetGimmickActive(false);
        uiDoc.rootVisualElement.style.display = DisplayStyle.None;
    }

    void Reset()
    {
        System.Array.Clear(returnedBooks, 0, returnedBooks.Length);
        selectedBook = null;
        selectButton = null;

        for (int i = 0; i < returnNumberButtons.Count; i++)
        {
            returnNumberButtons[i].style.backgroundImage = new StyleBackground(returnPositionUI);
            returnNumberButtons[i].text = (i + 1).ToString();
        }

        InteractBookData[] books = Inventory.Instance != null ? Inventory.Instance.GetBook() : System.Array.Empty<InteractBookData>();
        for (int i = 0; i < selecterBookButtons.Count; i++)
        {
            if (i >= books.Length)
            {
                selecterBookButtons[i].style.display = DisplayStyle.None;
                continue;
            }

            selecterBookButtons[i].style.display = DisplayStyle.Flex;
            selecterBookButtons[i].style.backgroundImage = new StyleBackground(books[i].UI);
        }
    }

    /// <summary>
    /// 本が正しい順番で返却されているか確認する
    /// </summary>
    bool CheckSuccess()
    {
        if (returnedBooks.Length != bookOrder.Length)
        {
            Debug.LogError("The number of bookshelf slots must match the number of library books.", this);
            return false;
        }

        for (int i = 0; i < returnedBooks.Length; i++)
        {
            if (returnedBooks[i] == null || returnedBooks[i].BookId != bookOrder[i]) return false;
        }

        return true;
    }

    /// <summary>
    /// 4冊持ってるか確認する
    /// </summary>
    /// <returns></returns>
    bool CheckHasFourBooks()
    {
        InteractBookData[] books = Inventory.Instance.GetBook();
        return books.Length >= 4;
    }

    /// <summary>
    /// ボタンの操作可否を設定する
    /// </summary>
    /// <param name="interactable"></param>
    public void IsInteractable(bool interactable)
    {
        foreach(var btn in returnNumberButtons)
            btn.pickingMode = interactable ? PickingMode.Position : PickingMode.Ignore;

        foreach(var btn in selecterBookButtons)
            btn.pickingMode = interactable ? PickingMode.Position : PickingMode.Ignore;
    }
}