# 机復元ギミック（ReturnTable）解説

**最終更新**: 2026-11-21  
**実装ファイル**: `Assets/Scripts/UI/ReturnTable/ReturnTableUIPanel.cs`

---

## 📖 目次

1. [ギミック概要](#ギミック概要)
2. [処理フロー](#処理フロー)
3. [主要処理の詳細](#主要処理の詳細)
4. [配置データ構造](#配置データ構造)
5. [成功時・失敗時の流れ](#成功時失敗時の流れ)
6. [ボタン操作の流れ](#ボタン操作の流れ)
7. [重要な数値と変数](#重要な数値と変数)
8. [キーポイント](#キーポイント)

---

## 🎯 ギミック概要

### 目的
壊れた机の形状・色・配置を正しい順序で復元するパズル型イベント。プレイヤーが段階的に形状・色・配置位置を選択し、すべてが正しい配置で埋まるとイベント成功。

### 構成
- **配置列**: 3列（One・Two・Three）
- **配置行**: 列によって異なる
  - One列: 3個
  - Two列: 2個
  - Three列: 3個
  - **合計**: 8個の机

### 正解パターン
各列にはCircle→Square→Triangle の順に、色はAllで統一

| 列 | 1段目 | 2段目 | 3段目 |
|----|-------|-------|-------|
| One | Circle/All | Square/All | Triangle/All |
| Two | Circle/All | Square/All | — |
| Three | Circle/All | Square/All | Triangle/All |

---

## 📊 処理フロー

```
【UI表示】
  ↓
1️⃣ Shape選択（Circle/Square/Triangle）
  ├─ 色ボタン画像を選択形状に合わせて更新
  ↓
2️⃣ Color選択（All/LeftRight/TopBottom）
  ├─ 列選択UIに遷移
  ↓
3️⃣ Column選択（One/Two/Three）
  ├─ SetTable() 実行
  │
  └─→ SetPosition()：配置先の位置を決定
      ├─ currntColumn[i]++ で次の行に進む
      ├─ 枠が満杯 → 配置失敗・戻る
      └─ 配置成功
  
  ├─→ SetColor()：スプライト（見た目）を設定
  
  ├─→ currentTables[列][行]に記録（比較用）
  
  ├─→ CheckPutTable()：満杯の列をボタン無効化
  
  ├─→ CheckSuccess()：正解判定
      │
      ├─ 【全枠未充満】→ 不正解
      │
      ├─ 【全枠充満 + 並び間違い】
      │  → Reset()：全机を破壊・状態初期化
      │  → 1秒後 UI表示（リトライ）
      │
      └─ 【全枠充満 + 正しい並び】
         → SuccessMovie()
         → 机の出現地点に移動
         → メモUI激活
         → フェード演出
```

---

## 🔧 主要処理の詳細

### **1. 配置フェーズ（SetPosition）**

```csharp
bool SetPosition(GameObject table, Column col)
{
    GameObject[] setTables = null;
    int columnIndex = (int)col;

    // 列に応じた配置目標物配列を選択
    if (col == Column.One)
        setTables = setOneTable;
    else if (col == Column.Two)
        setTables = setTwoTable;
    else if (col == Column.Three)
        setTables = setThreeTable;

    // 範囲チェック
    if (setTables == null || currntColumn[columnIndex] >= setTables.Length)
    {
        Debug.LogWarning($"Column {col} has no available position.");
        return false;
    }

    // 位置設定とカウント進行
    table.transform.position = setTables[currntColumn[columnIndex]].transform.position;
    currntColumn[columnIndex]++;
    return true;
}
```

**処理内容**:
- 列ごとに専用の「配置目標物」配列を参照
- **One列**: 3個の配置目標 → `currntColumn[0]` 管理
- **Two列**: 2個の配置目標 → `currntColumn[1]` 管理  
- **Three列**: 3個の配置目標 → `currntColumn[2]` 管理
- 机のワールド座標を配置目標物の位置に設定
- カウンター増加で、次回は次の行に配置される

**失敗条件**:
- 列の最大数に達している
- 配置目標物配列がnull

---

### **2. 記録フェーズ（SetTable内）**

```csharp
void SetTable(TableInfo info)
{
    GameObject table = Instantiate(tablePrefab);
    spawnedTables.Add(table);  // 破壊対象に記録
    
    if (!SetPosition(table, info.column))
    {
        Destroy(table);
        return;
    }

    SetColor(table, info.shape, info.color);
    
    // 配置内容を記録
    int columnIndex = (int)info.column;
    int rowIndex = currntColumn[columnIndex] - 1;  // ← 配置直後の位置
    currentTables[columnIndex][rowIndex] = new TableInfo(info.shape, info.color, info.column);

    Hide();
    CheckPutTable();
    IsSuccess = CheckSuccess();
    // ... 判定処理へ
}
```

**重要ポイント**:
- 配置完了後、**実際に置いた内容**をcurrentTablesに記録
- `currntColumn[columnIndex] - 1` で「今置いた」行のインデックスを取得
- 後で正解配列と比較するため、必ず値をコピーして保存

---

### **3. 正解判定フェーズ（CheckSuccess）**

```csharp
bool CheckSuccess()
{
    // Step 1: 全枠が埋まったか確認
    for (int i = 0; i < SuccessTables.Length; i++)
    {
        if (currntColumn[i] < SuccessTables[i].Length)
        {
            return false;  // まだ空きがある
        }
    }

    // Step 2: 正解配列と比較
    for (int i = 0; i < SuccessTables.Length; i++)
    {
        for (int j = 0; j < SuccessTables[i].Length; j++)
        {
            TableInfo expected = SuccessTables[i][j];
            TableInfo actual = currentTables[i][j];

            // Shape / Color / Column をすべて検査
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
```

**判定ロジック**:

| 条件 | 結果 |
|-----|-----|
| 全枠未充満 | 不正解（判定スキップ） |
| 全枠充満 + 並び正解 | **正解** → IsSuccess = true |
| 全枠充満 + 並び違い | **不正解** → Reset() 実行 |

---

### **4. リセット処理（Reset）**

```csharp
void Reset()
{
    // 生成した全ての机を破壊
    foreach(var t in spawnedTables)
        Destroy(t);
    
    spawnedTables.Clear();
    currntColumn = new int[3];  // {0, 0, 0}に初期化
}
```

**処理内容**:
1. `spawnedTables` リスト内のすべてのGameObjectを削除
2. リスト自体をクリア
3. 配置カウンターをリセット
4. `currentTables` は自動的にnull参照になる（次の配置で上書き）

**実行タイミング**:
- 全8枠が埋まったにもかかわらず、並びが正解ではない場合のみ

---

### **5. 成功演出（SuccessMovie）**

```csharp
IEnumerator SuccessMovie()
{
    yield return FadeScreen.Instance.FadeInCoroutine();
    memo.ActivateMemo(nextPosition.position);
    yield return FadeScreen.Instance.FadeOutCoroutine();
}
```

**演出順序**:
1. 画面フェードイン（暗転）
2. メモUI表示（nextPositionに移動）
3. 画面フェードアウト（復帰）

---

## 📋 配置データ構造

### **正解配置（SuccessTables - static）**

```csharp
static readonly TableInfo[][] SuccessTables = new TableInfo[3][]
{
    new TableInfo[3]  // Column.One
    {
        new TableInfo(Shape.Circle, Color.All, Column.One),
        new TableInfo(Shape.Square, Color.All, Column.One),
        new TableInfo(Shape.Triangle, Color.All, Column.One),
    },
    new TableInfo[2]  // Column.Two
    {
        new TableInfo(Shape.Circle, Color.All, Column.Two),
        new TableInfo(Shape.Square, Color.All, Column.Two),
    },
    new TableInfo[3]  // Column.Three
    {
        new TableInfo(Shape.Circle, Color.All, Column.Three),
        new TableInfo(Shape.Square, Color.All, Column.Three),
        new TableInfo(Shape.Triangle, Color.All, Column.Three),
    }
};
```

**データ型**: ジャグ配列 `TableInfo[][]`
- 一次配列：列（3要素）
- 二次配列：行（列によって2〜3要素）
- 要素：`TableInfo(Shape, Color, Column)`

### **実配置状態（currentTables）**

```csharp
TableInfo[][] currentTables = new TableInfo[3][]
{
    new TableInfo[3],
    new TableInfo[2],
    new TableInfo[3]
};
```

**用途**: ユーザーが実際に置いた内容を記録  
**構造**: `SuccessTables` と完全に同じ形

### **配置カウンター（currntColumn）**

```csharp
int[] currntColumn = new int[3];  // {0, 0, 0}で初期化
```

| インデックス | 対象列 | 最大値 |
|------------|-------|-------|
| 0 | Column.One | 3 |
| 1 | Column.Two | 2 |
| 2 | Column.Three | 3 |

---

## ✅ 成功時・失敗時の流れ

### **成功パターン**

```
SetTable() 実行
  ↓
CheckSuccess() = true
  ↓
SuccessMovie() コルーチン開始
  ↓
1️⃣ FadeInCoroutine（画面暗転）
  ↓
2️⃣ memo.ActivateMemo(nextPosition.position)
  ├─ メモUI表示
  └─ プレイヤー位置を更新
  ↓
3️⃣ FadeOutCoroutine（画面復帰）
  ↓
イベント完了 / IsSuccess = true
```

### **失敗パターン**

```
CheckSuccess() 実行
  ↓
【判定分岐】
  ├─ 全枠未充満 → 不正解（次の配置待機）
  │
  └─ 全枠充満 + 並び違い → 不正解
     ↓
     Reset()
     ├─ 生成物破壊
     ├─ currntColumn初期化
     └─ currentTables自動クリア
     ↓
     NextTableSelect() コルーチン開始
     ├─ 1秒待機
     └─ Show()で UI再表示
     ↓
     Shape選択から再開
```

---

## 🎮 ボタン操作の流れ

### **全体的な操作フロー**

```
UI初期化（Show）
  ↓
┌─────────────────────────────────────────┐
│ 1️⃣ Shape選択ボタンクリック           │
│    ↓                                   │
│    OnShapeSelected(index)              │
│    ├─ currentInfo.shape = (Shape)index │
│    ├─ ChangeSelectType(Color)          │
│    └─ 色ボタン画像を更新              │
│    ↓                                   │
└─────────────────────────────────────────┘
      ↓
┌─────────────────────────────────────────┐
│ 2️⃣ Color選択ボタンクリック           │
│    ↓                                   │
│    OnColorSelected(index)              │
│    ├─ currentInfo.color = (Color)index │
│    └─ ChangeSelectType(Column)         │
│    ↓                                   │
└─────────────────────────────────────────┘
      ↓
┌─────────────────────────────────────────┐
│ 3️⃣ Column選択ボタンクリック          │
│    ↓                                   │
│    OnColumnSelected(index)             │
│    ├─ currentInfo.column = (Column)ind │
│    └─ SetTable(currentInfo) 実行       │
│    ↓                                   │
└─────────────────────────────────────────┘
      ↓
     正解判定 → 成功 or 失敗 or 継続
```

### **ボタンの有効/無効制御**

```csharp
void CheckPutTable()
{
    // 満杯になった列のボタンを無効化
    if(currntColumn[0] >= MaxColumn[0])
        columnButtons[0].style.display = DisplayStyle.None;
    if(currntColumn[1] >= MaxColumn[1])
        columnButtons[1].style.display = DisplayStyle.None;
    if(currntColumn[2] >= MaxColumn[2])
        columnButtons[2].style.display = DisplayStyle.None;
}
```

| 操作 | イベント | 次の表示 | 条件 |
|-----|---------|--------|-----|
| Shape選択 | OnShapeSelected() | Color選択UI | 形状を記録 |
| Color選択 | OnColorSelected() | Column選択UI | 色を記録 |
| Column選択 | OnColumnSelected() | SetTable()実行 | 枠に空きがあれば配置 |

---

## 🔑 重要な数値と変数

### **定数**

```csharp
static readonly int[] MaxColumn = {3, 2, 3};
```

| インデックス | 意味 | 値 |
|------------|------|-----|
| 0 | Column.One の最大配置数 | 3 |
| 1 | Column.Two の最大配置数 | 2 |
| 2 | Column.Three の最大配置数 | 3 |

### **状態変数**

| 変数 | 型 | 説明 |
|-----|-----|------|
| `currntColumn[3]` | int[] | 各列の現在の配置数（0から開始） |
| `currentTables[3][]` | TableInfo[][] | ユーザーが置いた机の内容 |
| `currentInfo` | TableInfo | 現在選択中の形状・色・位置 |
| `spawnedTables` | List\<GameObject\> | 生成された全ての机 |
| `IsSuccess` | bool | 最後の判定結果 |

### **配列と対応**

```
SetOneTable[]    ← Column.One の配置目標物（3個）
SetTwoTable[]    ← Column.Two の配置目標物（2個）
SetThreeTable[]  ← Column.Three の配置目標物（3個）

currntColumn[0]  ← SetOneTable 用のカウンター
currntColumn[1]  ← SetTwoTable 用のカウンター
currntColumn[2]  ← SetThreeTable 用のカウンター

currentTables[0][]  ← Column.One の配置内容
currentTables[1][]  ← Column.Two の配置内容
currentTables[2][]  ← Column.Three の配置内容
```

---

## 💡 キーポイント

### **1. 配置とカウントの同期**

```csharp
// SetPosition() 内
table.transform.position = setTables[currntColumn[columnIndex]].transform.position;
currntColumn[columnIndex]++;  // ← 次回は次の行に配置される
```

位置設定 → カウント進行 → 記録 の3段階で状態を管理。

### **2. 配列添字管理（重要）**

```csharp
// SetTable() 内
int columnIndex = (int)info.column;      // Column.One = 0, Two = 1, Three = 2
int rowIndex = currntColumn[columnIndex] - 1;  // 「今置いた」行のインデックス
currentTables[columnIndex][rowIndex] = new TableInfo(...);
```

配置直後の `currntColumn[i] - 1` で、今置いたばかりの行を特定する。

### **3. 全体満杯判定（AND条件）**

```csharp
// CheckSuccess() 内
bool isFullSet = currntColumn[0] >= MaxColumn[0] 
              && currntColumn[1] >= MaxColumn[1] 
              && currntColumn[2] >= MaxColumn[2];
```

**全3列がすべて満杯**になった場合のみ判定。OR条件ではないので注意。

### **4. リセットのみ（進行保存なし）**

間違いに気づいた場合、ゲームは「全選択やり直し」をユーザーに強制。  
部分的な続行はサポートされない（UX設計としてシンプルな失敗リセット）。

### **5. ジャグ配列の活用**

列によって行数が異なる（One・Three=3行、Two=2行）ため、ジャグ配列で効率的に表現。

```csharp
TableInfo[][] currentTables = new TableInfo[3][]
{
    new TableInfo[3],  // Column.One: 3行
    new TableInfo[2],  // Column.Two: 2行
    new TableInfo[3]   // Column.Three: 3行
};
```

---

## 🎨 色選択との関連

現在実装では **Color.All** で固定されていますが、UI上には以下の選択肢があります：

```csharp
public enum Color { All, LeftRight, TopBottom }
```

将来的に色パターンを増やす場合、ここを拡張可能な設計になっています。

---

## 📝 デバッグ情報

### **ログ出力**

```
"Column [X] has no available position."  → SetPosition失敗
"Column [X] Full"                        → CheckPutTable で列満杯
"テーブル配置は不正解です。"              → CheckSuccess で不正解
"テーブル配置は正解です。"                → CheckSuccess で正解
```

### **今後の拡張ポイント**

1. **色パターンの実装**: Color 列挙型の拡張と判定追加
2. **UI改善**: ボタン無効化の視覚的フィードバック強化
3. **ヒント機能**: 次に置くべき形を提示
4. **タイムアタック**: 制限時間内での完成
5. **難易度選択**: 配置列数や行数の変更

---

**ドキュメント作成者**: Copilot  
**ゲームタイトル**: MouseTrap  
**システム**: Unity ReturnTable UI System
