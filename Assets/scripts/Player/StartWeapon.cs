using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWeapon : MonoBehaviour
{
    public GameObject[] prefabs; // 선택 가능한 프리펩 배열
    private bool hasSpawned = false; // 한 번만 생성하기 위한 플래그

    void Start()
    {
        if (!hasSpawned)
        {
            SpawnPrefab();
            hasSpawned = true;
        }
    }

    void SpawnPrefab()
    {
        // 배열에서 랜덤으로 프리펩 선택
        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject selectedPrefab = prefabs[randomIndex];

        // 프리펩 생성
        Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity);
    }
}
