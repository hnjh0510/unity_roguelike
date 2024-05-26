using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleSpace : MonoBehaviour
{
    public Vector2Int leftDown;//직사각형 왼쪽 아래 좌표(기준점)
    public int width;//너비
    public int height;//높이

    public RectangleSpace(Vector2Int leftDown, int width, int height)
    {
        this.leftDown = leftDown;
        this.width = width;
        this.height = height;
    }

    public Vector2Int Center()//직사각형의 중심찾기
    {
        return new Vector2Int(((leftDown.x * 2)+ width - 1)/2, ((leftDown.y * 2)+ height - 1)/2);
    }
    
}
