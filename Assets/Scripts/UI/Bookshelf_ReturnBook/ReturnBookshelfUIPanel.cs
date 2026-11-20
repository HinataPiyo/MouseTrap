using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


/// <summary>
/// 取得している本を本棚に返すためのUIパネル
/// </summary>
public class ReturnBookshelfUIPanel : PanelView<ReturnBookshelfUIPanel>
{

    [SerializeField] Sprite returnPositionUI;
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
            };
        }

        // 本棚の空きボタン
        for (int i = 0; i < returnNumberButtons.Count; i++)
        {
            int index = i;      // クロージャ対策
            returnNumberButtons[i].clicked += () =>
            {
                if(selectedBook == null || selectButton == null || returnedBooks[index] != null) return;

                selectButton.style.display = DisplayStyle.None;
                returnNumberButtons[index].style.backgroundImage = new StyleBackground(selectedBook.UI);
                returnNumberButtons[index].text = "";
                returnedBooks[index] = selectedBook;
                selectedBook = null;
                selectButton = null;

                // 全て埋まったか確認
                if (!System.Array.TrueForAll(returnedBooks, book => book != null)) return;

                // 成功判定
                isSuccess = CheckSuccess();
                if (!isSuccess)
                {
                    Reset();
                    return;
                }

                // インベントリから削除
                foreach (InteractBookData book in returnedBooks)
                    Inventory.Instance.RemoveItem(book);

                // 成功時の処理
                OnBooksReturned?.Invoke();
                IsInteractable(false);
            };
        }

        Hide();
    }

    void Start()
    {
        playerInput = PlayerInputController.Instance;
        playerInput.OnInputESCKeyDisabled += Hide;
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

        if(!isSuccess) Reset();

        // 本を4冊所持していたら(会話を進行させる)
        return CheckHasFourBooks();
    }

    public void Hide()
    {
        isVisible = false;
        if(!isSuccess)
        {
            Reset();
            IsInteractable(true);
        }
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