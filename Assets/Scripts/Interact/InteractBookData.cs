using UnityEngine;


public enum LibraryBookId
{
    OneLittleChild,    // 一匹のちいさな子
    TwoRivals,         // 二匹の競争
    ThreeDepartures,   // 3人の旅立ち
    FourCandles,       // 四本のロウソク
    SomedayAgain       // いつかまた
}

[CreateAssetMenu(fileName = "InteractBookData", menuName = "MouseTrap/InteractBookData")]
public class InteractBookData : InteractData
{
    [SerializeField] LibraryBookId bookId;
    [SerializeField] Sprite ui;

    public LibraryBookId BookId => bookId;
    public Sprite UI => ui;
}