using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>Timeline上の時刻と対象の親基準の位置を通知するマーカー。</summary>
[Serializable]
public class WaypointMarker : Marker, INotification, INotificationOptionProvider
{
    [Tooltip("対象の親オブジェクトを基準とするローカル座標")]
    [SerializeField] Vector2 waypoint;

    /// <summary>対象の親を基準とする移動先のローカルXY座標。</summary>
    public Vector2 Waypoint => waypoint;

    public PropertyName id => new PropertyName(nameof(WaypointMarker));

    // 再生位置がマーカーを通過したときに通知し、過去のマーカーは遡って発火しない。
    public NotificationFlags flags => default;
}
