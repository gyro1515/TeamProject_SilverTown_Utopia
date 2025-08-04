using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float scrollSpeed = 50f;
    private float lastPositionY = 777f;

    private void Start()
    {
        fadeImage.GetComponent<FadeCamera>().FadeOut();
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayBGM(BGMType.Ending);
    }
    void Update()
    {
        EndingScene();
    }

    public void EndingScene() 
    {
        if (transform.position.y < lastPositionY)
        {
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
    }

    public void EndingStart() 
    {
        this.enabled = true;
    } 
}
