using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneChange
{
    void SceneChange(Room.Name room);
    Room.Name GetCurrentRoom();
    List<Room.Name> GetMoveableRooms();
    void CanMoveRoom(Room.Name room, bool canMove);
}

public class SceneChangeManager : MonoBehaviour, ISceneChange
{
    Room.Name currentRoom = Room.Name.None;

    // 移動可能な部屋を管理する
    Dictionary<Room.Name, bool> canMoveRooms = new Dictionary<Room.Name, bool>();
    IGameState gameState;

    void Awake()
    {
        ApiProvider.Register<ISceneChange>(this);
        currentRoom = Room.GetCurrentRoom(SceneManager.GetActiveScene().name);
        CanMoveRoom(Room.Name.Staffroom, true);
        CanMoveRoom(Room.Name.Library, true);   // テストとして開放
    }

    void Start()
    {
        gameState = ApiProvider.Get<IGameState>();
    }

    /// <summary>
    /// 指定した部屋の移動可能状態を設定する
    /// </summary>
    public void CanMoveRoom(Room.Name room, bool canMove)
    {
        canMoveRooms[room] = canMove;
    }

    /// <summary>
    /// 現在移動可能な部屋のリストを取得する
    /// 現在の部屋を除外した部屋リスト
    /// </summary>
    public List<Room.Name> GetMoveableRooms()
    {
        List<Room.Name> moveableRooms = new List<Room.Name>();
        foreach (var kvp in canMoveRooms)
        {
            if (kvp.Value && kvp.Key != currentRoom)
            {
                moveableRooms.Add(kvp.Key);
            }
        }
        return moveableRooms;
    }

    public void SceneChange(Room.Name room)
    {
        StartCoroutine(SceneChangeCoroutine(room));
    }


    IEnumerator SceneChangeCoroutine(Room.Name room)
    {

        // フェードなどを実装
        currentRoom = room;
        yield return null;

        SceneManager.LoadScene(Room.RoomNameWithSceneName[room]);
        gameState.ChangeState(StateType.None);
    }

    public Room.Name GetCurrentRoom()
    {
        return currentRoom;
    }
}