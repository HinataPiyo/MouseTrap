using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WorldCanvasに表示するIntaractMarkのPrefabを管理するクラス
/// </summary>
public class IntaractMark : MonoBehaviour
{
    [System.Serializable]
    public class IntaractMarkData
    {
        public IntaractMarkType type;
        public Sprite sprite;
    }

    [SerializeField] IntaractMarkData[] intaractMarkDatas;
    [SerializeField] Image image;
    Transform point;

    bool isVisible = false;

    /// <summary>
    /// IntaractMarkTypeに応じて、IntaractMarkのSpriteを変更する
    /// </summary>
    public void SetMark(IntaractMarkType type, Transform point)
    {
        this.point = point;
        isVisible = true;

        foreach (var data in intaractMarkDatas)
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