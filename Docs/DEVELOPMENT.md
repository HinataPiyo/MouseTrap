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
| `Assets/Scripts/Mission/` | ミッション名定数、登録・完了管理、表示 |
| `Assets/Scripts/Core/` | シーン遷移、フェード、部屋名などの共通定義 |
| `Assets/Scripts/UI/` | UI Toolkitを中心とした画面表示 |
| `Assets/Datas/` | 会話・インタラクト・キャラクターのScriptableObjectアセット |
| `Assets/Prefab/` | プレイヤー、UI、システム、ステージオブジェクトのPrefab |
| `Assets/UI Toolkit/` | UI Toolkitのレイアウトとスタイル |

## 主要システム

### アーキテクチャ（シングルトン・状態保持・フェード）

各シーンは、RoomControl・Player・Fade Canvasなどの管理オブジェクトを自前で持ちます。`DontDestroyOnLoad`は使いません。

- **シーン単位の管理クラス**は`SceneSingleton<T>`（[SceneSingleton.cs](../Assets/Scripts/Core/SceneSingleton.cs)）を継承します。`Instance`は`Awake`で登録され、同一シーン内の重複は破棄されます。継承先で`Awake`/`OnDestroy`を定義するとUnityの仕様で基底の処理が呼ばれなくなるため、代わりに`OnSingletonAwake()`/`OnSingletonDestroy()`をoverrideします。他クラスが`Instance`を参照するのは`Start`以降にしてください。
- **シーンをまたぐ状態**は`GameSession`（[GameSession.cs](../Assets/Scripts/Core/GameSession.cs)）に置きます。解放キャラ・現在のキャラ・解放済みの部屋・インベントリ・フェードアウト待ちフラグを保持し、起動時に`Reset()`で初期化されます。セーブ機能を作る場合の対象もここです。Interactオブジェクトごとの完了状態は`InteractStateStore`が保持します。

- **Timelineの再生履歴**は`GameSession`がシーン内のDirectorとアセットの組み合わせごとに保持します。一度再生を開始したTimelineはシーンに戻っても再生されず、一時停止からの再開は可能です。履歴は`GameSession.Reset()`で初期化されます。

- **ゲームUIの表示**は`GameUIHandler`が会話とTimelineそれぞれの実行状態から決定します。どちらかが実行中なら非表示にし、会話の終了・中断またはTimelineの終了時にそれぞれの状態を解除します。Timelineの終了Signalは、Timelineの終了時刻より前に配置してください。
- **フェード後の処理**はグローバルイベントではなく、`FadeScreen.FadeInToFadeOut(Action onBlackScreen, float duration)`の引数で渡します。完全に暗転した時点で一度だけ呼ばれるため、購読・解除は不要です。
- **シーン遷移**は`SceneChangeManager.SceneChange`が行います。多重実行は防止されており、遷移先のフェードアウトは`GameSession.FadeOutPending`をもとに次のシーンの`FadeScreen`が実行します。
- ボタンの`clicked`などのイベントは、表示のたびではなく一度だけ登録します（`RoomChangeUI`を参照）。

### 入力と移動

`PlayerInputController`がゲーム状態に応じてキー入力イベントを発行し、UIやイベント側が購読します。`Movement`はWASDを読み取り、会話・インタラクト・部屋移動・演出中は移動を止めます。キー割り当てを変える場合は、入力を直接読む箇所も含めて確認してください。

Timelineでオブジェクトを移動する場合は、対象に`DirectorWaypointMovement`を追加し、Signal TrackをそのGameObjectにバインドします。トラックに`WaypointMarker`を追加して親オブジェクト基準のローカル座標を設定します。最初のマーカーでその位置に移し、同じトラックの次のマーカーの時刻まで座標を直線補間します。最後のマーカーでは位置を保持し、マーカーが1つだけの場合もその位置を保持します。同じ時刻に複数のWaypointは置けません。親がない場合はワールド座標と一致し、Z座標は変更しません。Timelineの時刻が停止すると補間も停止し、Timelineの終了・停止時に位置保持を解除します。Playerにはこのコンポーネントを追加済みで、移動方向に応じてアニメーションを切り替えます。Transformを直接移動するため、Rigidbody2Dの衝突による移動制限は適用されません。

