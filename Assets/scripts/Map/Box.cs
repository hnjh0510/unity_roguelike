using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public GameObject player;
    public bool isOpen = false;
    public List<GameObject> item;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == player && isOpen == false)
        {
            //���� ���
            Debug.Log("���� ���");
            int itemrand = Random.Range(0, item.Count);
            item[itemrand].transform.position = transform.position;
            Instantiate(item[itemrand]);
            isOpen = true;
        }
    }
}
