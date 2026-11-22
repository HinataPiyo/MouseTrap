using System;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// バインドされたTimelineのWaypoint間を、対象のTransformのローカル座標で移動する。
/// 最後のWaypointでは位置を保持し、Timelineの終了・停止時に制御を解除する。
/// </summary>
[DisallowMultipleComponent]
public class DirectorWaypointMovement : MonoBehaviour, INotificationReceiver
{
    PlayableDirector director;
    WaypointMarker startWaypoint;
    WaypointMarker endWaypoint;

    /// <summary>Waypointによる位置制御が有効かどうか。</summary>
    public bool IsActive => director != null && startWaypoint != null;

    /// <summary>再生中のWaypoint区間のワールド方向。停止中や位置保持中はゼロ。</summary>
    public Vector2 MovementDirection
    {
        get
        {
            if (!IsActive || endWaypoint == null || director.state != PlayState.Playing
                || director.time < startWaypoint.time || director.time >= endWaypoint.time)
                return Vector2.zero;

            Vector2 localDirection = endWaypoint.Waypoint - startWaypoint.Waypoint;
            Transform parent = transform.parent;
            if (parent == null)
                return localDirection;

            // 親の回転・スケールを反映し、ワールド方向のアニメーションに合わせる。
            Vector3 worldDirection = parent.TransformVector(new Vector3(localDirection.x, localDirection.y, 0f));
            return new Vector2(worldDirection.x, worldDirection.y);
        }
    }

    /// <summary>位置更新時のワールド座標での移動量。位置保持・制御解除時はゼロ。</summary>
    public event Action<Vector2> Moved;

    /// <summary>Waypointの位置を設定し、同じトラック上の次のWaypointまでの移動を開始する。</summary>
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (!(notification is WaypointMarker marker))
            return;

        PlayableDirector source = origin.GetGraph().GetResolver() as PlayableDirector;
        if (source == null || marker.parent == null)
        {
            Debug.LogError("Waypoint requires a PlayableDirector and a parent track.", this);
            return;
        }

        // マーカーの列挙順に依存せず、現在より後で最も近い時刻を探す。
        WaypointMarker next = null;
        foreach (var trackMarker in marker.parent.GetMarkers())
        {
            if (!(trackMarker is WaypointMarker candidate) || candidate == marker)
                continue;

            if (candidate.time == marker.time)
            {
                Debug.LogError("Waypoint markers on the same track must have different times.", this);
                return;
            }

            if (candidate.time > marker.time && (next == null || candidate.time < next.time))
                next = candidate;
        }

        if (!IsValidWaypoint(marker.Waypoint) || (next != null && !IsValidWaypoint(next.Waypoint)))
        {
            Debug.LogError("Waypoint coordinates must be finite.", this);
            return;
        }

        if (director != source)
        {
            ReleaseDirector();
            director = source;
            director.stopped += OnDirectorStopped;
        }

        startWaypoint = marker;
        endWaypoint = next;
        // マーカー到達時の位置合わせは移動アニメーションに含めない。
        SetLocalPosition(marker.Waypoint);
        Moved?.Invoke(Vector2.zero);
    }

    void LateUpdate()
    {
        if (!IsActive)
            return;

        // Timelineの時刻で補間するため、Pause中は同じ位置が維持される。
        Vector2 position = startWaypoint.Waypoint;
        if (endWaypoint != null)
        {
            float progress = Mathf.Clamp01((float)((director.time - startWaypoint.time)
                / (endWaypoint.time - startWaypoint.time)));
            position = Vector2.Lerp(position, endWaypoint.Waypoint, progress);
        }

        Vector3 previousPosition = transform.position;
        SetLocalPosition(position);
        Vector3 delta = transform.position - previousPosition;
        Moved?.Invoke(new Vector2(delta.x, delta.y));

        // Hold終了では停止イベントが発生しない場合もあるため、終了時刻でも解除する。
        if (director.time >= director.duration)
            ReleaseDirector();
    }

    void OnDisable()
    {
        ReleaseDirector();
    }

    void OnDirectorStopped(PlayableDirector stoppedDirector)
    {
        ReleaseDirector();
    }

    void ReleaseDirector()
    {
        // 別のDirectorに切り替わる場合も、以前の購読を解除する。
        if (director != null)
            director.stopped -= OnDirectorStopped;

        director = null;
        startWaypoint = null;
        endWaypoint = null;
        Moved?.Invoke(Vector2.zero);
    }

    void SetLocalPosition(Vector2 position)
    {
        // WaypointはXYのみを指定し、既存のローカルZは変更しない。
        Vector3 localPosition = transform.localPosition;
        localPosition.x = position.x;
        localPosition.y = position.y;
        transform.localPosition = localPosition;
    }

    static bool IsValidWaypoint(Vector2 position)
    {
        return !float.IsNaN(position.x) && !float.IsInfinity(position.x)
            && !float.IsNaN(position.y) && !float.IsInfinity(position.y);
    }
}
