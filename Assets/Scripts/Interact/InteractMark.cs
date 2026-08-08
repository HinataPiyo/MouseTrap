using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WorldCanvasに表示するInteractMarkのPrefabを管理するクラス
/// </summary>
public class InteractMark : MonoBehaviour
{
    [System.Serializable]
    public class InteractMarkData
    {
        public InteractMarkType type;
        public Sprite sprite;
    }

    [SerializeField] InteractMarkData[] interactMarkDatas;
    [SerializeField] Image image;
    Transform point;

    bool isVisible = false;

    /// <summary>
    /// InteractMarkTypeに応じて、InteractMarkのSpriteを変更する
    /// </summary>
    public void SetMark(InteractMarkType type, Transform point)
    {
        this.point = point;
        isVisible = true;

        foreach (var data in interactMarkDatas)
        {
            if (data.type == type)
            {
                image.sprite = data.sprite;
                return;
            }
        }
    }

    void Update()
    {
        if(isVisible)
        {
            transform.position = point.position;
        }
    }

    void OnDisable()
    {
        isVisible = false;
    }
}