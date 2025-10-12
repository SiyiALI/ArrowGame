using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

//游戏底层规则

[System.Serializable]
public class Position
{
    public int r;
    public int c;

    public Position(int row, int col)
    {
        r = row;
        c = col;
    }
}

[System.Serializable]
public class Arrow
{
    public List<Position> positions = new List<Position>();
    public int direction; // 上0 下1 左2 右3

    public Arrow(List<Position> posList)
    {
        positions = posList;
        CalculateDirection();
    }

    public void CalculateDirection()
    {
        if (positions.Count < 2)
        {
            direction = -1;
            return;
        }

        Position tail = positions[positions.Count - 2];
        Position head = positions[positions.Count - 1];
        int dr = head.r - tail.r;
        int dc = head.c - tail.c;

        if (dr == -1 && dc == 0)
            direction = 0; // 上
        else if (dr == 1 && dc == 0)
            direction = 1; // 下
        else if (dr == 0 && dc == -1)
            direction = 2; // 左
        else if (dr == 0 && dc == 1)
            direction = 3; // 右
        else
            direction = -1;
    }
}

public class ArrowGame : MonoBehaviour
{
    
    public int rows = 6;
    public int cols = 6;
    public float cellSize = 100f;
    public RectTransform boardParent;
    public GameObject cellPrefab;
    public TextMeshProUGUI messageText;
    public ArrowDrawer drawer; 

    private ArrowCell[,] grid;
    private List<Arrow> arrows = new List<Arrow>();

    void Start()
    {
        GenerateArrows();// 生成箭头
        BuildBoard();// 生成格子
        DrawArrows();// 绘制箭头
    }

    // 绘制箭头
    void DrawArrows()
    {
        if (drawer == null)
        {
            UnityEngine.Debug.LogError("ArrowDrawer not filled by script！");
            return;
        }

        drawer.ClearAll();

        foreach (var arrow in arrows)
        {
           
            drawer.DrawArrow(arrow);
        }
    }
    
    //生成棋盘
    void BuildBoard()
    {
        foreach (Transform t in boardParent) Destroy(t.gameObject);
        grid = new ArrowCell[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject go = Instantiate(cellPrefab, boardParent);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(c * cellSize, -r * cellSize);
                ArrowCell cell = go.GetComponent<ArrowCell>();
                cell.Setup(this, r, c);
                grid[r, c] = cell;
            }
        }

