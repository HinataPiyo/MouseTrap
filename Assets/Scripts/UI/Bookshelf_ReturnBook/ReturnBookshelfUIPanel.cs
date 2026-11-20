using System.Collections.Generic;
using UnityEngine;
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

    List<InteractBookData> returnedBooks = new List<InteractBookData>();

    InteractBookData selectedBook;
    Button selectButton;

    protected override void OnSingletonAwake()
    {
        uiDoc = GetComponent<UIDocument>();
        returnNumberButtons = uiDoc.rootVisualElement.Q("return-number-buttons").Query<Button>().ToList();
        selecterBookButtons = uiDoc.rootVisualElement.Q("book-selecter-buttons").Query<Button>().ToList();

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
                if(selectedBook == null || selectButton == null) return;

                selectButton.style.display = DisplayStyle.None;
                returnNumberButtons[index].style.backgroundImage = new StyleBackground(selectedBook.UI);
                returnNumberButtons[index].text = "";
                returnedBooks.Add(selectedBook);
                selectedBook = null;
                selectButton = null;
            };
        }

        Hide();
    }

    protected override void OnSingletonDestroy()
    {

    }

    public void Show()
    {
        uiDoc.rootVisualElement.style.display = DisplayStyle.Flex;

        InteractBookData[] books = Inventory.Instance.GetBook();

        // 下端の所持してる本を表示
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

        Reset();
    }

    public void Hide()
    {
        Reset();
        uiDoc.rootVisualElement.style.display = DisplayStyle.None;
    }

    void Reset()
    {
        returnedBooks.Clear();
        selectedBook = null;

        for (int i = 0; i < returnNumberButtons.Count; i++)
        {
            returnNumberButtons[i].style.backgroundImage = new StyleBackground(returnPositionUI);
            returnNumberButtons[i].text = (i + 1).ToString();
        }
    }
}