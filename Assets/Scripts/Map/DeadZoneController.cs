using PG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZoneController : MonoBehaviour
{
    [SerializeField] private PlayerScript script;
    [SerializeField] private LapController lapController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            script.transform.position = new Vector3(lapController.respawnPointPosition.x, lapController.respawnPointPosition.y + 2.0f, lapController.respawnPointPosition.z);
            script.transform.rotation = lapController.respawnPointRotation;

            // ������ ����
            script.rb.velocity = Vector3.zero;
            script.rb.angularVelocity = Vector3.zero;

            // �浹 ���� �ڷ�ƾ�� ����
            StartCoroutine(script.PreventCollision());
        }
    }
}