        boardParent.sizeDelta = new Vector2(cols * cellSize, rows * cellSize);
    }

    //生成关卡箭头
    void GenerateArrows()
    {
        arrows.Clear();

        List<Position> p1 = new List<Position>()
         {
             new Position(0,0),
             new Position(1,0),
             new Position(1,1),
             new Position(2,1)
         };
         arrows.Add(new Arrow(p1));

                List<Position> p2 = new List<Position>()
         {
             new Position(2,0),
             new Position(3,0),
             new Position(3,1),
             new Position(4,1),
             new Position(5,1)
         };
         arrows.Add(new Arrow(p2));

                List<Position> p3 = new List<Position>()
         {
             new Position(5,0),
             new Position(4,0)
         };
         arrows.Add(new Arrow(p3));

                List<Position> p4 = new List<Position>()
         {
             new Position(0,1),
             new Position(0,2),
             new Position(0,3)
         };
         arrows.Add(new Arrow(p4));

                List<Position> p5 = new List<Position>()
         {
             new Position(0,4),
             new Position(0,5),
             new Position(1,5)
         };
         arrows.Add(new Arrow(p5));

                List<Position> p6 = new List<Position>()
         {
             new Position(1,4),
             new Position(1,3),
             new Position(1,2),
             new Position(2,2),
             new Position(2,3),
             new Position(2,4),
             new Position(2,5)
         };
         arrows.Add(new Arrow(p6));

                List<Position> p7 = new List<Position>()
         {
             new Position(3,3),
             new Position(3,2),
             new Position(4,2),
             new Position(5,2)
         };
         arrows.Add(new Arrow(p7));

                List<Position> p8 = new List<Position>()
         {
             new Position(5,3),
             new Position(4,3),
             new Position(4,4),
             new Position(3,4),
             new Position(3,5)
         };
         arrows.Add(new Arrow(p8));

                List<Position> p9 = new List<Position>()
         {
             new Position(4,5),
             new Position(5,5),
             new Position(5,4)
         };
         arrows.Add(new Arrow(p9));
    }

    //检查箭头是否可移除
    private bool CanRemove(Arrow arrow)
    {
        Position head = arrow.positions[arrow.positions.Count - 1];

        Position dir = arrow.direction switch
        {
            0 => new Position(-1, 0), //向上继续查看
            1 => new Position(1, 0),  //向下继续查看
            2 => new Position(0, -1), //向左继续查看
            3 => new Position(0, 1),  //向右继续查看
            _ => new Position(0, 0)
        };

        //从箭头头部沿方向检查
        Position check = new Position(head.r + dir.r, head.c + dir.c);

        while (check.r >= 0 && check.r < rows && check.c >= 0 && check.c < cols)
        {
            //如果有别的箭头占格子->false
            foreach (var other in arrows)
            {
                if (other == arrow) continue;

                foreach (var pos in other.positions)
                {
                    if (pos.r == check.r && pos.c == check.c)
                        return false; 
                }
            }

            check.r += dir.r;
            check.c += dir.c;
        }

        //没被挡->true
        return true;
    }

    private void RemoveArrow(Arrow arrow)
    {
        // 清除格子状态
        foreach (var pos in arrow.positions)
        {
            grid[pos.r, pos.c].Clear();
        }

        arrows.Remove(arrow);

        //场上没有箭头 -> 胜利
        if (arrows.Count == 0)
        {
            messageText.text = "You win!";
        }
    }

    //通过用户点击的格子寻找所属的箭头
    private Arrow FindArrowAt(int r, int c)
    {
        foreach (var arrow in arrows)
        {
            foreach (var p in arrow.positions)
            {
                if (p.r == r && p.c == c)
                    return arrow;
            }
        }
        return null;
    }

    // 点击格子
    public void OnArrowClicked(Arrow clickedArrow)
    {
        if (clickedArrow == null)
            return;
        if (CanRemove(clickedArrow))
        {
            messageText.text = "Arrow can move!";
            StartCoroutine(MoveArrowOut(clickedArrow));
        }
        else
        {
            messageText.text = "Blocked!";
        }
    }
    private IEnumerator MoveArrowOut(Arrow arrow)
    {
        // 箭头方向
        Position head = arrow.positions[arrow.positions.Count - 1];
        Position dir = arrow.direction switch
        {
            0 => new Position(-1, 0),
            1 => new Position(1, 0),
            2 => new Position(0, -1),
            3 => new Position(0, 1),
            _ => new Position(0, 0)
        };

        // 移动目标偏移
        Vector2 moveDir = new Vector2(dir.c, -dir.r);

        float duration = 0.5f;
        float elapsed = 0f;

        // 获取所有线段,点,箭头
        var objectsToMove = new List<RectTransform>();
        foreach (Transform child in drawer.boardParent)
        {
            var clickable = child.GetComponent<ArrowClickable>();
            if (clickable != null && clickable.linkedArrow == arrow)
                objectsToMove.Add(child.GetComponent<RectTransform>());
        }

        //平滑移动出棋盘
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            foreach (var obj in objectsToMove)
            {
                if (obj != null)
                    obj.anchoredPosition += moveDir * 300f * Time.deltaTime;
            }
            yield return null;
        }

        // 移除对象
        foreach (var obj in objectsToMove)
            if (obj != null) Destroy(obj.gameObject);

        arrows.Remove(arrow);
        messageText.text = "Arrow removed!";

        if (arrows.Count == 0)
            messageText.text = "You win!";
    }



}
