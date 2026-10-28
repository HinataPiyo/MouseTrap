using UnityEngine;

/// <summary>
/// シーンごとに1つ配置するマネージャー／UIの基底クラス。
/// このクラスはシーンの寿命にだけ責任を持つ。シーンをまたいで残す状態は <see cref="GameSession"/> に置く。
/// 
/// 派生クラスは Awake / OnDestroy を定義せず、OnSingletonAwake / OnSingletonDestroy を使うこと。
/// (定義すると基底の登録処理が呼ばれず、Instance が null のままになる)
/// </summary>
public abstract class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
{
    public static T Instance { get; private set; }

    void Awake()
    {
        // 同じシーン内の重複のみ除外する。前シーンの残りが破棄待ちの場合は新しい方に引き継ぐ。
        if (Instance != null && Instance != this && Instance.gameObject.scene == gameObject.scene)
        {
            Debug.LogWarning($"{typeof(T).Name} is duplicated in scene '{gameObject.scene.name}'.", this);
            Destroy(this);
            return;
        }

        Instance = (T)this;
        OnSingletonAwake();
    }

    void OnDestroy()
    {
        if (Instance != this) return;

        OnSingletonDestroy();
        Instance = null;
    }

    protected virtual void OnSingletonAwake() { }
    protected virtual void OnSingletonDestroy() { }
}
