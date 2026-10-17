# 開発ガイド

## 開発環境

- Unity 6 `6000.0.51f1`
- Input System `1.14.0`
- Universal Render Pipeline `17.0.4`
- UI ToolkitおよびuGUI

Unity Hubで指定バージョンのUnityを使ってリポジトリを開きます。依存パッケージは`Packages/manifest.json`に定義されています。プロジェクトを開いた後、`File > Build Settings`で有効なシーンを確認してください。

## 主要なフォルダー

| パス | 内容 |
| --- | --- |
| `Assets/Scenes/` | 部屋・ステージのシーン |
| `Assets/Scripts/Player/` | プレイヤー移動、入力、キャラクター操作 |
| `Assets/Scripts/Interact/` | インタラクト共通処理と部屋固有イベント |
| `Assets/Scripts/Talk/` | 会話データ |
| `Assets/Scripts/Inventory/` | アイテム所持状態 |
| `Assets/Scripts/Mission/` | ミッションデータ、管理、表示 |
| `Assets/Scripts/Core/` | シーン遷移、フェード、部屋名などの共通定義 |
| `Assets/Scripts/UI/` | UI Toolkitを中心とした画面表示 |
| `Assets/Datas/` | 会話・インタラクト・キャラクターのScriptableObjectアセット |
| `Assets/Prefab/` | プレイヤー、UI、システム、ステージオブジェクトのPrefab |
| `Assets/UI Toolkit/` | UI Toolkitのレイアウトとスタイル |

## 主要システム

### 入力と移動

`PlayerInputController`がゲーム状態に応じてキー入力イベントを発行し、UIやイベント側が購読します。`Movement`はWASDを読み取り、会話・インタラクト・部屋移動・演出中は移動を止めます。キー割り当てを変える場合は、入力を直接読む箇所も含めて確認してください。

### インタラクトとイベント

`IInteract`が対象の共通インターフェースです。`Interact`を継承する対象では、`InteractData`の設定を使って取得可否、アイテム表示、取得後の無効化、再利用可否を制御します。固有イベントは`Assets/Scripts/Interact/Other/`以下の実装を参考にし、シーン上のCollider、レイヤー、参照先データも合わせて設定してください。

### 会話

`TalkData`は会話行（立ち絵、キャラクター名、本文）の配列を持つScriptableObjectです。会話データは`Assets/Datas/TalkDatas/`にあり、`TalkPanelController`が表示とSpaceキーによる進行を管理します。

### インベントリ

インタラクト対象の`InteractData`に表示用アイテム設定がある場合、`SearchInteractObjects`がインベントリに追加し、取得パネルを表示します。インベントリUIは開くと所持内容を再描画します。

### キャラクター

キャラクター名や部屋名の定義は`CoreConstants.cs`、キャラクターごとの表示・アニメーション情報は`CharacterBaseData`と`Assets/Datas/CharacterDatas/`のアセットで管理します。操作可能状態は`CharacterControlHandler`が保持します。新しいキャラクターを追加する場合は、定義、データアセット、アニメーション、プレイヤー入力、解放イベント、UI表示を一緒に確認してください。

### 部屋移動と通行止め

`RoomChangePoint`と`BlockerArea`は、関連イベントの完了状態を調べて通行可否を決めます。`SceneChangeManager`が部屋の移動可否とシーン遷移を管理し、`CoreConstants.cs`の部屋名・シーン名対応を参照します。起動時に図書室を移動可能にする設定にはテスト用の注記があるため、正式な進行条件に合わせて確認してください。

### ミッション

`MissionData`にタイトルと完了状態があり、`MissionManager`がUIへのタイトル設定と完了状態の更新を行います。ミッションを実際の進行条件として使う場合は、イベントからの設定・完了呼び出しと、完了時のプレイヤー向けフィードバックも確認してください。

## シーン・アセットの変更時

1. 既存のPrefab、ScriptableObject、イベント実装を確認し、既存の仕組みを再利用します。
2. インタラクト対象を追加するときは、データアセット、Prefabまたはシーン上のコンポーネント、Collider、Interactableレイヤー、イベント参照を確認します。
3. 部屋を追加するときは、シーン、Build Settings、`Room`の列挙値と対応表、移動可否、部屋移動UIを合わせて更新します。
4. 会話・イベント追加後は、前提条件を満たした状態と未達状態の両方で、シーンをまたぐ進行をUnity Editorで確認します。

## 検証

このドキュメントの作成時点では、Unity Editorの起動、ビルド、通しプレイを実施していません。変更後は少なくとも次をEditor上で確認してください。

- `StaffroomScene`からの起動と基本移動
- インタラクト、会話の行送り、アイテム取得
- インベントリとキャラクター情報の開閉
- イベント前後の通行止め、キャラクター・部屋の解放
- 部屋移動後のUI、プレイヤー位置、ゲーム状態
- Build Settingsに含めた全シーンの起動
