using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpreadTilemap : MonoBehaviour
{
    [SerializeField]
    private Tilemap floor;//floorŸ�� ������Ʈ
    [SerializeField]
    private Tilemap wall;//wallŸ�� ������Ʈ
    [SerializeField]
    private Tilemap door;//doorŸ�� ������Ʈ
    [SerializeField]
    private TileBase floorTile;//floorŸ�� ����
    [SerializeField]
    private TileBase wallTile;//wallŸ�� ����
    [SerializeField]
    private TileBase doorTile;//doorŸ�� ����
    [SerializeField]


    public void SpreadFloorTilemap(HashSet<Vector2Int> positions)//���� ��ǥ�� floorŸ�� ���
    {
        SpreadTile(positions, floor, floorTile);
    }

    public void SpreadWallTilemap(HashSet<Vector2Int> positions)//���� ��ǥ�� wallŸ�� ���
    {
        SpreadTile(positions, wall, wallTile);
    }

    public void SpreadDoorTilemap(HashSet<Vector2Int> positions)//���� ��ǥ�� doorŸ�� ���
    {
        SpreadTile(positions, door, doorTile);
    }

    private void SpreadTile(HashSet<Vector2Int> positions, Tilemap tilemap, TileBase tile)//��ǥ, Ÿ�� ������Ʈ, Ÿ�Ͽ����� �Է¹޾� Ÿ�� ������Ʈ�� Ÿ�� ������ �׸���
    {
        foreach (var position in positions)
        {
            tilemap.SetTile((Vector3Int)position, tile);
        }
    }

    public void ClearAllTiles()//�̹� �ִ� Ÿ���� �����
    {
        floor.ClearAllTiles();
        wall.ClearAllTiles();
        door.ClearAllTiles();
    }

}
