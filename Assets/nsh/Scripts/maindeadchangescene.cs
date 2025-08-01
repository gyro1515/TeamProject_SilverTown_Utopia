using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerDeadChangeScene : MonoBehaviour
{
    public GameObject player;       // 플레이어 오브젝트
    public string deadSceneName = "DeadScene"; // 전환할 씬 이름

    private bool isDead = false;

    void Update()
    {
        if (!isDead && player != null)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>(); // 체력 스크립트
            if (hp != null && hp.currentHp <= 0)
            {
                isDead = true;
                StartCoroutine(GoToDeadScene());
            }
        }
    }

    IEnumerator GoToDeadScene()
    {
        // 카메라 위치 저장
        Vector3 camPos = Camera.main.transform.position;

        // 화면 전환 효과나 딜레이
        yield return new WaitForSeconds(1.5f);

        // DeadScene으로 이동
        SceneManager.LoadScene(deadSceneName);
    }
}
