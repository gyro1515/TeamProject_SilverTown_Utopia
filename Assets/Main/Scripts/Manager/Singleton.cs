using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 싱글턴 매니저로 만들 클래스들은 해당 클래스 상속하여 사용하기
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance { get { return instance; } private set { instance = value; } }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else if (Instance != this)
        {
            // 현재는 씬 하나라 중복 생성될 일은 없겠지만, 혹시 모르니까
            Debug.LogWarning($"중복된 {typeof(T).Name} 싱글톤이 발견되어 파괴됩니다.");
            Destroy(gameObject); // 중복 제거
        }
    }
}
