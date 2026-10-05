namespace Player
{
    using System.Collections.Generic;
    using UnityEditor.Animations;
    using UnityEngine;

    //! 現在キャラ追加の処理を作ったから次は、追加したキャラに切り替えて移動や特殊能力を使えるようにする

    /// <summary>
    /// キャラクターの操作可能状態を管理するクラス
    /// </summary>
    public class CharacterControlHandler : MonoBehaviour
    {
        public static CharacterControlHandler Instance { get; private set; }

        [System.Serializable]
        public class CharacterWithAnimator
        {
            public Character.Name characterName;
            public AnimatorController animation;
        }

        Animator animator;
        [SerializeField] CharacterWithAnimator[] charactersWithAnimators;
        Dictionary<Character.Name, bool> characterControlStates = new Dictionary<Character.Name, bool>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetInstance()
        {
            Instance = null;
        }

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        void OnEnable()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            characterControlStates.Clear();
            SetCharacterControlState(Character.Name.Girl, true);        // 最初は少女の操作を可能にする
        }

        /// <summary>
        /// キャラクターの操作可能状態を設定する処理
        /// </summary>
        public void SetCharacterControlState(Character.Name characterName, bool isControllable)
        {
            characterControlStates[characterName] = isControllable;
        }

        /// <summary>
        /// キャラクターの操作可能状態を切り替える処理
        /// </summary>
        /// <param name="characterName">操作可能状態を切り替えるキャラクターの名前</param>
        /// <returns>切り替えが成功したかどうか</returns>
        public bool ChangeCharacterControlState(Character.Name characterName)
        {
            if (!characterControlStates.ContainsKey(characterName))
            {
                return false;
            }

            bool currentState = characterControlStates[characterName];
            characterControlStates[characterName] = !currentState;

            animator.runtimeAnimatorController = GetAnimatorControllerForCharacter(characterName);
            Debug.Log($"<color=green>Character {characterName} control state changed to {!currentState}</color>");
            return true;
        }

        /// <summary>
        /// 指定したキャラクターに対応するAnimatorControllerを取得する処理
        /// </summary>
        /// <param name="characterName">取得したいキャラクターの名前</param>
        /// <returns>キャラクターに対応するAnimatorController。存在しない場合はnull。</returns>
        AnimatorController GetAnimatorControllerForCharacter(Character.Name characterName)
        {
            foreach (var characterWithAnimator in charactersWithAnimators)
            {
                if (characterWithAnimator.characterName == characterName)
                {
                    return characterWithAnimator.animation;
                }
            }
            return null;
        }
    }
}