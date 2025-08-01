using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//플레이어 사망시(K키 입력시) 카메라 위치저장, DeadScene으로 전환
public class PlayerDeathKey : MonoBehaviour
{
    public int playerHealth = 100;
    public static Vector3 lastCameraPosition; // DeadScene에서 참조할 변수

    void Update()
    {
        // 테스트용: K키 누르면 체력 0
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
            // 현재 카메라 위치 저장
            lastCameraPosition = Camera.main.transform.position;

            // DeadScene으로 전환
            SceneManager.LoadScene("DeadScene");
        }
    }
}
