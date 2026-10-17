# MouseTrap 現状調査・完成までの課題整理

調査日: 2026-10-18

Unityバージョン: 6000.0.51f1

## タスク管理の絶対条件

ゲーム完成に関する調査・計画・実装タスクでは、以下を必ず守る。この節はタスク管理上の必須条件であり、コードを書く前の確認事項である。

1. **実装前に関連する現状を調査する。** クラス名やファイル名だけで実装済みと判定せず、処理本体、呼び出し関係、状態遷移、Scene/Prefab上のコンポーネント、ScriptableObject等のデータ参照、Build Settings、既存Docsを確認する。
2. **実際の進行経路を追う。** 入力からイベント開始、完了条件、解除・遷移、遷移先での再開までを確認する。単独の関数やアセットの存在を、ゲーム全体が完成している根拠にしない。
3. **機能を「実装済み」「一部実装済み」「未実装」「調査だけでは判断できない」に分類し、根拠を記録する。** 動作確認していない内容は動作確認済みと報告しない。静的に見つけた懸念と、実行時に再現した不具合を区別する。
4. **不明なゲーム仕様を推測で確定しない。** 最終目的、クリア条件、必要な部屋・キャラクター・ミッション、イベントの正しい順番など、実装判断を左右する未確定事項を先に確認事項として示す。
5. **調査・計画の依頼では、許可されていないコードやアセットを変更しない。** 実装依頼に移った後も、関連範囲に限定して変更し、既存の利用者変更を保持する。
6. **完成を妨げる問題を優先し、依存関係に沿ってタスクを並べる。** クリア不能・進行不能、ビルド不能、イベント条件の破綻、必須機能不足を先に扱い、リファクタリング・最適化・追加機能は分離する。
7. **完了条件と検証方法を各タスクに設定する。** 可能なテストとビルドを実施し、Unity Editorや実機での未確認事項、残課題を明記する。

## 調査範囲と制約

Scripts、Build Settings、3つのSceneファイル、関連Prefab、UI Toolkitレイアウト、キャラクター・会話・インタラクト用データ、既存Docsを静的に調査した。Unity Editorでの起動、Playerビルド、実機操作、全ルートの通しプレイは実施していない。そのため、アセット参照の実行時解決、衝突判定、画面表示、実際の遷移結果は未確認である。

この調査ではコードとゲームアセットを変更していない。READMEと本調査文書は、調査結果を記録しタスク管理ルールへ案内するためのDocsである。既存の作業ツリー変更は調査対象外として保持した。

## 1. ゲーム全体の進行構造

### 現在のコード・データから追跡できる経路

1. Build Settingsの先頭は`StaffroomScene`。`Player`を操作し、インタラクト対象の範囲に入ると`SearchInteractObjects`がマークを表示する。
2. 職員室のロッカーイベントにEキーで触れると、`LockerIntetract`が少年を表示して会話を開始する。会話後のフェード時に少年を`NextPosition`の位置へ移動する。
3. `NextPosition`を調べると、資料室についての会話、物音の会話、フェード演出を行い、少年の操作可能状態と図書室への移動許可を設定する。
4. 部屋移動画面から図書室へ遷移する。図書室には眼鏡少年の初回イベントがあり、会話とフェード後に眼鏡少年の操作を設定し、プレイヤー位置を移して対象を無効化する。
5. 図書室以降の完成ルート、クリア条件、エンディング、再スタート処理は、調査範囲のコード・データから確認できない。

主な根拠: [LockerIntetract.cs](../Assets/Scripts/Interact/Other/StaffRoom/LockerIntetract.cs)、[NextPosition.cs](../Assets/Scripts/Interact/Other/StaffRoom/NextPosition.cs)、[GlassesBoyFirstEvent.cs](../Assets/Scripts/Interact/Other/LibraryRoom/GlassesBoyFirstEvent.cs)、[TalkPanelController.cs](../Assets/Scripts/UI/TalkPanel/TalkPanelController.cs)。

### 上記の順序は現状の実際の解放条件では保証されない

- [SceneChangeManager.cs](../Assets/Scripts/Core/SceneChangeManager.cs)は起動時に職員室と図書室を両方移動可能にする。図書室の行にはテスト用の注記がある。
- [NextPosition.cs](../Assets/Scripts/Interact/Other/StaffRoom/NextPosition.cs)は`Start`時点でイベント完了フラグを`true`にしており、ここにもテスト用の注記がある。
- 職員室の`BlockerArea`はScene上で`NextPosition`をイベント条件として参照しているが、上記の初期完了状態により通行止めの条件が最初から成立する。
- 職員室の`RoomChangePoint`と`BlockerArea`は`NextPosition`を参照しているが、同コンポーネントは起動時点で完了扱いになる。図書室の`RoomChangePoint` Sceneインスタンスは`eventObjects`が0件で、別の前提条件を持たない。

