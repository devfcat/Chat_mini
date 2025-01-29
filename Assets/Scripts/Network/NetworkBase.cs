using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class BasePacket
{
    public bool isSuccess;
    public int errorCode;
    public string sessionKey;
}

public enum eSendType // 기본 전송 타입 GET
{
    GET = 0,
    POST,
    PATCH,
    PUT,
    DELETE,
    MULTI_PART_POST,
}

public abstract class NetworkBase : MonoBehaviour
{
    public string m_URL;

    public float m_RequestTimeout;
    public float m_RequestDuration;

    public object m_param;

    public eSendType m_POST;

    protected UnityWebRequest m_WWW;

    void Update()
    {
        if (m_RequestTimeout > 0.0f)
        {
            if (m_WWW != null)
            {
                CheckRequestTimeout();
            }
        }
    }

    public void SendRequest()
    {
        StartCoroutine(StartRequest());
    }

    public IEnumerator SendRequestCall()
    {
        yield return StartCoroutine(StartRequest());
    }

    IEnumerator StartRequest()
    {
        string API_KEY = NetworkManager.Instance.API_KEY;

        m_RequestDuration = 0.0f;

        string data = MakeSendingPacket(m_param);

        if (data == null)
        {
            data = "";
        }

        Debug.Log("▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼");
        Debug.Log("Send Data : " + data);

        if (m_POST == eSendType.POST)
        {
            m_WWW = new UnityWebRequest(m_URL, "POST");
            byte[] jsonToSend = new UTF8Encoding().GetBytes(data);
            m_WWW.uploadHandler = new UploadHandlerRaw(jsonToSend);
            m_WWW.downloadHandler = new DownloadHandlerBuffer();
            m_WWW.SetRequestHeader("Content-Type", "application/json");
            m_WWW.SetRequestHeader("Authorization", "Bearer " + API_KEY);
        }
        else if (m_POST == eSendType.MULTI_PART_POST)
        {
            m_WWW = new UnityWebRequest(m_URL, "POST");

            List<IMultipartFormSection> formData = MakeSendMultiPart();
            byte[] boundary = UnityWebRequest.GenerateBoundary();
            byte[] formSections = UnityWebRequest.SerializeFormSections(formData, boundary);
            m_WWW.uploadHandler = new UploadHandlerRaw(formSections);
            m_WWW.downloadHandler = new DownloadHandlerBuffer();
            m_WWW.SetRequestHeader("Content-Type", "multipart/form-data");
            m_WWW.SetRequestHeader("Authorization", "Bearer " + API_KEY);
        }
        else if (m_POST == eSendType.PATCH)
        {
            byte[] jsonToSend = null;
            if (!string.IsNullOrEmpty(data))
            {
                jsonToSend = new UTF8Encoding().GetBytes(data);
            }
            m_WWW = UnityWebRequest.Put(m_URL, jsonToSend);
            m_WWW.method = "PATCH";
            m_WWW.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            m_WWW.SetRequestHeader("Content-Type", "application/json");
            m_WWW.SetRequestHeader("Authorization", "Bearer " + API_KEY);

        }
        else if (m_POST == eSendType.PUT)
        {
            m_WWW = new UnityWebRequest(m_URL, "PUT");
            byte[] jsonToSend = new UTF8Encoding().GetBytes(data);
            m_WWW.uploadHandler = new UploadHandlerRaw(jsonToSend);
            m_WWW.downloadHandler = new DownloadHandlerBuffer();
            m_WWW.SetRequestHeader("Authorization", "Bearer " + API_KEY);
            m_WWW.SetRequestHeader("Content-Type", "application/json");
        }
        else if (m_POST == eSendType.DELETE)
        {
            string strURL = string.Format("{0}", m_URL);
            m_WWW = UnityWebRequest.Delete(strURL);

            m_WWW.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            m_WWW.SetRequestHeader("Content-Type", "application/json");
            m_WWW.SetRequestHeader("Authorization", "Bearer " + API_KEY);
        }
        else // GET일 경우
        {
            string strURL = string.Format("{0}", m_URL);
            m_WWW = UnityWebRequest.Get(strURL);

            m_WWW.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            m_WWW.SetRequestHeader("Content-Type", "application/json");
            m_WWW.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
            m_WWW.SetRequestHeader("Authorization", "Bearer " + API_KEY);
        }
        Debug.Log("===> Send Packet : " + m_WWW.url);

        yield return m_WWW.SendWebRequest();

        while (!m_WWW.isDone)
        {
            Debug.Log("Connecting");
            yield return m_WWW;
        }

        if (m_WWW.isDone && m_WWW.error == null)
        {
            Debug.Log(m_WWW.url);
            Debug.Log(m_WWW.downloadHandler.text);
            Debug.Log("▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲");
            ProcessResponse(m_WWW);
        }
        else
        {
            if (m_WWW.error != null)
            {
                // 공통 에러 처리
                Debug.LogError("===> Error : " + m_WWW.error + " DisConnect");
            }
            Debug.LogError("DisConnect");
            // 각 프로토콜에 대한 에러 처리
            ProcessWWWError(m_WWW);
        }
        m_WWW.uploadHandler?.Dispose();
        GameObject.Destroy(this);
    }

    private void CheckRequestTimeout() // 설정한 시간 내로 데이터 전송이 안 끝나면 종료시킴
    {
        if (m_WWW.downloadProgress > 0.0f)
        {
            Debug.Log("m_WWW.progress : " + m_WWW.downloadProgress);
        }

        m_RequestDuration += Time.deltaTime;

        if (m_RequestDuration > m_RequestTimeout)
        {
            Debug.Log("===> TimeOut DisConnect <===");
            GameObject.Destroy(this);
        }
    }

    protected virtual List<IMultipartFormSection> MakeSendMultiPart()
    {
        List<IMultipartFormSection> _formData = new List<IMultipartFormSection>();

        return _formData;
    }

    protected abstract string MakeSendingPacket(object param);

    protected abstract void ProcessResponse(UnityWebRequest www);

    protected abstract void ProcessWWWError(UnityWebRequest www);
}
