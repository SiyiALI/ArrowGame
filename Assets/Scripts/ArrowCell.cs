using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class ArrowCell : MonoBehaviour
{
    private ArrowGame board;
    private int row, col;
    private Image img;

    public bool HasArrowPart { get; private set; } = false;

    //初始化棋盘
    public void Setup(ArrowGame b, int r, int c)
    {
        board = b;
        row = r;
        col = c;

        img = GetComponent<Image>();
        if (img != null)
        {
            img.enabled = true;
            img.color = new Color(0.8f, 0.8f, 0.8f, 0.4f);
        }
    }

    // 设置格子是否属于箭头的一部分
    public void SetArrowPart(bool hasArrow)
    {
        HasArrowPart = hasArrow;
    }

    //清除格子状态
    public void Clear()
    {
        HasArrowPart = false;
        if (img != null)
            img.color = new Color(0.9f, 0.9f, 0.9f, 0.4f);
    }

    
}
