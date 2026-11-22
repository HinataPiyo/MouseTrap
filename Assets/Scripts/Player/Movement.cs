namespace Player
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Movement : MonoBehaviour
    {

        [SerializeField] float moveSpeed = 2f;
        [SerializeField] Animator ainm;
        public enum MoveType { None = -1, Idle, Left, Right, Up, Down }

        Vector2 moveDir;
        Rigidbody2D rb;
        MoveType currentMoveType;

        PlayerInputController gameState;
        CharacterControlHandler characterControlHandler;
        DirectorWaypointMovement directorMovement;

        bool isStopped = false;


        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // Timeline移動自体は共通コンポーネントに任せ、方向アニメーションだけ連携する。
            directorMovement = GetComponent<DirectorWaypointMovement>();
            if (directorMovement != null)
                directorMovement.Moved += OnDirectorMoved;
        }

        void Start()
        {
            gameState = PlayerInputController.Instance;
            characterControlHandler = CharacterControlHandler.Instance;
            if (characterControlHandler == null)
            {
                Debug.LogError("CharacterControlHandler is not available.", this);
                enabled = false;
                return;
            }

            characterControlHandler.OnCharacterChanged += OnCharacterChanged;
        }

        void OnDestroy()
        {
            if (characterControlHandler != null)
                characterControlHandler.OnCharacterChanged -= OnCharacterChanged;
            if (directorMovement != null)
                directorMovement.Moved -= OnDirectorMoved;
        }

        void OnDirectorMoved(Vector2 displacement)
        {
            // 移動量が一時的にゼロでも、再生中の区間の方向を維持する。
            if (isActiveAndEnabled && DirectorManager.Instance != null
                && DirectorManager.Instance.CurrentState != DirectorManager.State.None)
                UpdateMoveType(directorMovement.MovementDirection.normalized);
        }

        void OnCharacterChanged(Character.Name characterName)
        {
            ainm.SetInteger("MoveType", (int)currentMoveType);
        }

        void Update()
        {
            DirectorManager.State directorState = DirectorManager.Instance.CurrentState;
            if (directorState != DirectorManager.State.None)
            {
                // Waypoint制御がない演出中は移動アニメーションを停止する。
                if (directorMovement == null || !directorMovement.IsActive)
                    UpdateMoveType(Vector2.zero);
                return;
            }

            float x = 0f;
            float y = 0f;

            if (!isStopped)
            {
                x = Keyboard.current.aKey.isPressed ? -1f : Keyboard.current.dKey.isPressed ? 1f : 0f;
                y = Keyboard.current.sKey.isPressed ? -1f : Keyboard.current.wKey.isPressed ? 1f : 0f;
            }
            moveDir = new Vector2(x, y).normalized;

            DirectionToAnimation(moveDir);
        }

        void LateUpdate()
        {
            if (DirectorManager.Instance.CurrentState != DirectorManager.State.None)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            if (!isStopped)
            {
                rb.linearVelocity = moveDir * moveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        public void DirectionToAnimation(Vector2 dir)
        {
            if (gameState.CurrentState == StateType.Talk || gameState.CurrentState == StateType.Interact
            || gameState.CurrentState == StateType.RoomChange || gameState.CurrentState == StateType.Movie)
            {
                currentMoveType = MoveType.Idle;
                isStopped = true;
                ainm.SetInteger("MoveType", (int)currentMoveType);
                return;
            }
            else
            {
                isStopped = false;
            }

            UpdateMoveType(dir);
        }

        void UpdateMoveType(Vector2 dir)
        {
            MoveType previousMoveType;
            if (dir.sqrMagnitude < 0.0001f)
            {
                previousMoveType = MoveType.Idle;
            }
            // 入力の絶対値が大きい方を優先してアニメーションを切り替える
            else if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
            {
                previousMoveType = dir.y > 0 ? MoveType.Up : MoveType.Down;
            }
            // 横の入力の方が大きい場合は左右のアニメーションを切り替える
            else
            {
                previousMoveType = dir.x > 0 ? MoveType.Right : MoveType.Left;
            }

            // 入力の方向が変わったときだけアニメーションを切り替える
            if (previousMoveType != currentMoveType)
            {
                currentMoveType = previousMoveType;
                ainm.SetInteger("MoveType", (int)currentMoveType);
            }
        }
    }

}