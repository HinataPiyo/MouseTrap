namespace Player
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Movement : MonoBehaviour
    {

        [SerializeField] float moveSpeed = 2f;
        [SerializeField] Animator ainm;
        enum MoveType { None = -1, Idle, Left, Right, Up, Down }

        Vector2 moveDir;
        Rigidbody2D rb;
        MoveType currentMoveType;

        IGameState gameState;

        bool isStopped = false;


        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            gameState = ApiProvider.Get<IGameState>();
        }

        void Update()
        {
            float x = Keyboard.current.aKey.isPressed ? -1f : Keyboard.current.dKey.isPressed ? 1f : 0f;
            float y = Keyboard.current.sKey.isPressed ? -1f : Keyboard.current.wKey.isPressed ? 1f : 0f;
            moveDir = new Vector2(x, y).normalized;

            DirectionToAnimation(moveDir);
        }

        void LateUpdate()
        {
            if (!isStopped)
            {
                rb.linearVelocity = moveDir * moveSpeed;
            }
        }

        public void DirectionToAnimation(Vector2 dir)
        {
            MoveType previousMoveType;

            if(dir.sqrMagnitude < 0.0001f || gameState.CurrentState == StateType.Talk)
            {
                previousMoveType = MoveType.Idle;
            }
            // 入力の絶対値が大きい方を優先してアニメーションを切り替える
            else if(Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
            {
                previousMoveType = dir.y > 0 ? MoveType.Up : MoveType.Down;
            }
            // 横の入力の方が大きい場合は左右のアニメーションを切り替える
            else
            {
                previousMoveType = dir.x > 0 ? MoveType.Right : MoveType.Left;
            }

            // 入力の方向が変わったときだけアニメーションを切り替える
            if(previousMoveType != currentMoveType)
            {
                currentMoveType = previousMoveType;
                ainm.SetInteger("MoveType", (int)currentMoveType);
            }
        }
    }

}