インベントリまたはキャラクター情報を開いているときのEキーは、画面を閉じてから対象をインタラクトします。インタラクト中のE／Spaceキーは開いている画面を閉じます。自動開始のインタラクトも、演出中を除き、進行中の操作をキャンセルしてから開始します。

### インタラクトとイベント

`IInteract`が対象の共通インターフェースです。`Interact`を継承する対象では、`InteractData`の設定を使って取得可否、アイテム表示、取得後の無効化、再利用可否を制御します。固有イベントは`Assets/Scripts/Interact/Other/`以下の実装を参考にし、シーン上のCollider、レイヤー、参照先データも合わせて設定してください。

インタラクトマークはプレイヤーの検索Colliderに重なる対象を継続的に確認し、利用可能で前提条件を満たす対象がある間に表示します。範囲内で条件を満たした場合も、入り直す必要はありません。

### 会話

`TalkData`は会話行（立ち絵、キャラクター名、本文）の配列を持つScriptableObjectです。会話データは`Assets/Datas/TalkDatas/`にあり、`TalkPanelController`が表示とE／Spaceキーによる進行を管理します。文字表示中の入力で全文を表示し、全文表示後の入力で次の行に進みます。

本棚画面から始まった会話が終了した場合、本棚画面を開いたままなら入力状態をインタラクトに戻します。会話開始前に画面を閉じた場合は、待機中の会話を開始しません。

### インベントリ

インタラクト対象の`InteractData`に表示用アイテム設定がある場合、`SearchInteractObjects`がインベントリに追加し、取得パネルを表示します。インベントリUIは開くと所持内容を再描画します。

本棚では、５冊を`LibraryBookId`の順に並べると本を消費し、本棚イベントが完了します。配置済みの枠をクリックすると本は選択欄に戻り、別の本を選択中なら入れ替わります。正解した配置は画面を閉じるまで表示され、E／Space／Escapeキーで画面を閉じられます。全枠を埋めて順番が間違っていた場合は配置をリセットします。

### キャラクター

キャラクター名や部屋名の定義は`CoreConstants.cs`、キャラクターごとの表示・アニメーション情報は`CharacterBaseData`と`Assets/Datas/CharacterDatas/`のアセットで管理します。操作可能状態と現在のキャラクターは`GameSession`が保持し、`CharacterControlHandler`が切り替えと見た目の反映を行います。新しいキャラクターを追加する場合は、定義、データアセット、アニメーション、プレイヤー入力、解放イベント、UI表示を一緒に確認してください。

### 部屋移動と通行止め

`RoomChangePoint`と`BlockerArea`は、関連イベントの完了状態を調べて通行可否を決めます。`SceneChangeManager`が部屋の移動可否（`GameSession.UnlockedRooms`）とシーン遷移を管理し、`CoreConstants.cs`の部屋名・シーン名対応を参照します。`GameSession.Reset`で図書室を移動可能にする設定にはテスト用の注記があるため、正式な進行条件に合わせて確認してください。

### ミッション

ミッション名は`MissionManager.cs`内の`MissionNames`の定数で定義し、`MissionManager.SetMission`で登録・表示、`CompleteMission`で完了状態にします。完了後に次のミッションへ進める場合は`CompleteMissionAndSetNext`を使い、完了表示後3秒で次へ進みます。先行して完了したミッションは完了状態と次ミッションの対応を記録するだけで、現在の未完了ミッションを置き換えません。現在のミッション完了後は、完了済みの後続ミッションを飛ばして最初の未完了ミッションを表示します。登録・完了状態と遷移順は`GameSession`がシーン遷移をまたいで保持し、アプリ起動時にリセットします。現在は図書室で本を散らすと「図書室に散らばった本を集める」が登録され、すべての本を回収すると完了表示になります。新しいミッションを追加する場合は、`MissionNames`の定数と定義済み一覧を更新し、開始・完了イベントから同じ定数を渡してください。

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
