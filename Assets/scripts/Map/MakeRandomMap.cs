using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class MakeRandomMap : MonoBehaviour
{
    [SerializeField]
    private int distance;//����� �Ÿ�
    [SerializeField]
    private int minRoomWidth;//�ּ� �� �ʺ�
    [SerializeField]
    private int minRoomHeight;//�ּ� �� ����
    [SerializeField]
    private DivideSpace divideSpace;//�������� ����
    [SerializeField]
    private SpreadTilemap spreadTilemap;//Ÿ�ϱ���
    [SerializeField]
    private GameObject player;//�÷��̾�
    [SerializeField]
    private GameObject entrance;//������ �Ѿ�� ��Ż
    [SerializeField]
    private GameObject box;//����

    public GameObject enemy;

    [SerializeField]
    private List<RectangleSpace> roomSpace;

    public GameObject isinside;

    private HashSet<Vector2Int> floor;//floorŸ��
    private HashSet<Vector2Int> wall;//wallŸ��
    private HashSet<Vector2Int> door;//doorŸ��



    private void Start()
    {
        StartRandomMap();
    }

    public void StartRandomMap()
    {
        spreadTilemap.ClearAllTiles();//����ִ� Ÿ�� ����
        divideSpace.totalspace = new RectangleSpace(new Vector2Int(0, 0), divideSpace.totalWidth, divideSpace.totalHeight);
        //divideSpace�� totalspace(�ڸ� �簢��)�� ����
        divideSpace.spaceList = new List<RectangleSpace>();//divideSpace�� spaceList(�߸� �簢���������� ����Ʈ)�� ����
        floor = new HashSet<Vector2Int>();
        wall = new HashSet<Vector2Int>();
        door = new HashSet<Vector2Int>();
        divideSpace.DivideRoom(divideSpace.totalspace);//�簢���� �ڸ�
        MakeRandomRooms();//��

        MakeDoor();//��
        spreadTilemap.SpreadDoorTilemap(door);//doorŸ�� ���

        MakeCorridors();//����

        MakeWall();//��

        spreadTilemap.SpreadFloorTilemap(floor);//floorŸ�� ���
        spreadTilemap.SpreadWallTilemap(wall);//wallŸ�� ���
        
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
        foreach(var space in divideSpace.spaceList)//spaceList�ȿ� �ִ� ��� space��ŭ
        {
            HashSet<Vector2Int> positions = MakeARandomRectangleRoom(space);//space�� ��ġ�� ����
            floor.UnionWith(positions);//UnionWith�� ��ġ�� floor�� �����Ѵ�
        }
    }

    private HashSet<Vector2Int> MakeARandomRectangleRoom(RectangleSpace space)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();//positions�� ����
        int width = Random.Range(minRoomWidth, space.width + 1 - distance * 2);
        //���� �ʺ� (�ּҹ��� �ʺ�) ~ (space�� �ʺ� - distance(����� �Ÿ�))������ ������ ����
        int height = Random.Range(minRoomHeight, space.height + 1 - distance * 2);
        //���� ���̸� (�ּҹ��� ����) ~ (space�� ���� - distance(������� �Ÿ�))������ ������ ����
        for (int i = space.Center().x - width / 2; i <= space.Center().x + width / 2; i++)//width�� ���̸�ŭ �ݺ�
        {
            for(int j = space.Center().y - height / 2; j < space.Center().y + height / 2; j++)//height�� ���̸�ŭ �ݺ�
            {
                positions.Add(new Vector2Int(i,j));//positions�� ��ǥ�� ����
                
            }
        }
        MakeIsInside(space, width, height);//�濡 �÷��̾�� ���� ���������� ���� ����� isinside�� ����
        return positions;//positions�� ��ȯ��
    }
    public void MakeIsInside(RectangleSpace space, int width, int height)
    {
        isinside.transform.localScale = new Vector3(width - 0.3f, height-1.5f);//isinside�� ũ�⸦ �޾ƿ� �ʺ��, ���̷� �ٲ�
        Instantiate(isinside, (Vector3Int)space.Center(), Quaternion.identity);//���ο�isinside �����
        RectangleSpace roomspace = new RectangleSpace(new Vector2Int(space.Center().x  - width / 2, space.Center().y - height / 2), width, height);
        //Debug.DrawLine((Vector3Int)roomspace.leftDown, new Vector3(roomspace.leftDown.x + roomspace.width, roomspace.leftDown.y + roomspace.height), Color.red);
        roomSpace.Add(roomspace);
    }


    private void MakeCorridors()
    {
        List<Vector2Int> tempCenters = new List<Vector2Int>();//���� �߽ɵ��� ����Ʈ�� ����
        foreach(var space in divideSpace.spaceList)//����Ʈ������ŭ
        {
            tempCenters.Add(new Vector2Int(space.Center().x, space.Center().y)); //tempCenters�� �߽���ǥ�� ����
        }
        Vector2Int nextCenter;//�������ο� ���� �߽�
        Vector2Int currentCenter = tempCenters[Random.Range(0,tempCenters.Count)];//������ ����� ������ ���� �������� ����
        tempCenters.Remove(currentCenter);//���۹��� �߽��� ����Ʈ���� ����
        while(tempCenters.Count != 0)//tempCenters�� �ƹ��͵� ���������� �ݺ�
        {
            nextCenter = ChooseShortestNextCorridor(tempCenters, currentCenter); //nextCenter�� ���� ��� �߽��� ã�� �����Ѵ�
            MakeOneCorridor(currentCenter, nextCenter);//������ ����
            currentCenter = nextCenter;//currentCenter�� nextCenter�� ����
            tempCenters.Remove(currentCenter);//���� currentCenter�� ���� ���� ����Ʈ���� ����
        }
    }

    private Vector2Int ChooseShortestNextCorridor(List<Vector2Int> tempCenters, Vector2Int previousCenter)
    {
        int n = 0;
        float minLength = float.MaxValue;//���� ª����
        for(int i = 0; i < tempCenters.Count; i++)//�޾ƿ� tempCenters����Ʈ�� ������ŭ �ݺ�
        {
            if(Vector2.Distance(previousCenter, tempCenters[i]) < minLength)//minLength���� ���� Ȯ���� ���� �� ª����
            {
                minLength = Vector2.Distance(previousCenter, tempCenters[i]);//minLength�� ���� Ȯ���� ���� ���̷� �ٲ�
                n = i;
            }
        }
        return tempCenters[n];//���� ����� ���� �߽��� ��ȯ��
    }

    private void MakeOneCorridor(Vector2Int currentCenter, Vector2Int nextCenter)
    {
        Vector2Int current = new Vector2Int(currentCenter.x, currentCenter.y);
        Vector2Int next = new Vector2Int(nextCenter.x, nextCenter.y);
        floor.Add(current);
        while(current.x != next.x)//�����߽��� x���� ���� �߽��� x���� ������������ 1ĭ�� �̵��ϸ鼭 �� ��ġ�� ������
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
        while(current.y != next.y)//�����߽��� y���� ���� �߽��� y���� ������������ 1ĭ�� �̵��ϸ鼭 �� ��ġ�� ������
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

    private void MakeWall()//floor�� ��� Ÿ�Ͽ� 3x3�� ������ ����� �װ��� ������� ������ wallŸ���� �׸���
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

    private void MakeDoor()//floor�� ��� Ÿ�Ͽ� 3x3�� ������ ����� �װ��� ������� ������ doorŸ���� �׸���
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

    private HashSet<Vector2Int> Make3X3Square(Vector2Int tile)//������ 3x3Ÿ�ϰ����� �׸���
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
