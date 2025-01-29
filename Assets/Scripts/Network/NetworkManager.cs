using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// API 호출 메서드를 보유하는 클래스이다.
/// 싱글톤이므로 NetworkManager.Instance.___ 형식으로 사용한다.
/// </summary>

public class CommonReq
{
}

public class CommonResponse
{
    public string message;
    public int code;
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get {
            if(!instance)
            {
                instance = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;

                if (instance == null)
                    Debug.Log("no Singleton obj");
            }
            return instance;
        }
    }

    public string WWW_URL;
    public float WWW_REQ_TIMEOUT;
    public bool IsOnNetwork;


    [Header("네트워크 종류 변수")]
    [SerializeField] public string API_KEY;

    public void Init()
    {
        WWW_URL = "";
    }

    public void Start()
    {
        Debug.Log(Application.internetReachability);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("인터넷 연결 없음");
            IsOnNetwork = false;
        }
        else
        {
            IsOnNetwork = true;
        }
    }

    public void Update()
    {
        // 인터넷 연결 끊기면 바로 에러 띄우도록
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("인터넷 연결 없음");
            IsOnNetwork = false;
        }
        else
        {
            IsOnNetwork = true;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        UnityWebRequest.ClearCookieCache();

        Init();
    }

    // 파이어베이스 유저 등록 및 발급
    public void Make_FireBaseUser()
    {
        /*
        MakeFirebaseUserP send = gameObject.AddComponent<MakeFirebaseUserP>();
        send.m_netWorkManager = this;
        send.m_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + api_key;
        send.m_RequestTimeout = WWW_REQ_TIMEOUT;
        
        send.SendRequest();
        */
    }

}
