using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointsController : MonoBehaviour
{
    [SerializeField] private LapController lapController;
    [SerializeField] private int index;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // LapController�� ���� üũ����Ʈ ������ �����϶�� �˸��ϴ�.
            lapController.UpdateCheckPoint(index, other.transform.position, other.transform.rotation);
        }
    }
}