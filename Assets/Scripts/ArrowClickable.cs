using UnityEngine;
using UnityEngine.EventSystems;
//让箭头能点击
public class ArrowClickable : MonoBehaviour, IPointerClickHandler
{
    private ArrowGame game;
    public Arrow linkedArrow;

    public void Setup(ArrowGame g, Arrow a)
    {
        game = g;
        linkedArrow = a;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (game != null && linkedArrow != null)
        {
            game.OnArrowClicked(linkedArrow);
        }
    }
}
