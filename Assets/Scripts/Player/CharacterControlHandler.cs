namespace Player
{
    using System;
    using UnityEngine;

    /// <summary>
    /// 操作キャラクターの切り替えを行う。解放状態と現在のキャラクターは <see cref="GameSession"/> が持つ。
    /// </summary>
    public class CharacterControlHandler : SceneSingleton<CharacterControlHandler>
    {
        [SerializeField] CharacterBaseData[] characterBaseData;
        Animator animator;

        public Character.Name CurrentCharacter => GameSession.CurrentCharacter;
        public event Action<Character.Name> OnCharacterUnlocked;
        public event Action<Character.Name> OnCharacterChanged;

        protected override void OnSingletonAwake()
        {
            animator = GetComponentInChildren<Animator>();

            // 新しいシーンのPlayerにも、現在のキャラクターの見た目を引き継ぐ
            if (GameSession.CurrentCharacter == Character.Name.None)
            {
                SetCharacterControlState(Character.Name.Girl, true);        // 最初は少女の操作を可能にする
                ChangeCharacterControlState(Character.Name.Girl);
            }
            else
            {
                ApplyCharacter(GameSession.CurrentCharacter);
            }
        }

        /// <summary>
        /// キャラクターの解放状態を設定する処理
        /// </summary>
        public void SetCharacterControlState(Character.Name characterName, bool isControllable)
        {
            if (characterName == Character.Name.None) return;

            if (!isControllable)
            {
                GameSession.UnlockedCharacters.Remove(characterName);
                return;
            }

            if (GameSession.UnlockedCharacters.Add(characterName))
                OnCharacterUnlocked?.Invoke(characterName);
        }

        public bool IsUnlocked(Character.Name characterName) => GameSession.UnlockedCharacters.Contains(characterName);

        /// <summary>
        /// 操作するキャラクターを切り替える処理
        /// </summary>
        /// <param name="characterName">操作したいキャラクターの名前</param>
        /// <returns>切り替えが成功したかどうか</returns>
        public bool ChangeCharacterControlState(Character.Name characterName)
        {
            if (!IsUnlocked(characterName)) return false;
            if (characterName == GameSession.CurrentCharacter) return false;

            if (!ApplyCharacter(characterName)) return false;

            GameSession.CurrentCharacter = characterName;
            OnCharacterChanged?.Invoke(characterName);
            return true;
        }

        bool ApplyCharacter(Character.Name characterName)
        {
            CharacterBaseData data = GetCharacterData(characterName);
            if (data == null || data.Animation == null)
            {
                Debug.LogWarning($"Character data or animation is not set: {characterName}");
                return false;
            }

            if (animator == null)
            {
                Debug.LogError($"Animator is not set on the Player for character: {characterName}", this);
                return false;
            }

            animator.runtimeAnimatorController = data.Animation;
            return true;
        }

        /// <summary>
        /// 指定したキャラクターのデータを取得する処理
        /// </summary>
        /// <returns>キャラクターのデータ。存在しない場合はnull。</returns>
        public CharacterBaseData GetCharacterData(Character.Name characterName)
        {
            foreach (var character in characterBaseData)
            {
                if (character != null && character.CharacterNameEnum == characterName)
                    return character;
            }
            return null;
        }
    }
}