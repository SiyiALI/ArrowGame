using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;

public class ArrowDrawer : MonoBehaviour
{
    public RectTransform boardParent;   // 棋盘UI
    public GameObject pointPrefab;      // 点的UI
    public GameObject linePrefab;       // 线段的UI
    public GameObject arrowPrefab;

    public float cellSize = 100f;
    public int rows = 6, cols = 6;
    private List<GameObject> activeObjects = new List<GameObject>();

    // 清除旧图形
    public void ClearAll()
    {
        foreach (var obj in activeObjects)
            Destroy(obj);
        activeObjects.Clear();
    }

    // 输入坐标，绘制箭头
    public void DrawArrow(Arrow arrow)
    {
        var coords = new List<Vector2Int>();
        foreach (var p in arrow.positions)
            coords.Add(new Vector2Int(p.r, p.c));

        for (int i = 0; i < coords.Count; i++)
        {
            Vector2 pos = new Vector2(coords[i].y * cellSize, -coords[i].x * cellSize);
            var point = Instantiate(pointPrefab, boardParent);
            point.GetComponent<RectTransform>().anchoredPosition = pos;

            //点击
            var click = point.AddComponent<ArrowClickable>();
            click.Setup(FindFirstObjectByType<ArrowGame>(), arrow);


            activeObjects.Add(point);

            if (i < coords.Count - 1)
            {
                Vector2 nextPos = new Vector2(coords[i + 1].y * cellSize, -coords[i + 1].x * cellSize);
                var line = DrawLine(pos, nextPos, arrow);
                activeObjects.Add(line);
            }
        }

        // 创建箭头头部
        CreateHead(coords[coords.Count - 2], coords[coords.Count - 1], arrow);
    }


    //创建坐标之间的线段
    private GameObject DrawLine(Vector2 start, Vector2 end, Arrow arrow)
    {
        var line = Instantiate(linePrefab, boardParent);
        RectTransform rt = line.GetComponent<RectTransform>();
        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rt.sizeDelta = new Vector2(distance, 10f);
        rt.anchoredPosition = (start + end) / 2f;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);

        var click = line.AddComponent<ArrowClickable>();
        click.Setup(FindFirstObjectByType<ArrowGame>(), arrow);

        return line;
    }

    //创建箭头
    private void CreateHead(Vector2Int from, Vector2Int to, Arrow arrow)
    {
        Vector2 start = new Vector2(from.y * cellSize, -from.x * cellSize);
        Vector2 end = new Vector2(to.y * cellSize, -to.x * cellSize);
        Vector2 dir = (end - start).normalized;

        GameObject diamond = Instantiate(arrowPrefab, boardParent);
        RectTransform rt = diamond.GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(28f, 28f);
        rt.anchoredPosition = end;
        rt.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 45f);
        var img = diamond.GetComponent<UnityEngine.UI.Image>();
        img.color = Color.black;

 
        var click = diamond.AddComponent<ArrowClickable>();
        click.Setup(FindFirstObjectByType<ArrowGame>(), arrow);


        activeObjects.Add(diamond);
    }

}
