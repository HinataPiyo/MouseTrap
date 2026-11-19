using System.Collections.Generic;
using UnityEngine.UIElements;

public partial class SearchPCUIPanel
{
    VisualElement searchPanel;
    VisualElement genrePanel;
    VisualElement onSearchPanel;
    Label title;
    Button backButton;
    Button[] searchButtons = new Button[5];
    VisualElement[] onSearchPanels = new VisualElement[5];

    // 生物・植物・物語・勉強・趣味
    static readonly string[] SearchButtonGenre = { "生物", "植物", "物語", "勉強", "趣味" };

    void SearchPanelInit()
    {
        searchPanel = uiDoc.rootVisualElement.Q("search-panel");
        genrePanel = uiDoc.rootVisualElement.Q("genre-panel");
        onSearchPanel = uiDoc.rootVisualElement.Q("on-search-panels");
        backButton = onSearchPanel.Q<Button>("back");
        title = onSearchPanel.Q<Label>("Title");

        List<Button> buttons = genrePanel.Q<VisualElement>("search-buttons").Query<Button>().ToList();
        List<VisualElement> panels = onSearchPanel.Q("panel-container").Query<VisualElement>("on-search").ToList();

        for (int i = 0; i < onSearchPanels.Length; i++) onSearchPanels[i] = panels[i];

        for (int i = 0; i < searchButtons.Length; i++)
        {
            searchButtons[i] = buttons[i];
            RegisterSearchButtons(searchButtons[i], i);
        }

        backButton.clicked += BackToGenrePanel;
    }

    /// <summary>
    /// 指定の検索パネルを表示する
    /// </summary>
    /// <param name="index"></param>
    void OnSearch(int index)
    {
        onSearchPanel.style.display = DisplayStyle.Flex;
        genrePanel.style.display = DisplayStyle.None;
        // 指定された番号以外は非表示にする
        for(int i=0;i<onSearchPanels.Length;i++) onSearchPanels[i].style.display = DisplayStyle.None;
        onSearchPanels[index].style.display = DisplayStyle.Flex;

        title.text = SearchButtonGenre[index];
    }

    /// <summary>
    /// 戻る処理
    /// </summary>
    void BackToGenrePanel()
    {
        for(int i=0;i<onSearchPanels.Length;i++) onSearchPanels[i].style.display = DisplayStyle.None;
        searchPanel.style.display = DisplayStyle.Flex;      // 検索画面大元
        genrePanel.style.display = DisplayStyle.Flex;       // ジャンル選択
        onSearchPanel.style.display = DisplayStyle.None;    // ジャンルを選択したのちの画面
    }

    /// <summary>
    /// 検索用ボタン群を登録する処理
    /// </summary>
    void RegisterSearchButtons(Button btn, int index)
    {
        btn.text = SearchButtonGenre[index];
        btn.clicked += () => OnSearch(index);
    }
}