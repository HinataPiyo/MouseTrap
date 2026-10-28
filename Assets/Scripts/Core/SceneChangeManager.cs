using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 部屋移動を行う。移動可否の状態は <see cref="GameSession"/> が持つ。
/// </summary>
public class SceneChangeManager : SceneSingleton<SceneChangeManager>
{
    Room.Name currentRoom = Room.Name.None;
    bool isChanging;

    protected override void OnSingletonAwake()
    {
        currentRoom = Room.GetCurrentRoom(gameObject.scene.name);
    }

    /// <summary>
    /// 指定した部屋の移動可能状態を設定する
    /// </summary>
    public void CanMoveRoom(Room.Name room, bool canMove)
    {
        if (room == Room.Name.None) return;

        if (canMove)
            GameSession.UnlockedRooms.Add(room);
        else
            GameSession.UnlockedRooms.Remove(room);
    }

    /// <summary>
    /// 現在の部屋より後ろで、移動可能な最も近い部屋を取得する
    /// </summary>
    public Room.Name GetNextMoveableRoom()
    {
        foreach (Room.Name room in Enum.GetValues(typeof(Room.Name)))
        {
            if (room > currentRoom && GameSession.UnlockedRooms.Contains(room))
                return room;
        }
        return Room.Name.None;
    }

    /// <summary>
    /// 現在の部屋より前で、移動可能な最も近い部屋を取得する
    /// </summary>
    public Room.Name GetBackMoveableRoom()
    {
        Room.Name lastRoom = Room.Name.None;
        foreach (Room.Name room in Enum.GetValues(typeof(Room.Name)))
        {
            if (room != Room.Name.None && room < currentRoom && GameSession.UnlockedRooms.Contains(room))
                lastRoom = room;
        }
        return lastRoom;
    }

    public void SceneChange(Room.Name room)
    {
        // 連続クリックや多重呼び出しで、シーン読み込みが重ならないようにする
        if (isChanging) return;
        if (!GameSession.UnlockedRooms.Contains(room)) return;
        if (!Room.RoomNameWithSceneName.ContainsKey(room)) return;

        StartCoroutine(SceneChangeCoroutine(room));
    }

    IEnumerator SceneChangeCoroutine(Room.Name room)
    {
        isChanging = true;
        PlayerInputController.Instance.ChangeState(StateType.Movie);

        yield return FadeScreen.Instance.FadeInCoroutine();

        // 読み込み後のシーンで、暗転から明けるフェードアウトを再生させる
        GameSession.FadeOutPending = true;
        SceneManager.LoadScene(Room.RoomNameWithSceneName[room]);
    }

    public Room.Name GetCurrentRoom()
    {
        return currentRoom;
    }
}
