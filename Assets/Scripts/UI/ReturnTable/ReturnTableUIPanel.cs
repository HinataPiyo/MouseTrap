using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReturnTableUIPanel : PanelView<ReturnTableUIPanel>
{
    public enum SelectType { Shape, Color, Column }
    public enum Shape { Circle, Square, Triangle }
    public enum Color { All, LeftRight, TopBottom }
    public enum Column { One, Two, Three }
    UIDocument uiDoc;
    VisualElement shapeSelect;
    VisualElement colorSelect;
    VisualElement columnSelect;

    List<Button> shapeButtons = new List<Button>();
    List<Button> colorButtons = new List<Button>();
    List<Button> columnButtons = new List<Button>();
    
    [Header("テーブルの配置が成功した場合の次の出現位置")]
    [SerializeField] Transform nextPosition;
    [SerializeField] MemoFormClassroom memo;

    [SerializeField] GameObject tablePrefab;

    /// <summary>
    /// tableの配置を事前に把握しておく
    /// </summary>
    [SerializeField] GameObject[] setOneTable;
    [SerializeField] GameObject[] setTwoTable;
    [SerializeField] GameObject[] setThreeTable;

    [SerializeField] TableObject[] circleTable;
    [SerializeField] TableObject[] squareTable;
    [SerializeField] TableObject[] triangleTable;

    [System.Serializable]
    public class TableObject
    {
        public Sprite table;
        public Sprite obj;
        public Color color;
    }


    public class TableInfo
    {
        public Shape shape;
        public Color color;
        public Column column;

        public TableInfo()
        {
        }

        public TableInfo(Shape shape, Color color, Column column)
        {
            this.shape = shape;
            this.color = color;
            this.column = column;
        }
    }

    TableInfo currentInfo = new TableInfo();
    int[] currntColumn = new int[3];
    static readonly int[] MaxColumn = {3, 2, 3};
    TableInfo[][] currentTables = new TableInfo[3][]
    {
        new TableInfo[3],
        new TableInfo[2],
        new TableInfo[3]
    };

    List<GameObject> spawnedTables = new List<GameObject>();

    public bool IsSuccess { get; private set; } = false;

    // 一次配列は列
    // 二次配列は行
    static readonly TableInfo[][] SuccessTables = new TableInfo[3][]
    {
        new TableInfo[3]
        {
            new TableInfo(Shape.Circle, Color.All, Column.One),
            new TableInfo(Shape.Square, Color.All, Column.One),
            new TableInfo(Shape.Triangle, Color.All, Column.One),
        },
        new TableInfo[2]
        {
            new TableInfo(Shape.Circle, Color.All, Column.Two),
            new TableInfo(Shape.Square, Color.All, Column.Two),
        },
        new TableInfo[3]
        {
            new TableInfo(Shape.Circle, Color.All, Column.Three),
            new TableInfo(Shape.Square, Color.All, Column.Three),
            new TableInfo(Shape.Triangle, Color.All, Column.Three),
        }
    };

    protected override void OnSingletonAwake()
    {
        uiDoc = GetComponent<UIDocument>();
        panel = uiDoc.rootVisualElement;
        shapeSelect = panel.Q("shape-select");
        colorSelect = panel.Q("color-select");
        columnSelect = panel.Q("column-select");

        shapeButtons = shapeSelect.Query<Button>().ToList();
        colorButtons = colorSelect.Query<Button>().ToList();
        columnButtons = columnSelect.Query<Button>().ToList();
//------------------
        for (int i = 0; i < shapeButtons.Count; i++)
        {
            int index = i;
            shapeButtons[i].clicked += () => OnShapeSelected(index);
        }

        for (int i = 0; i < colorButtons.Count; i++)
        {
            int index = i;
            colorButtons[i].clicked += () => OnColorSelected(index);
        }

        for (int i = 0; i < columnButtons.Count; i++)
        {
            int index = i;
            columnButtons[i].clicked += () => OnColumnSelected(index);
        }
//------------------
    }

    void Start()
    {
        Hide();
    }

    protected override void OnSingletonDestroy()
    {
    }

    public void Show()
    {
        SetVisible(true);
        ChangeSelectType(SelectType.Shape);
    }

    public void Hide()
    {
        SetVisible(false);
        CheckPutTable();
    }

    /// <summary>
    /// SelectTypeに応じて表示切替
    /// </summary>
    void ChangeSelectType(SelectType type)
    {
        shapeSelect.style.display = type == SelectType.Shape ? DisplayStyle.Flex : DisplayStyle.None;
        colorSelect.style.display = type == SelectType.Color ? DisplayStyle.Flex : DisplayStyle.None;
        columnSelect.style.display = type == SelectType.Column ? DisplayStyle.Flex : DisplayStyle.None;
    }

//------------------
    // Shapeが選択できた時の処理
    void OnShapeSelected(int index)
    {
        currentInfo.shape = (Shape)index;
        ChangeSelectType(SelectType.Color);

        // 画像を入れ替える
        for (int i = 0; i < colorButtons.Count; i++)
        {
            switch(currentInfo.shape)
            {
                case Shape.Circle:
                    colorButtons[i].style.backgroundImage = new StyleBackground(circleTable[i].table);
                    break;
                case Shape.Square:
                    colorButtons[i].style.backgroundImage = new StyleBackground(squareTable[i].table);
                    break;
                case Shape.Triangle:
                    colorButtons[i].style.backgroundImage = new StyleBackground(triangleTable[i].table);
                    break;
            }
        }
    }

    // Colorが選択できた時の処理
    void OnColorSelected(int index)
    {
        currentInfo.color = (Color)index;
        ChangeSelectType(SelectType.Column);
    }

    // Columnが選択できた時の処理
    void OnColumnSelected(int index)
    {
        currentInfo.column = (Column)index;
        SetTable(currentInfo);
    }

//-------------------
    // テーブルをセットする
    void SetTable(TableInfo info)
    {
        GameObject table = Instantiate(tablePrefab);
        spawnedTables.Add(table);
        if (!SetPosition(table, info.column))
        {
            Destroy(table);
            return;
        }

        SetColor(table, info.shape, info.color);
        int columnIndex = (int)info.column;
        int rowIndex = currntColumn[columnIndex] - 1;
        currentTables[columnIndex][rowIndex] = new TableInfo(info.shape, info.color, info.column);

        Hide();
        CheckPutTable();
        IsSuccess = CheckSuccess();
        if(!IsSuccess)
        {
            // 全て置いていたらリセット
            bool isFullSet = currntColumn[0] >= MaxColumn[0] && currntColumn[1] >= MaxColumn[1] && currntColumn[2] >= MaxColumn[2];
            if(isFullSet)
                Reset();
            StartCoroutine(NextTableSelect());
            return;
        }

        //* 成功したときの処理
        StartCoroutine(SuccessMovie());
    }

    // 色をセットする
    void SetColor(GameObject table, Shape shape, Color col)
    {
        SpriteRenderer rend = table.GetComponent<SpriteRenderer>();
        int colIndex = (int)col;
            
        switch(shape)
        {
            case Shape.Circle:
                rend.sprite = circleTable[colIndex].obj;
                break;
            case Shape.Square:
                rend.sprite = squareTable[colIndex].obj;
                break;
            case Shape.Triangle:
                rend.sprite = triangleTable[colIndex].obj;
                break;
        }
    }

    // 位置をセットする
    bool SetPosition(GameObject table, Column col)
    {
        GameObject[] setTables = null;
        int columnIndex = (int)col;

        if (col == Column.One)
        {
            setTables = setOneTable;
        }
        else if (col == Column.Two)
        {
            setTables = setTwoTable;
        }
        else if (col == Column.Three)
        {
            setTables = setThreeTable;
        }

        if (setTables == null || currntColumn[columnIndex] >= setTables.Length)
        {
            Debug.LogWarning($"Column {col} has no available position.");
            return false;
        }

        table.transform.position = setTables[currntColumn[columnIndex]].transform.position;
        currntColumn[columnIndex]++;
        return true;
    }


//------------------

    // 次のテーブル選択へ
    IEnumerator NextTableSelect()
    {
        yield return new WaitForSeconds(1f);
        Show();
    }

    // 列のチェック
    void CheckPutTable()
    {
        // これ以上置けないようになっていたら列のボタンは無効にする
        if(currntColumn[0] >= MaxColumn[0])
        {
            columnButtons[0].style.display = DisplayStyle.None;
            Debug.Log("Column One Full");
        }
        if(currntColumn[1] >= MaxColumn[1])
        {
            columnButtons[1].style.display = DisplayStyle.None;
            Debug.Log("Column Two Full");
        }
        if(currntColumn[2] >= MaxColumn[2])
        {
            columnButtons[2].style.display = DisplayStyle.None;
            Debug.Log("Column Three Full");
        }
    }

    // 成功判定
    bool CheckSuccess()
    {
        for (int i = 0; i < SuccessTables.Length; i++)
        {
            if (currntColumn[i] < SuccessTables[i].Length)
            {
                return false;
            }
        }

        for (int i = 0; i < SuccessTables.Length; i++)
        {
            for (int j = 0; j < SuccessTables[i].Length; j++)
            {
                TableInfo expected = SuccessTables[i][j];
                TableInfo actual = currentTables[i][j];

                if (actual == null ||
                    actual.shape != expected.shape ||
                    actual.color != expected.color ||
                    actual.column != expected.column)
                {
                    Debug.Log("テーブル配置は不正解です。");
                    return false;
                }
            }
        }

        Debug.Log("テーブル配置は正解です。");
        return true;
    }

    // リセット
    void Reset()
    {
        foreach(var t in spawnedTables)
            Destroy(t);
        spawnedTables.Clear();
        currntColumn = new int[3];
    }

    IEnumerator SuccessMovie()
    {
        yield return FadeScreen.Instance.FadeInCoroutine();
        memo.ActivateMemo(nextPosition.position);
        yield return FadeScreen.Instance.FadeOutCoroutine();
    }
}