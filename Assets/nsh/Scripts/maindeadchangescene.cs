using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerDeadChangeScene : MonoBehaviour
{
    public GameObject player;       // �÷��̾� ������Ʈ
    public string deadSceneName = "DeadScene"; // ��ȯ�� �� �̸�

    private bool isDead = false;

    void Update()
    {
        if (!isDead && player != null)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>(); // ü�� ��ũ��Ʈ
            if (hp != null && hp.currentHp <= 0)
            {
                isDead = true;
                StartCoroutine(GoToDeadScene());
            }
        }
    }

    IEnumerator GoToDeadScene()
    {
        // ī�޶� ��ġ ����
        Vector3 camPos = Camera.main.transform.position;

        // ȭ�� ��ȯ ȿ���� ������
        yield return new WaitForSeconds(1.5f);

        // DeadScene���� �̵�
        SceneManager.LoadScene(deadSceneName);
    }
}
