using System;
using System.Collections.Generic;

public interface IEventBus
{
    void Publish<TEvent>(TEvent message);
    void Subscribe<TEvent>(Action<TEvent> callback);
    void Unsubscribe<TEvent>(Action<TEvent> callback);
}

/// <summary>
/// EventBusクラスは、イベントの発行と購読を管理するためのシンプルなイベントバスの実装です。
/// </summary>
public class EventBus : IEventBus
{
    readonly Dictionary<Type, Delegate> _events = new();

    /// <summary>
    /// Publish<T>では、T型のイベントを発行するために、イベントの型をキーとしてデリゲートを取得し
    /// 存在する場合はそのデリゲートを呼び出す。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    public void Publish<T>(T message)
    {
        if (_events.TryGetValue(typeof(T), out var callback))
            ((Action<T>)callback)?.Invoke(message);
    }

    /// <summary>
    /// Subscribe<T>では、T型のイベントに対してコールバックを登録するために、イベントの型をキーとしてデリゲートを取得し
    /// 存在する場合は既存のデリゲートにコールバックを追加し、存在しない場合は新しいデリゲートを作成して登録する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="callback"></param>
    public void Subscribe<T>(Action<T> callback)
    {
        if (_events.TryGetValue(typeof(T), out var existing))
            _events[typeof(T)] = Delegate.Combine(existing, callback);
        else
            _events[typeof(T)] = callback;
    }

    /// <summary>
    /// Unsubscribe<T>では、T型のイベントからコールバックを解除するために、イベントの型をキーとしてデリゲートを取得し
    /// 存在する場合は既存のデリゲートからコールバックを削除する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="callback"></param>
    public void Unsubscribe<T>(Action<T> callback)
    {
        if (_events.TryGetValue(typeof(T), out var existing))
            _events[typeof(T)] = Delegate.Remove(existing, callback);
    }
}