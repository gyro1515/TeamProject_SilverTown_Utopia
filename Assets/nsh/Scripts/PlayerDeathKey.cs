using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//�÷��̾� �����(KŰ �Է½�) ī�޶� ��ġ����, DeadScene���� ��ȯ
public class PlayerDeathKey : MonoBehaviour
{
    public int playerHealth = 100;
    public static Vector3 lastCameraPosition; // DeadScene���� ������ ����

    void Update()
    {
        // �׽�Ʈ��: KŰ ������ ü�� 0
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerHealth = 0;
            CheckDeath();
        }
    }

    void CheckDeath()
    {
        if (playerHealth <= 0)
        {
            // ���� ī�޶� ��ġ ����
            lastCameraPosition = Camera.main.transform.position;

            // DeadScene���� ��ȯ
            SceneManager.LoadScene("DeadScene");
        }
    }
}