従って、コードには「職員室イベントを経て図書室を解放する」意図が示されている一方、現在の設定ではその順番を強制しない。テスト注記が残っている理由と、提出時に必要な順序を確認する必要がある。

## 2. 実装状況一覧

判定はソース・設定・アセットの静的調査によるもの。Editorや実機で動作確認済みという意味ではない。

### 実装済み

| 機能 | 根拠と確認範囲 |
| --- | --- |
| WASDによる2D移動、方向アニメーション切替 | [Movement.cs](../Assets/Scripts/Player/Movement.cs)、[Player.prefab](../Assets/Prefab/Player.prefab)。ゲーム状態による停止処理も実装されている。 |
| キーボード入力のゲーム状態分岐 | [PlayerInputController.cs](../Assets/Scripts/Player/PlayerInputController.cs)。None/Talk/Interact/RoomChange/Inventory/CharacterInfoごとに入力イベントを発行する。 |
| インタラクト範囲検出とデータに基づく取得 | [SearchInteractObjects.cs](../Assets/Scripts/Player/SearchInteractObjects.cs)、[Interact.cs](../Assets/Scripts/Interact/Interact.cs)、[InteractData.cs](../Assets/Scripts/Interact/InteractData.cs)。 |
| 会話データ形式と会話表示処理 | [TalkData.cs](../Assets/Scripts/Talk/TalkData.cs)、[TalkPanelController.cs](../Assets/Scripts/UI/TalkPanel/TalkPanelController.cs)、`Assets/Datas/TalkDatas/`。文字送りとSpaceによる行送りがある。 |
| Build Settings上の2シーン登録 | [EditorBuildSettings.asset](../ProjectSettings/EditorBuildSettings.asset)に`StaffroomScene`、`LibraryScene`の順で有効登録されている。 |

### 一部実装済み

| 機能 | 根拠と不足・懸念 |
| --- | --- |
| ゲーム全体の進行 | 職員室の少年イベントと図書室の眼鏡少年イベントまではコードと会話データがある。最終目的、最終イベント、クリア・終了処理は見つからず、全体ルートは完成と確認できない。 |
| 部屋移動とイベントゲート | [CoreConstants.cs](../Assets/Scripts/Core/CoreConstants.cs)には職員室・図書室の対応があり、[SceneChangeManager.cs](../Assets/Scripts/Core/SceneChangeManager.cs)にフェード遷移がある。しかし移動許可がテスト設定で初期化され、Sceneのゲート条件も初期完了状態により迂回可能。 |
| 部屋固有イベント | StaffroomSceneにロッカー、ヒント、次の位置が配置され、LibrarySceneに眼鏡少年イベントが参照設定されている。完了条件の設定時期と実際の通行条件にテスト状態が混在する。 |
| キャラクター切替 | 入力は1～5を扱うが、[Player.prefab](../Assets/Prefab/Player.prefab)の`characterBaseData`には少女・少年・眼鏡少年の3データが設定される。4・5のデータ／解放処理は確認できない。[CharacterControlHandler.cs](../Assets/Scripts/Player/CharacterControlHandler.cs)は選択時に解放フラグを反転する一方、値ではなく辞書キーの有無で切替可否を判定しており、選択状態と解放状態の管理が一致していない。 |
| インベントリ | [Inventory.cs](../Assets/Scripts/Inventory/Inventory.cs)に追加・削除・所持確認があり、[InventoryUIPanel.cs](../Assets/Scripts/UI/PlayerInformation/Inventory/InventoryUIPanel.cs)に一覧表示がある。削除・所持確認をゲーム進行から利用する呼び出しは見つからず、アイテムを使う／謎解きに使う処理も確認できない。 |
| キャラクター情報UI | `Q`で表示・非表示にする枠組みは[CharacterInformationUIPanel.cs](../Assets/Scripts/UI/PlayerInformation/CharacterInformationUIPanel.cs)にある。キャラクターデータを取得して情報欄へ反映する処理は確認できない。 |
| ミッション | [MissionData.cs](../Assets/Scripts/Mission/MissionData.cs)、[MissionManager.cs](../Assets/Scripts/Mission/MissionManager.cs)、[MissionUIHandler.cs](../Assets/Scripts/Mission/UI/MissionUIHandler.cs)とPrefab上のManagerはある。一方、ゲームアセット内にMissionDataアセットは見つからず、`SetMission`/`CompleteMission`の呼び出しも見つからない。表示枠はプレースホルダーのまま。 |
| 部屋移動画面 | [RoomChangeUI.cs](../Assets/Scripts/UI/RoomChange/RoomChangeUI.cs)に前後の移動ボタン、使用可否表示、遷移呼び出しがある。`ShowPanel`のたびにクリックコールバックを追加し、解除しない。複数回開いた際の多重遷移リスクがある。 |
| UIボタン操作 | キーボードイベントと部屋移動・インベントリ内の一部コールバックはある。Main_Game上部のボタンやキャラクター番号ボタン、アイテム詳細の使用ボタンに対するコード上の登録は確認できない。飾りか操作対象かは仕様確認が必要。 |
| 入力中の移動停止 | [Movement.cs](../Assets/Scripts/Player/Movement.cs)は会話・インタラクト・部屋移動・演出状態では停止するが、`Inventory`と`CharacterInfo`は停止条件に含まれない。UIを開いたまま移動できる可能性がある。 |

