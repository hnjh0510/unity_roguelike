using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpreadTilemap : MonoBehaviour
{
    [SerializeField]
    private Tilemap floor;//floor타일 오브젝트
    [SerializeField]
    private Tilemap wall;//wall타일 오브젝트
    [SerializeField]
    private Tilemap door;//door타일 오브젝트
    [SerializeField]
    private TileBase floorTile;//floor타일 에셋
    [SerializeField]
    private TileBase wallTile;//wall타일 에셋
    [SerializeField]
    private TileBase doorTile;//door타일 에셋
    [SerializeField]


    public void SpreadFloorTilemap(HashSet<Vector2Int> positions)//받은 좌표에 floor타일 깔기
    {
        SpreadTile(positions, floor, floorTile);
    }

    public void SpreadWallTilemap(HashSet<Vector2Int> positions)//받은 좌표에 wall타일 깔기
    {
        SpreadTile(positions, wall, wallTile);
    }

    public void SpreadDoorTilemap(HashSet<Vector2Int> positions)//받은 좌표에 door타일 깔기
    {
        SpreadTile(positions, door, doorTile);
    }

    private void SpreadTile(HashSet<Vector2Int> positions, Tilemap tilemap, TileBase tile)//좌표, 타일 오브젝트, 타일에셋을 입력받아 타일 오브젝트에 타일 에셋을 그린다
    {
        foreach (var position in positions)
        {
            tilemap.SetTile((Vector3Int)position, tile);
        }
    }

    public void ClearAllTiles()//이미 있던 타일을 지운다
    {
        floor.ClearAllTiles();
        wall.ClearAllTiles();
        door.ClearAllTiles();
    }

}
