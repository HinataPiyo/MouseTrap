using VContainer;
using VContainer.Unity;

/// <summary>
/// GameLifetimeScopeクラスは、VContainerのLifetimeScopeを継承し、ゲーム全体で使用される依存関係を登録するためのクラスです。
/// [Inject]の条件は、GameLifetimeScopeのConfigureメソッドで登録された依存関係に基づいて解決されます。
/// 
/// Registerとは、依存関係をコンテナに登録するためのメソッドです。
/// Registerメソッドは、指定された型のインスタンスを生成し、コンテナに登録します。
/// これにより、他のクラスがその型のインスタンスを必要とする場合に、コンテナから解決されるようになります。
/// 
/// Lifetimeとは、依存関係のライフサイクルを指定するためのパラメータです。
/// Lifetimeには、Singleton、Scoped、Transientの3つのオプションがあります。
///  - Singletonは、アプリケーション全体で1つのインスタンスを共有
///  - Scopedは、特定のスコープ内で1つのインスタンスを共有
///  - Transientは、依存関係が要求されるたびに新しいインスタンスを生成
/// 
/// As<T>とは、登録された型を指定された型として解決するためのメソッドです。
/// 
/// RegisterEntryPointとは、エントリーポイントとして登録するためのメソッドです。
/// エントリーポイントとは、アプリケーションの開始時に自動的にインスタンス化されるクラスのことです。
/// 
/// RegisterComponentInHierarchyとは、指定されたコンポーネントを階層内のすべての子オブジェクトに登録するためのメソッドです。
/// 
/// </summary>

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // EventBusをシングルトンとして登録
        builder.Register<EventBus>(Lifetime.Singleton).As<IEventBus>();

        builder.RegisterEntryPoint<PlayerInputController>();

        builder.RegisterComponentInHierarchy<TakeObjectPanel>();
    }
}
