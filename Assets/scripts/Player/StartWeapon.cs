using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWeapon : MonoBehaviour
{
    public GameObject[] prefabs; // ���� ������ ������ �迭
    private bool hasSpawned = false; // �� ���� �����ϱ� ���� �÷���

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
        // �迭���� �������� ������ ����
        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject selectedPrefab = prefabs[randomIndex];

        // ������ ����
        Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity);
    }
}
