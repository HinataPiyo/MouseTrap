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
    static SceneChangeManager instance;
    Room.Name currentRoom = Room.Name.None;

    // 移動可能な部屋を管理する
    Dictionary<Room.Name, bool> canMoveRooms = new Dictionary<Room.Name, bool>();
    IGameState gameState;
    IFadeScreen fadeScreen;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ApiProvider.Register<ISceneChange>(this);
        currentRoom = Room.GetCurrentRoom(SceneManager.GetActiveScene().name);
        CanMoveRoom(Room.Name.Staffroom, true);
    }

    void Start()
    {
        gameState = ApiProvider.Get<IGameState>();
        fadeScreen = ApiProvider.Get<IFadeScreen>();
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
        yield return fadeScreen.FadeInCoroutine();
        currentRoom = room;

        SceneManager.LoadScene(Room.RoomNameWithSceneName[room]);
        gameState.ChangeState(StateType.None);

        yield return fadeScreen.FadeOutCoroutine();
    }

    public Room.Name GetCurrentRoom()
    {
        return currentRoom;
    }
}