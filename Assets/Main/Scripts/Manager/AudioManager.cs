using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;    // 싱글턴 인스턴스
    
    // 인스펙터창에서 등록할 때 enum과 오디오클립을 매칭하기 위해서 구조체 생성
    [Header("Audio Struct Settings")]
    public List<BGMData> bgmDatas;         // BGM들의 구조체
    public List<SEData> seDatas;           // SE들의 구조체
    [System.Serializable] public struct BGMData
    {
        public BGMType type;
        public AudioClip clip;
    }
    [System.Serializable] public struct SEData
    {
        public SEType type;
        public AudioClip clip;
    }
    
    [Header("AudioSource Settings")]
    [SerializeField] private AudioSource bgmSource;
    
    // 여러 효과음의 동시재생을 위해 오디오 풀링 방식을 채용
    [SerializeField] private int sePoolSize = 10;
    private List<AudioSource> seSourcePool;
    private int seSourceIndex = 0;

    private Dictionary<BGMType, AudioClip> bgmDictionary;
    private Dictionary<SEType, AudioClip> seDictionary;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬전환 할 일은 없지만 혹시나 싶어서 넣어둠
            
            // 오디오 딕셔너리 초기화(오디오의 타입명과 오디오클립의 구조체를 이용해서 딕셔너리에 등록)
            bgmDictionary = new Dictionary<BGMType, AudioClip>();
            foreach (var bgm in bgmDatas)
            {
                bgmDictionary[bgm.type] = bgm.clip;
            }
            seDictionary = new Dictionary<SEType, AudioClip>();
            foreach (var se in seDatas)
            {
                seDictionary[se.type] = se.clip;
            }

            // 오디오 소스 풀 생성
            seSourcePool = new List<AudioSource>();
            for (int i = 0; i < sePoolSize; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;    // 오디오소스가 생성시 재생되는것을 방지
                
                // TODO 볼륨, 믹서 등 필요하면 세팅
                seSourcePool.Add(audioSource);
            }
            // 오디오 딕셔너리 초기화(에디터를 통해 등록한 오디오 크립을 딕셔너리에 등록 - 람다식 사용)
            // - 코드 리팩토리로 폐기
            //bgmDictionary = bgmClips.ToDictionary(x => x.name, x => x);
            //seDictionary = seClips.ToDictionary(x => x.name, x => x);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    // BGM을 재생 - 오디오 소스는 하나의 소스를 인스펙터창에 등록해서 사용
    public void PlayBGM(BGMType type)
    {
        if (bgmDictionary.TryGetValue(type, out AudioClip clip))
        {
            if (bgmSource.clip == clip) return;
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }
    // 효과음을 재생 - 오디오소스는 오브젝트 풀링방식으로 처리
    public void PlaySE(SEType type)
    {
        if (seDictionary.TryGetValue(type, out var clip))
        {
            var audioSource = FindAvailableSESource();
            if (audioSource != null)
                audioSource.PlayOneShot(clip);
        }
    }

    public void StopBGM() => bgmSource.Stop();
    // 오디오 소스가 들어있는 리스트에서 재생중이지 않은 오디오 소스를 반환한다
    private AudioSource FindAvailableSESource()
    {
        // 라운드 로빈 방식(순서대로 돌아가며 사용)
        for (int i = 0; i < sePoolSize; i++)
        {
            seSourceIndex = (seSourceIndex + 1) % sePoolSize;
            var audioSource = seSourcePool[seSourceIndex];
            if (!audioSource.isPlaying) return audioSource;
        }
        // 모두 재생중이면 첫 번째 소스를 강제 사용(최악의 경우)
        return seSourcePool[0];
    }
}
