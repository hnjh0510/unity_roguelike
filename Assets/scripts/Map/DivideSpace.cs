using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideSpace : MonoBehaviour
{
    public int totalWidth;//맵 전체의 너비
    public int totalHeight;//맵 전체의 높이
    [SerializeField]
    private int minWidth;//분할된 공간의 최소너비
    [SerializeField]
    private int minHeight;//분할된 공간의 최소높이

    public RectangleSpace totalspace;//나눠지기 전의 맵

    public List<RectangleSpace> spaceList;//나누어진 공간들을 저장할 리스트

    public void DivideRoom(RectangleSpace space)//나눌수없을때까지 나누고 나온것을 spaceList에 넣는다
    {
        if(space.height >= minHeight*2 && space.width >= minWidth*2)//높이, 너비 둘다 2배 이상
        {
            if (Random.Range(1, 2) == 1)//50%확률
            {
                RectangleSpace[] spaces = DivideHorizontal(space);//가로로 자른다

                DivideRoom(spaces[0]);//나눈것을 다시 DivideRoom()함수를 돌린다
                DivideRoom(spaces[1]);//나눈것을 다시 DivideRoom()함수를 돌린다
            }
            else if(Random.Range(1, 2) == 2)
            {
                RectangleSpace[] spaces = DivideVertical(space);//세로로 자른다

                DivideRoom(spaces[0]);//나눈것을 다시 DivideRoom()함수를 돌린다
                DivideRoom(spaces[1]);//나눈것을 다시 DivideRoom()함수를 돌린다
            }
        }
        else if(space.height < minHeight *2 && space.width >= minWidth * 2)//너비만 2배 이상
        {
            RectangleSpace[] spaces = DivideVertical(space);//세로로 자른다

            DivideRoom(spaces[0]);//나눈것을 다시 DivideRoom()함수를 돌린다
            DivideRoom(spaces[1]);//나눈것을 다시 DivideRoom()함수를 돌린다
        }
        else if(space.height >= minHeight * 2 && space.width < minWidth * 2)//높이만 2배 이상
        {
            RectangleSpace[] spaces = DivideHorizontal(space);//가로로 자른다

            DivideRoom(spaces[0]);//나눈것을 다시 DivideRoom()함수를 돌린다
            DivideRoom(spaces[1]);//나눈것을 다시 DivideRoom()함수를 돌린다
        }
        else//둘다 2배 보다 작음
        {
            spaceList.Add(space);//spaceList에 해당 공간을 저장
        }
    }

    private RectangleSpace[] DivideHorizontal(RectangleSpace space)//위아래로 둘로 나누기(가로로 자른다)
    {
        int newSpace1Height = minHeight + Random.Range(0, space.height - minHeight * 2 + 1);//아래쪽 사각형의 높이
                            //최소높이 +  랜덤(0 ~ 자르려는 사각형의 높이 - 최소 높이2개 +1)
        RectangleSpace newSpace1 = new RectangleSpace(space.leftDown, space.width, newSpace1Height);//새로운 사각형 newSpace1을 만듬

        int newSpace2Height = space.height - newSpace1Height;//newSpace2Height(newSpace2의 높이) = 자르려는 사각형의 높이 - newSpace1의 높이
        Vector2Int newSpace2LeftDown = new Vector2Int(space.leftDown.x, space.leftDown.y + newSpace1Height);//newSpace2의 새로운 왼쪽아래점(기준점)을 생성
                                    //x값은 기존의 사각형의 x값 그대로, y값은 기존의 사각형의 y값에 newSpace1의 높이를 더한다
        RectangleSpace newSpace2 = new RectangleSpace(newSpace2LeftDown, space.width, newSpace2Height);//새로운 사각형 newSpace2를 만듬

        RectangleSpace[] spaces = new RectangleSpace[2];//spaces배열을 만든다
        spaces[0] = newSpace1;//spaces[0]에 newSpace1을 넣음
        spaces[1] = newSpace2;//spaces[1]에 newSpace2를 넣음
        return spaces;//spaces를 반환
    }

    private RectangleSpace[] DivideVertical(RectangleSpace space)//양옆으로 둘로 나눈다(세로로 자른다)
    {
        int newSpace1Width = minWidth + Random.Range(0, space.width - minWidth * 2 + 1);//왼쪽사각형의 너비
                           //최소너비 + 랜덤(0 ~ 자르려는 사각형의 너비 - 최소너비2개 +1)
        RectangleSpace newSpace1 = new RectangleSpace(space.leftDown, newSpace1Width, space.height);//새로운 사각형 newSpace1을 만듬

        int newSpace2Width = space.width - newSpace1Width;//newSpace2Width(newSpace2의 너비) = 자르려는 사각형의 너비 - newSpace1의 너비
        Vector2Int newSpace2LeftDown = new Vector2Int(space.leftDown.x + newSpace1Width, space.leftDown.y);//newSpace2의 새로운 왼쪽아래점(기준점)을 생성
                                    //자르려는 사각형의 x값에 newSpace1의 너비를 더한다, y값은 기존의 y값 그대로
        RectangleSpace newSpace2 = new RectangleSpace(newSpace2LeftDown, newSpace2Width, space.height);//새로운 사각형 newSpace2를 만듬

        RectangleSpace[] spaces = new RectangleSpace[2];//spaces배열을 만듬
        spaces[0] = newSpace1;//spaces[0]에 newSpace1을 넣음
        spaces[1] = newSpace2;//spaces[1]에 newSpace2를 넣음
        return spaces;//spaces를 반환
    }
}
