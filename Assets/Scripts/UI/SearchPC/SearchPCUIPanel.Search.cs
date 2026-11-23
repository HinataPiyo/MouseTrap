using System.Collections.Generic;
using UnityEngine.UIElements;

public partial class SearchPCUIPanel
{
    VisualElement searchPanel;
    VisualElement genrePanel;
    VisualElement onSearchPanel;
    Button backButton;
    Button[] searchButtons = new Button[5];

    void SearchPanelInit()
    {
        searchPanel = uiDoc.rootVisualElement.Q("search-panel");
        genrePanel = uiDoc.rootVisualElement.Q("genre-panel");
        onSearchPanel = uiDoc.rootVisualElement.Q("on-search-panels");
        backButton = onSearchPanel.Q<Button>("back");

        List<Button> buttons = genrePanel.Q<VisualElement>("search-buttons").Query<Button>().ToList();

        for (int i = 0; i < searchButtons.Length; i++)
        {
            searchButtons[i] = buttons[i];
            // 物語だけButton押下を許可
            if(i == 2)
                searchButtons[i].clicked += () =>
                {
                    AudioManager.Instance.PlaySE("普通のボタン");
                    OnSearch();
                };
        }

        backButton.clicked += BackToGenrePanel;
        backButton.clicked += () => AudioManager.Instance.PlaySE("キャンセル");
    }

    /// <summary>
    /// 指定の検索パネルを表示する
    /// </summary>
    /// <param name="index"></param>
    void OnSearch()
    {
        onSearchPanel.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// 戻る処理
    /// </summary>
    void BackToGenrePanel()
    {
        searchPanel.style.display = DisplayStyle.Flex;      // 検索画面大元
        genrePanel.style.display = DisplayStyle.Flex;       // ジャンル選択
        onSearchPanel.style.display = DisplayStyle.None;    // ジャンルを選択したのちの画面
    }
}