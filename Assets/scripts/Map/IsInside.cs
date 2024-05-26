using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInside : MonoBehaviour/////////////////////////////정중환
{
    public Collider2D roomCollider;  // 방의 콜라이더를 할당
    public int playersInRoom = 0;   // 방에 있는 플레이어 수
    public int enemiesInRoom = 0;   // 방에 있는 적 수
    public GameObject player;
    public GameObject enemy;
    public GameObject door;
    
    void Start()
    {
        transform.Translate(new Vector3(0.5f, 0, 0));
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        door = GameObject.FindGameObjectWithTag("Door");

        if (door != null)
        {
            door.SetActive(false);
        }
    }
    void Update()
    {
        door = GameObject.FindGameObjectWithTag("Door");

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRoom++;
        }
        else if (other.CompareTag("Enemy"))
        {
            enemiesInRoom++;
        }

        UpdateColliderState();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRoom--;
        }
        else if (other.CompareTag("Enemy"))
        {
            enemiesInRoom--;
        }

        UpdateColliderState();
    }

    void UpdateColliderState()
    {
        if ((playersInRoom > 0 && enemiesInRoom > 0))
        {
            GameObject.Find("GameObject").transform.Find("door").gameObject.SetActive(true);
            Debug.Log("close");
        }
        else
        {
            GameObject.Find("GameObject").transform.Find("door").gameObject.SetActive(false);
            Debug.Log("open");
        }
    }
}