### 未実装（調査範囲で実装を確認できないもの）

| 機能 | 根拠と注記 |
| --- | --- |
| ゲームクリア状態・エンディング・クリア後の終了／再開始 | 調査したScripts、Scene、Datasに実装を確認できない。ただし必要な最終仕様自体が未確定。 |
| 眼鏡少年イベントより後の進行ルート | SceneにはLibrarySceneのイベントがあるが、その先のゲート解除・目的達成・終了への呼び出しは確認できない。 |

上記の「未実装」は、調査した範囲で該当処理を見つけていないという意味であり、別途仕様が存在しないと断定するものではない。

### 調査だけでは判断できない

| 項目 | 確認が必要な理由 |
| --- | --- |
| 作品の正式な物語、必要なイベント順、最終目的・クリア条件 | リポジトリDocsとデータだけでは完成仕様を確定できない。 |
| `stage03`を製品ルートに含めるか | [stage03.unity](../Assets/Scenes/stage03.unity)は存在するがBuild Settings未登録で、[CoreConstants.cs](../Assets/Scripts/Core/CoreConstants.cs)にも部屋対応がない。 |
| 不良少年・ツインテール少女を実際にプレイ可能にするか | enumと入力分岐はあるが、Playerのデータ参照と解放イベントがない。意図した機能か確認が必要。 |
| 遷移後のSingleton・Inventory・現在位置の保持 | SceneChangeManagerはPrefab階層の子として配置されながら`DontDestroyOnLoad(gameObject)`を呼ぶ。Unityでは永続化対象はルートGameObjectである必要があるため、Scene遷移時の破棄・再生成・状態喪失・フェード停止をEditorで確認する必要がある。 |
| Editor以外のPlayerビルド成功 | [CharacterControlHandler.cs](../Assets/Scripts/Player/CharacterControlHandler.cs)と[CharacterBaseData.cs](../Assets/Scripts/Datas/CharacterBaseData.cs)が`UnityEditor.Animations.AnimatorController`を実行時スクリプトから参照する。PlayerビルドでEditor専用API参照が障害になる可能性が高いが、この調査ではビルド未実施。 |
| 画面レイアウト、Collider/Layer設定、会話イベントの実際の動作 | Scene/Prefabの参照は確認したが、Editor上でロード・実行していない。 |

## 3. 完成を妨げる問題

優先度は、現時点で完成可否に与える影響に基づく。未確認のゲーム仕様・実行結果は明記した。

### 最優先

1. **クリア可能な最終ルートが確認できない。** Sceneとイベントは職員室・図書室に限られ、明示的なクリア状態・エンディングが見つからない。最終仕様が不明なため、まず受け入れ条件を決める必要がある。
2. **イベントによる部屋解放が現状の設定で迂回される。** `SceneChangeManager`が図書室を起動時から許可し、`NextPosition.Start`がイベント完了を最初からtrueにする。該当コードにテスト用の注記がある。意図する進行順に戻すなら、実装タスクで解除条件をそろえる。
3. **Playerビルドが通らない可能性。** Runtime Scriptsから`UnityEditor.Animations`型を参照している。Playerビルド未実施のため、ビルドログで確認するまでは確定エラーとして扱わない。
4. **シーン遷移時のManager永続化が不適切な階層に見える。** `SceneChangeManager`はRoomControl Prefabの子に含まれるが、自身のGameObjectへ`DontDestroyOnLoad`を呼ぶ。遷移時にManagerのCoroutine、移動許可、Inventory等がどうなるかはEditor実行で確認が必要。

