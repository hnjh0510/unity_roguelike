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

    public List<GameObject> enemy;//�� ����Ʈ

    [SerializeField]
    private List<RectangleSpace> roomSpace;//isinside���� ����Ʈ
    [SerializeField]
    private List<RectangleSpace> _roomSpace;//�������Ҷ� ����� �� ����Ʈ
    [SerializeField]
    private List<int> roomCheck;
    private int roomcheck = 0;

    public GameObject isinside;//�濡 �÷��̾�� ���� �ִ��� �����Ͽ� ���� �����ϴ� collider

    private HashSet<Vector2Int> floor;//floorŸ��
    private HashSet<Vector2Int> wall;//wallŸ��
    private HashSet<Vector2Int> door;//doorŸ��

    public List<int> RoomSize;//��ũ�� ������ ����Ʈ
    private int beforeCount;//���� �ٴڰ���
    private int floorCount;//���� �ٴڰ���

    public GameObject boss;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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

        int pRand = Random.Range(0, roomSpace.Count);//�÷��̾� ��ġ ����
        player.transform.position = (Vector2)roomSpace[pRand].Center();//��ġ�� �ű�
        //�⺻���� �߰�
        //RoomSize[pRand] = 0;//�÷��̾ �ִ¹��� �ٴ�Ÿ�ϰ����� 0���� ����(���� �������� �ʰ��ϱ� ����)
        RoomSize.RemoveAt(pRand);
        roomSpace.RemoveAt(pRand);//roomSpace���� �÷��̾�ִ¹��� ��
        roomCheck.RemoveAt(pRand);

        if (boss == null)
        {
            int exitRand = Random.Range(0, roomSpace.Count);//�������� �Ѿ�� ��Ż ��ġ ����
            entrance.transform.position = (Vector2)roomSpace[exitRand].Center();////��ġ �ű�
                                                                                //RoomSize[exitRand] = 0;//��Ż�� �ִ� ���� �ٴڰ��� 0���θ���
            RoomSize.RemoveAt(exitRand);
            roomSpace.RemoveAt(exitRand);//����Ʈ���� ����
            roomCheck.RemoveAt(exitRand);
        }
        else
        {
            int exitRand = Random.Range(0, roomSpace.Count);//�������� �Ѿ�� ��Ż ��ġ ����
            boss.transform.position = (Vector2)roomSpace[exitRand].Center();////��ġ �ű�
                                                                                //RoomSize[exitRand] = 0;//��Ż�� �ִ� ���� �ٴڰ��� 0���θ���
            RoomSize.RemoveAt(exitRand);
            roomSpace.RemoveAt(exitRand);//����Ʈ���� ����
            roomCheck.RemoveAt(exitRand);
            Destroy(entrance);
        }
        int boxRand = Random.Range(0, roomSpace.Count);//���� ��ġ ����
        box.transform.position = (Vector2)roomSpace[boxRand].Center();//��ġ�ű�
        //RoomSize[boxRand] = 0;//���ڰ� �ִ� �� Ÿ�ϰ��� 0���θ���
        RoomSize.RemoveAt(boxRand);
        roomSpace.RemoveAt(boxRand);//����Ʈ���� �����
        roomCheck.RemoveAt(boxRand);

        int count = roomCheck.Count;
        for (int i = 0; i < count; i++)
        {
            if (RoomSize[i] < 30)
            {
                for (int j = 0; j < 1; j++)
                {
                    int RandEnemy = Random.Range(0, enemy.Count);
                    float xrand = Random.Range(_roomSpace[roomCheck[i]].Center().x - _roomSpace[roomCheck[i]].width / 2 + 2, _roomSpace[roomCheck[i]].Center().x + _roomSpace[roomCheck[i]].width / 2);
                    float yrand = Random.Range(_roomSpace[roomCheck[i]].Center().y - _roomSpace[roomCheck[i]].height / 2 + 2, _roomSpace[roomCheck[i]].Center().y + _roomSpace[roomCheck[i]].height / 2);
                    Instantiate(enemy[RandEnemy], new Vector3(xrand, yrand), Quaternion.identity);
                }
            }
            if (RoomSize[i] < 50)
            {
                for (int j = 0; j < 2; j++)
                {
                    int RandEnemy = Random.Range(0, enemy.Count);
                    float xrand = Random.Range(_roomSpace[roomCheck[i]].Center().x - _roomSpace[roomCheck[i]].width / 2 + 2, _roomSpace[roomCheck[i]].Center().x + _roomSpace[roomCheck[i]].width / 2);
                    float yrand = Random.Range(_roomSpace[roomCheck[i]].Center().y - _roomSpace[roomCheck[i]].height / 2 + 2, _roomSpace[roomCheck[i]].Center().y + _roomSpace[roomCheck[i]].height / 2);
                    Instantiate(enemy[RandEnemy], new Vector3(xrand, yrand), Quaternion.identity);
                }
            }
            else if (RoomSize[i] < 70)
            {
                for (int j = 0; j < 3; j++)
                {
                    int RandEnemy = Random.Range(0, enemy.Count);
                    float xrand = Random.Range(_roomSpace[roomCheck[i]].Center().x - _roomSpace[roomCheck[i]].width / 2 + 2, _roomSpace[roomCheck[i]].Center().x + _roomSpace[roomCheck[i]].width / 2);
                    float yrand = Random.Range(_roomSpace[roomCheck[i]].Center().y - _roomSpace[roomCheck[i]].height / 2 + 2, _roomSpace[roomCheck[i]].Center().y + _roomSpace[roomCheck[i]].height / 2);
                    Instantiate(enemy[RandEnemy], new Vector3(xrand, yrand), Quaternion.identity);
                }
            }
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    int RandEnemy = Random.Range(0, enemy.Count);
                    float xrand = Random.Range(_roomSpace[roomCheck[i]].Center().x - _roomSpace[roomCheck[i]].width / 2 + 2, _roomSpace[roomCheck[i]].Center().x + _roomSpace[roomCheck[i]].width / 2);
                    float yrand = Random.Range(_roomSpace[roomCheck[i]].Center().y - _roomSpace[roomCheck[i]].height / 2 + 2, _roomSpace[roomCheck[i]].Center().y + _roomSpace[roomCheck[i]].height / 2);
                    Instantiate(enemy[RandEnemy], new Vector3(xrand, yrand), Quaternion.identity);
                }
            }
        }

    }

    private void MakeRandomRooms()
    {
        foreach (var space in divideSpace.spaceList)//spaceList�ȿ� �ִ� ��� space��ŭ
        {
            HashSet<Vector2Int> positions = MakeARandomRectangleRoom(space);//space�� ��ġ�� ����
            floor.UnionWith(positions);//UnionWith�� ��ġ�� floor�� �����Ѵ�

            CheckRoomSize();
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
            for (int j = space.Center().y - height / 2; j < space.Center().y + height / 2; j++)//height�� ���̸�ŭ �ݺ�
            {
                positions.Add(new Vector2Int(i, j));//positions�� ��ǥ�� ����

            }
        }
        MakeIsInside(space, width, height);//�濡 �÷��̾�� ���� ���������� ���� ����� isinside�� ����
        return positions;//positions�� ��ȯ��
    }
    public void MakeIsInside(RectangleSpace space, int width, int height)
    {
        isinside.transform.localScale = new Vector3(width - 0.3f, height - 1.5f);//isinside�� ũ�⸦ �޾ƿ� �ʺ��, ���̷� �ٲ�
        Instantiate(isinside, (Vector3Int)space.Center(), Quaternion.identity);//���ο�isinside �����
        RectangleSpace roomspace = new RectangleSpace(new Vector2Int(space.Center().x - width / 2, space.Center().y - height / 2), width, height);
        //Debug.DrawLine((Vector3Int)roomspace.leftDown, new Vector3(roomspace.leftDown.x + roomspace.width, roomspace.leftDown.y + roomspace.height), Color.red);
        roomSpace.Add(roomspace);
        _roomSpace.Add(roomspace);
        roomCheck.Add(roomcheck);
        roomcheck++;
    }

    private void CheckRoomSize()//���� �ٴ�Ÿ�� ���� üũ
    {
        foreach (Vector2Int tile in floor)//�ٴڰ�����ŭ
        {
            floorCount++;//1������
        }
        if (beforeCount == 0)//ù��°
        {
            RoomSize.Add(floorCount);//Ÿ�ϰ����� ����Ʈ�� �ø�
            beforeCount = floorCount;//beforeCount�� ������ü�ٴ�Ÿ�ϰ��� ����
            floorCount = 0;//0���� �ʱ�ȭ
        }
        else//ù��° �ƴ�
        {
            RoomSize.Add(floorCount - beforeCount);//���ݻ� Ÿ�ϰ��� - ��Ÿ�ϰ����� ����Ʈ�� �ø�
            beforeCount = floorCount;//beforeCount�� ������ü�ٴ�Ÿ�ϰ��� ����
            floorCount = 0;//0���� �ʱ�ȭ
        }
    }

    private void MakeCorridors()
    {
        List<Vector2Int> tempCenters = new List<Vector2Int>();//���� �߽ɵ��� ����Ʈ�� ����
        foreach (var space in divideSpace.spaceList)//����Ʈ������ŭ
        {
            tempCenters.Add(new Vector2Int(space.Center().x, space.Center().y)); //tempCenters�� �߽���ǥ�� ����
        }
        Vector2Int nextCenter;//�������ο� ���� �߽�
        Vector2Int currentCenter = tempCenters[Random.Range(0, tempCenters.Count)];//������ ����� ������ ���� �������� ����
        tempCenters.Remove(currentCenter);//���۹��� �߽��� ����Ʈ���� ����
        while (tempCenters.Count != 0)//tempCenters�� �ƹ��͵� ���������� �ݺ�
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
        for (int i = 0; i < tempCenters.Count; i++)//�޾ƿ� tempCenters����Ʈ�� ������ŭ �ݺ�
        {
            if (Vector2.Distance(previousCenter, tempCenters[i]) < minLength)//minLength���� ���� Ȯ���� ���� �� ª����
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
        while (current.x != next.x)//�����߽��� x���� ���� �߽��� x���� ������������ 1ĭ�� �̵��ϸ鼭 �� ��ġ�� ������
        {
            if (current.x < next.x)
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
        while (current.y != next.y)//�����߽��� y���� ���� �߽��� y���� ������������ 1ĭ�� �̵��ϸ鼭 �� ��ġ�� ������
        {
            if (current.y < next.y)
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
        foreach (Vector2Int tile in floor)
        {
            HashSet<Vector2Int> boundary = Make3X3Square(tile);
            boundary.ExceptWith(floor);
            if (boundary.Count != 0)
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
        for (int i = tile.x - 1; i <= tile.x + 1; i++)
        {
            for (int j = tile.y - 1; j <= tile.y + 1; j++)
            {
                boundary.Add(new Vector2Int(i, j));
            }
        }
        return boundary;
    }


}
