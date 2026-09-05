namespace Player
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface ICharacterState
    {
        void SetCharacterControlState(Character.Name characterName, bool isControllable);
    }
    
    //! 現在キャラ追加の処理を作ったから次は、追加したキャラに切り替えて移動や特殊能力を使えるようにする

    /// <summary>
    /// キャラクターの操作可能状態を管理するクラス
    /// </summary>
    public class CharacterControlHandler : MonoBehaviour, ICharacterState
    {
        Dictionary<Character.Name, bool> characterControlStates = new Dictionary<Character.Name, bool>();

        void Awake()
        {
            ApiProvider.Register<ICharacterState>(this);
            SetCharacterControlState(Character.Name.Girl, true);        // 最初は少女の操作を可能にする
        }

        /// <summary>
        /// キャラクターの操作可能状態を設定する処理
        /// </summary>
        public void SetCharacterControlState(Character.Name characterName, bool isControllable)
        {
            characterControlStates[characterName] = isControllable;
        }
    }
}