### 高優先

5. **イベント完了フラグの初期値・設定時期が進行を飛ばす。** `NextPosition`は起動時に完了、`LockerIntetract`はイベント開始時、`GlassesBoyFirstEvent`も会話完了前に完了を設定する。ゲート条件と合わせて、各イベントが「いつ完了したとみなされるべきか」を仕様に合わせる必要がある。
6. **キャラクター選択状態と解放状態が混在する。** `ChangeCharacterControlState`は選択のたびに辞書のboolを反転させ、そのboolを切替可否判定には使わない。キーの存在が解放扱いになっているため、選択・解放・現在キャラクターの状態が正しく表現されない。
7. **Scene遷移UIのクリック処理が積み重なる。** [RoomChangeUI.cs](../Assets/Scripts/UI/RoomChange/RoomChangeUI.cs)はパネルを表示するたびにボタンの`clicked`購読を追加する。繰り返し利用したときに同じクリックから遷移Coroutineが複数開始する可能性がある。

### 中優先／仕様確認後に着手

8. **会話中以外のUIを開いたときも移動が止まる保証がない。** Inventory/CharacterInfoは`Movement`の停止判定に含まれない。
9. **ミッション・アイテム使用・キャラクター情報のデータ連動が未確認。** それらを完成条件に含める場合、現状のUI枠とデータだけではゲーム進行を成立させる接続が不足する。
10. **同時に複数のインタラクト対象に入ったときの選択が不安定になりうる。** [SearchInteractObjects.cs](../Assets/Scripts/Player/SearchInteractObjects.cs)は対象を1つだけ保持し、距離比較で最寄りを選ぶ処理がない。実シーンで問題が出るかはEditor確認が必要。

## 4. 完成までに必要なタスク（実装順）

依存関係に沿った順序。1は仕様決定、2以降は結果に応じた作業であり、未確定仕様を前提にコードを変更しない。

| 順 | タスク | 依存関係・完了条件 |
| --- | --- | --- |
| 1 | **ゲームの完成範囲と開始からクリアまでの進行を確定する。** | 先行条件。最終目的、各イベント順、必要な部屋（`stage03`を含むか）、解放するキャラクター、ミッション／アイテム使用の必要性、クリア後状態を合意し、受け入れ可能なルートを文書化する。 |
| 2 | **Unity Editor再生とPlayerビルドでベースラインを採取する。** | 1と独立して実施可能。Consoleエラー、Player build結果、シーン起動と遷移時の状態保持を記録し、確定エラーと懸念を切り分ける。 |
| 3 | **テスト用の部屋解放・イベント完了状態を、確定した進行条件へ置き換える。** | 1に依存。初期状態では必要な部屋・ゲートが閉じ、所定イベント完了後だけ開き、遷移後も許可状態が保たれる。 |
| 4 | **イベント状態遷移と完了タイミングを確定仕様に合わせる。** | 1・3に依存。各イベントが二重起動せず、会話・演出の完了後に正しい条件が一度だけ成立し、未達時にはゲートを通過できない。 |
| 5 | **PlayerビルドのEditor専用API依存を解消する。** | 2でPlayerビルド障害を確認した場合に実施。Runtimeで使用可能な型・アセット参照へ切り替え、Editorおよび対象Playerビルドが成功する。 |
| 6 | **部屋移動とプレイ状態の引継ぎを完成させる。** | 2・3に依存。Managerの生存期間、Fade、移動先、移動許可、Inventory等について仕様上必要な状態が遷移をまたいで維持され、複数回移動しても破綻しない。 |
| 7 | **キャラクター解放・選択・表示を完成させる。** | 1に依存。必要な全キャラクターについてデータと解放条件が揃い、未解放キャラを選べず、解放済みキャラへ切替・再選択できる。情報UIも仕様に含む場合は正しいデータを表示する。 |
| 8 | **確定した最終ルート・各部屋イベント・必要なミッション／アイテム進行を実装する。** | 1・3～7に依存。開始からクリアまでの全イベントが前提条件を満たしたプレイでつながり、終了条件に到達する。ミッションとアイテム機能は要件に含まれる場合に限り接続する。 |
| 9 | **入力・UI・遷移の残る進行阻害を修正する。** | 対象機能の仕様確定後。UI状態での移動可否、重複クリック購読、対象選択など、受け入れテストで確認された問題を解消する。 |
| 10 | **全ルートをEditorで通し検証し、提出用Playerを作る。** | 3～9完了後。初期状態、イベント未達／達成、各部屋往復、クリア状態を実行し、Consoleに例外がなく、対象プラットフォームでビルド・起動できる。 |

