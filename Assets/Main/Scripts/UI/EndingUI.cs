using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float playerSpeed = 3f;
    private float lastPositionY = 777f;

    private void Start()
    {
        fadeImage.GetComponent<FadeCamera>().FadeOut();
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayBGM(BGMType.Ending);
        //fadeImage.GetComponentInChildren<GameObject>(true).SetActive(true);
        fadeImage.transform.Find("Player").gameObject.SetActive(true);
    }
    void Update()
    {
        EndingScene();
    }

    public void EndingScene() 
    {
        if (transform.position.y < lastPositionY)
        {
            // 스탭롤 처리
            Vector2 position = transform.position;
            position.y += scrollSpeed * Time.deltaTime;
            transform.position = position;
        }
        else 
        {
            if (Input.GetMouseButton(0)) // 왼쪽 버튼 눌리고 있는 동안
            {
                // TODO 클릭했을때 스타트화면으로 넘어가기
                Debug.Log("스타트 화면으로 전이");
            }
        }
        // 플레이어 처리
        if (fadeImage.transform.Find("Player") != null) 
        {
            Vector2 playerPosition = fadeImage.transform.Find("Player").transform.position;
            playerPosition.y -= playerSpeed * Time.deltaTime;
            if (playerPosition.y < 200f && playerPosition.x > 400f)
            {
                playerPosition.x -= playerSpeed * Time.deltaTime * 1.5f;
            }
            //Debug.Log($"position : {playerPosition}");
            fadeImage.transform.Find("Player").transform.position = playerPosition;
            if (playerPosition.y < -100f)
            {
                Destroy(fadeImage.transform.Find("Player").gameObject);
            }
        }
    }

    public void EndingStart() 
    {
        this.enabled = true;
    } 
}
