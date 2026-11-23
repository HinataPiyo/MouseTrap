using UnityEngine;

[CreateAssetMenu(fileName = "InteractArrangementData", menuName = "MouseTrap/InteractArrangementData")]
public class InteractArrangementData : InteractData
{
    [System.Serializable]
    public struct Element
    {
        [SerializeField] Sprite sprite;
        [SerializeField, Range(1, 5)] int correctPosition;

        public Sprite Sprite => sprite;
        public int CorrectPosition => correctPosition;
    }

    public const int MinElements = 2;
    public const int MaxElements = 5;

    [Header("並べ替え・配置する要素")]
    [SerializeField] Element[] elements;
    [SerializeField] bool requireOrder = true;
    [SerializeField] InteractData completedItem;

    public Element[] Elements => elements;
    public bool RequireOrder => requireOrder;
    public InteractData CompletedItem => completedItem;

    public bool TryValidate(out string error)
    {
        if (elements == null || elements.Length < MinElements || elements.Length > MaxElements)
        {
            error = $"The number of arrangement elements must be between {MinElements} and {MaxElements}.";
            return false;
        }

        if (completedItem == null)
        {
            error = "The completed item is not assigned.";
            return false;
        }

        bool[] positions = new bool[elements.Length];
        foreach (Element element in elements)
        {
            if (element.Sprite == null)
            {
                error = "An arrangement element has no sprite.";
                return false;
            }

            if (!requireOrder) continue;

            int position = element.CorrectPosition - 1;
            if (position < 0 || position >= elements.Length || positions[position])
            {
                error = "Correct positions must be unique and range from 1 to the number of elements.";
                return false;
            }

            positions[position] = true;
        }

        error = null;
        return true;
    }
}