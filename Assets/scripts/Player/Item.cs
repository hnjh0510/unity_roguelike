using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float floatAmplitude = 0.3f; // �������� ����
    public float floatFrequency = 1f; // �������� �ֱ�

    void Update()
    {
        FloatItem();
    }

    void FloatItem()
    {
        Vector3 tempPosition = transform.position;
        tempPosition.y += Mathf.Sin(Time.time * Mathf.PI * floatFrequency) * floatAmplitude * Time.deltaTime;
        transform.position = tempPosition;
    }
}