## 5. 後回しにできる作業

ゲームの開始からクリアまでを妨げないことを確認したうえで実施する。

- `PlayerInputController`の毎フレーム`Debug.Log`、調査用ログ、未使用フィールド／usingの整理。
- `SearchInteractObjects`の対象探索アルゴリズムや、Managerクラス構成の大規模な再設計。
- 性能最適化や不要な抽象化・共通化。
- 特技の新機能、追加キャラクターや追加部屋など、確定した提出範囲に含まれない追加要素。
- 画面演出・アニメーションの追加品質改善。

ただし、Editor専用API参照のPlayerビルド影響、RoomChangeUIの多重遷移など、見た目や保守性に見える項目でも提出ビルドやゲーム進行を阻害する場合は後回しにしない。

## 6. 次に実装するべきタスク

### 第1候補: イベント進行による図書室解放のテスト設定を正式な条件に戻す

**選定理由:** `SceneChangeManager`の初期設定と`NextPosition`の初期完了フラグに、それぞれテスト用の注記がある。現状はプレイヤーがイベントを終えずに図書室へ移動でき、職員室の通行止めも起動直後から成立する。このままではイベント順を受け入れテストできず、ゲーム進行の完成を判定できない。

ただし、正しい解放タイミングは企画仕様で確定する。既存コメントを手掛かりに候補を選ぶことはできるが、仕様確認前に確定扱いしない。

**変更対象候補（実装着手時に再確認）**

- [SceneChangeManager.cs](../Assets/Scripts/Core/SceneChangeManager.cs): 初期移動許可のテスト設定。
- [NextPosition.cs](../Assets/Scripts/Interact/Other/StaffRoom/NextPosition.cs): 起動時のテスト完了フラグと、所定イベント完了時の解放処理。
- [StaffroomScene.unity](../Assets/Scenes/StaffroomScene.unity)と[BlockerArea.prefab](../Assets/Prefab/Systems/BlockerArea.prefab): ゲートが正しいイベントを参照するか、Scene設定を再確認する。
- [RoomChangePoint.prefab](../Assets/Prefab/Systems/RoomChangePoint.prefab)および各Sceneインスタンス: 職員室の参照条件と図書室の空の条件が意図どおりか確認する。

**完了条件**

- 新規開始直後に、仕様でまだ解放されていない部屋へ移動できない。
- 対象イベントを最後まで完了した後にだけ、仕様で定めた部屋・ゲートが利用可能になる。
- 会話やフェードの途中で解放されず、イベント完了後に一度だけ状態が更新される。
- Scene間を往復しても解放状態が仕様どおり維持される。
- 他の部屋・イベントに意図しない影響がない。

**検証方法**

1. Editorを新規開始状態にし、未達のまま部屋移動ポイントと該当ゲートを試す。ブロックされることを確認する。
2. 職員室の対象イベントを順に最後まで進め、会話・フェード完了前後それぞれで解放タイミングを確認する。
3. 解放後に図書室へ移動し、職員室へ戻って再移動する。解放状態、UI、プレイヤー操作が維持されることを確認する。
4. Consoleに例外・Missing Referenceがなく、Playerビルドで遷移が動作することを確認する。

**先行して必要なこと:** ゲーム完成仕様として、このイベント後に図書室を開くのが正しいか、`stage03`や別の部屋が完成ルートに必要かを確認する。仕様が未確定の間は、このタスクをコード変更へ進めない。

## 未実施の検証

- Unity EditorでのScene起動、UI操作、Collider/Layer、会話、イベントの通しプレイ。
- Unity Playerビルドとビルド後の起動。
- 複数回の部屋移動時のFade、Singleton、Inventory、移動許可状態の保持。
- 完成条件が確定した後のクリア到達テスト。

したがって、本書はソースとアセット設定に基づく静的調査結果であり、ゲームの動作確認完了報告ではない。
