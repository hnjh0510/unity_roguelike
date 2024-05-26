using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class MakeRandomMap : MonoBehaviour
{
    [SerializeField]
    private int distance;//방사이 거리
    [SerializeField]
    private int minRoomWidth;//최소 방 너비
    [SerializeField]
    private int minRoomHeight;//최소 방 높이
    [SerializeField]
    private DivideSpace divideSpace;//나누어진 공간
    [SerializeField]
    private SpreadTilemap spreadTilemap;//타일깔기용
    [SerializeField]
    private GameObject player;//플레이어
    [SerializeField]
    private GameObject entrance;//다음씬 넘어가는 포탈
    [SerializeField]
    private GameObject box;//상자

    public GameObject enemy;

    [SerializeField]
    private List<RectangleSpace> roomSpace;

    public GameObject isinside;

    private HashSet<Vector2Int> floor;//floor타일
    private HashSet<Vector2Int> wall;//wall타일
    private HashSet<Vector2Int> door;//door타일



    private void Start()
    {
        StartRandomMap();
    }

    public void StartRandomMap()
    {
        spreadTilemap.ClearAllTiles();//깔려있는 타일 제거
        divideSpace.totalspace = new RectangleSpace(new Vector2Int(0, 0), divideSpace.totalWidth, divideSpace.totalHeight);
        //divideSpace의 totalspace(자를 사각형)을 만듬
        divideSpace.spaceList = new List<RectangleSpace>();//divideSpace의 spaceList(잘릴 사각형공간들의 리스트)를 만듬
        floor = new HashSet<Vector2Int>();
        wall = new HashSet<Vector2Int>();
        door = new HashSet<Vector2Int>();
        divideSpace.DivideRoom(divideSpace.totalspace);//사각형을 자름
        MakeRandomRooms();//방

        MakeDoor();//문
        spreadTilemap.SpreadDoorTilemap(door);//door타일 깔기

        MakeCorridors();//복도

        MakeWall();//벽

        spreadTilemap.SpreadFloorTilemap(floor);//floor타일 깔기
        spreadTilemap.SpreadWallTilemap(wall);//wall타일 깔기
        
        int pRand = Random.Range(0, roomSpace.Count);
        player.transform.position = (Vector2)roomSpace[pRand].Center();
        roomSpace.RemoveAt(pRand);

        int exitRand = Random.Range(0, roomSpace.Count);
        entrance.transform.position = (Vector2)roomSpace[exitRand].Center();
        roomSpace.RemoveAt(exitRand);

        int boxRand = Random.Range(0, roomSpace.Count);
        box.transform.position = (Vector2)roomSpace[boxRand].Center();
        roomSpace.RemoveAt(boxRand);

        int count = roomSpace.Count;
        for(int i=0; i < count; i++)
        {
            Instantiate(enemy, new Vector3(roomSpace[i].Center().x, roomSpace[i].Center().y), Quaternion.identity);
        }

        /*foreach(var room in roomSpace)
        {
            int xrand = Random.Range(1, room.width - 1);
            int hrand = Random.Range(1, room.height - 1);
            Instantiate(enemy, new Vector3(xrand, hrand), Quaternion.identity);
        }*/
    }

    private void MakeRandomRooms()
    {
        foreach(var space in divideSpace.spaceList)//spaceList안에 있는 모든 space만큼
        {
            HashSet<Vector2Int> positions = MakeARandomRectangleRoom(space);//space의 위치를 저장
            floor.UnionWith(positions);//UnionWith로 위치를 floor에 저장한다
        }
    }

    private HashSet<Vector2Int> MakeARandomRectangleRoom(RectangleSpace space)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();//positions를 만듬
        int width = Random.Range(minRoomWidth, space.width + 1 - distance * 2);
        //방의 너비를 (최소방의 너비) ~ (space의 너비 - distance(방사이 거리))사이의 값으로 저장
        int height = Random.Range(minRoomHeight, space.height + 1 - distance * 2);
        //방의 높이를 (최소방의 높이) ~ (space의 높이 - distance(방사이의 거리))사이의 값으로 저장
        for (int i = space.Center().x - width / 2; i <= space.Center().x + width / 2; i++)//width의 길이만큼 반복
        {
            for(int j = space.Center().y - height / 2; j < space.Center().y + height / 2; j++)//height의 길이만큼 반복
            {
                positions.Add(new Vector2Int(i,j));//positions에 좌표를 넣음
                
            }
        }
        MakeIsInside(space, width, height);//방에 플레이어와 적이 같이있으면 문을 만드는 isinside를 만듬
        return positions;//positions을 반환함
    }
    public void MakeIsInside(RectangleSpace space, int width, int height)
    {
        isinside.transform.localScale = new Vector3(width - 0.3f, height-1.5f);//isinside의 크기를 받아온 너비와, 높이로 바꿈
        Instantiate(isinside, (Vector3Int)space.Center(), Quaternion.identity);//새로운isinside 만들기
        RectangleSpace roomspace = new RectangleSpace(new Vector2Int(space.Center().x  - width / 2, space.Center().y - height / 2), width, height);
        //Debug.DrawLine((Vector3Int)roomspace.leftDown, new Vector3(roomspace.leftDown.x + roomspace.width, roomspace.leftDown.y + roomspace.height), Color.red);
        roomSpace.Add(roomspace);
    }


    private void MakeCorridors()
    {
        List<Vector2Int> tempCenters = new List<Vector2Int>();//방의 중심들을 리스트로 만듬
        foreach(var space in divideSpace.spaceList)//리스트갯수만큼
        {
            tempCenters.Add(new Vector2Int(space.Center().x, space.Center().y)); //tempCenters에 중심좌표를 넣음
        }
        Vector2Int nextCenter;//다음으로올 방의 중심
        Vector2Int currentCenter = tempCenters[Random.Range(0,tempCenters.Count)];//복도를 만들기 시작할 곳을 랜덤으로 정함
        tempCenters.Remove(currentCenter);//시작방의 중심을 리스트에서 없앰
        while(tempCenters.Count != 0)//tempCenters에 아무것도 없을때까지 반복
        {
            nextCenter = ChooseShortestNextCorridor(tempCenters, currentCenter); //nextCenter에 가장 가까문 중심을 찾아 저장한다
            MakeOneCorridor(currentCenter, nextCenter);//복도를 만듬
            currentCenter = nextCenter;//currentCenter에 nextCenter를 넣음
            tempCenters.Remove(currentCenter);//새로 currentCenter에 넣은 방을 리스트에서 제거
        }
    }

    private Vector2Int ChooseShortestNextCorridor(List<Vector2Int> tempCenters, Vector2Int previousCenter)
    {
        int n = 0;
        float minLength = float.MaxValue;//가장 짧은것
        for(int i = 0; i < tempCenters.Count; i++)//받아온 tempCenters리스트의 갯수만큼 반복
        {
            if(Vector2.Distance(previousCenter, tempCenters[i]) < minLength)//minLength보다 지금 확인한 것이 더 짧으면
            {
                minLength = Vector2.Distance(previousCenter, tempCenters[i]);//minLength를 지금 확인한 것의 길이로 바꿈
                n = i;
            }
        }
        return tempCenters[n];//가장 가까운 방의 중심을 반환함
    }

    private void MakeOneCorridor(Vector2Int currentCenter, Vector2Int nextCenter)
    {
        Vector2Int current = new Vector2Int(currentCenter.x, currentCenter.y);
        Vector2Int next = new Vector2Int(nextCenter.x, nextCenter.y);
        floor.Add(current);
        while(current.x != next.x)//현재중심의 x값이 다음 중심의 x값과 같아질때까지 1칸씩 이동하면서 그 위치를 더해줌
        {
            if(current.x < next.x)
            {
                current.x += 1;
                floor.Add(current);
            }
            else
            {
                current.x -= 1;
                floor.Add(current);
            }
        }
        while(current.y != next.y)//현재중심의 y값이 다음 중심의 y값과 같아질때까지 1칸씩 이동하면서 그 위치를 더해줌
        {
            if(current.y < next.y)
            {
                current.y += 1;
                floor.Add(current);
            }
            else
            {
                current.y -= 1;
                floor.Add(current);
            }
        }
    }

    private void MakeWall()//floor의 모든 타일에 3x3의 공간을 만들고 그곳에 빈공간이 있으면 wall타일을 그린다
    {
        foreach(Vector2Int tile in floor)
        {
            HashSet<Vector2Int> boundary = Make3X3Square(tile);
            boundary.ExceptWith(floor);
            if(boundary.Count != 0)
            {
                wall.UnionWith(boundary);
            }
        }
    }

    private void MakeDoor()//floor의 모든 타일에 3x3의 공간을 만들고 그곳에 빈공간이 있으면 door타일을 그린다
    {
        foreach (Vector2Int tile in floor)
        {
            HashSet<Vector2Int> boundary = Make3X3Square(tile);
            boundary.ExceptWith(floor);
            if (boundary.Count != 0)
            {
                door.UnionWith(boundary);
            }
        }
    }

    private HashSet<Vector2Int> Make3X3Square(Vector2Int tile)//가상의 3x3타일공간을 그린다
    {
        HashSet<Vector2Int> boundary = new HashSet<Vector2Int>();
        for(int i = tile.x - 1; i <= tile.x + 1; i++)
        {
            for(int j = tile.y - 1; j <= tile.y +1; j++)
            {
                boundary.Add(new Vector2Int(i, j));
            }
        }
        return boundary;
    }


}
