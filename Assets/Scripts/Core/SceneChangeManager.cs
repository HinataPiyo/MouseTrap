using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance { get; private set; }
    Room.Name currentRoom = Room.Name.None;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetInstance()
    {
        Instance = null;
    }

    // 移動可能な部屋を管理する
    Dictionary<Room.Name, bool> canMoveRooms = new Dictionary<Room.Name, bool>();
    PlayerInputController gameState;
    FadeScreen fadeScreen;

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentRoom = Room.GetCurrentRoom(SceneManager.GetActiveScene().name);
        canMoveRooms.Clear();
        CanMoveRoom(Room.Name.Staffroom, true);
        CanMoveRoom(Room.Name.Library, true);       //! テスト
    }

    void Start()
    {
        gameState = PlayerInputController.Instance;
        fadeScreen = FadeScreen.Instance;
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
    public Room.Name GetNextMoveableRoom()
    {
        foreach (var kvp in canMoveRooms)
        {
            // 現在の位置と現在の位置から前の位置は除外する
            if (kvp.Value && (int)kvp.Key > (int)currentRoom)
            {
                return kvp.Key;
            }
        }
        return Room.Name.None;
    }

    public Room.Name GetBackMoveableRoom()
    {
        Room.Name lastRoom = Room.Name.None;
        foreach (var kvp in canMoveRooms)
        {
            if (kvp.Value && (int)kvp.Key < (int)currentRoom)
            {
                lastRoom = kvp.Key;
            }
        }
        return lastRoom;
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