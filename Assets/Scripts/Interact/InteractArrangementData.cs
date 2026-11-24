using UnityEngine;

/// <summary>
/// 並べ替え用のデータ
/// 要素を正しい順序で並べる必要があるかどうか
/// 単一アイテムの並べ替え、または複数の素材を使った合成に使用する
/// </summary>
[CreateAssetMenu(fileName = "InteractArrangementData", menuName = "MouseTrap/InteractArrangementData")]
public class InteractArrangementData : InteractData
{
    [System.Serializable]
    public struct Element
    {
        [SerializeField, Range(1, 5)] int correctPosition;
        [SerializeField] InteractData ingredient;

        public int CorrectPosition => correctPosition;
        public InteractData Ingredient => ingredient;
    }

    public const int MinElements = 2;
    public const int MaxElements = 5;

    [Header("並べ替え・配置する要素")]
    [SerializeField] Element[] elements;
    [Header("並びを指定するか")]
    [SerializeField] bool requireOrder = true;
    [Header("完成品")]
    [SerializeField] InteractData completedItem;
    [Header("素材を使うか")]
    [SerializeField] bool useIngredients;

    public Element[] Elements => elements;
    public bool RequireOrder => requireOrder;
    public InteractData CompletedItem => completedItem;
    public bool UseIngredients => useIngredients;

    public bool ContainsIngredient(InteractData item)
    {
        if (!useIngredients || item == null || elements == null) return false;

        foreach (Element element in elements)
            if (element.Ingredient == item) return true;

        return false;
    }

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
            if (useIngredients && element.Ingredient == null)
            {
                error = "An arrangement ingredient is not assigned.";
                return false;
            }

            if (useIngredients && element.Ingredient.ArrangementRecipe != this)
            {
                error = $"Ingredient '{element.Ingredient.name}' does not reference this recipe.";
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