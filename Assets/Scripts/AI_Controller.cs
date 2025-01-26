using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AI_Controller : MonoBehaviour
{
    [Header("메세지 프리팹")] public GameObject textBox;
    [Header("내 메세지 프리팹")] public GameObject textBox_mine;
    [Header("메세지 화면")] public GameObject panel_textview;

    public TMP_InputField inputField;
    public Button btn_Send;

    private OpenAIAPI api;

    [Header("메세지 기록")] 
    public List<string> message_record;
    private List<ChatMessage> messages;

    void Start()
    {
        api = new OpenAIAPI(
            "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"
        );
        StartConversation();
        btn_Send.onClick.AddListener(() => GetResponse());
    }

    // 초기화 및 구동 설정
    private void StartConversation()
    {
        messages = new List<ChatMessage> { 
            new ChatMessage(ChatMessageRole.System, "너는 한국어로만 말하는 인공지능 비서야. 항상 반말로 답변해야 해. 사용자의 성격에 대해 설명하는 답변을 받으면 추천 공부법을 설명해주어야 해. 성격과 관련없는 질문을 받으면, '너의 성격을 말해주면 추천 공부법을 알려줄게!' 라고 말해야 해. 항상 15문장 이내로 말해야 해,") };

        inputField.text = "";
        message_record.Clear();

        string startString = "안녕! 나는 인공지능 챗봇 Chaty야. 너의 성격을 알려 주면 맞는 공부법을 추천해줄게!";
        Make_Msg(startString);
    }

    // 메세지박스를 뷰에 만들어주는 메서드
    public void Make_Msg(string msg, bool isPlayer = false)
    {
        GameObject new_textbox;
        if (!isPlayer)
        {
            new_textbox = Instantiate(textBox, panel_textview.transform);
        }
        else 
        { 
            new_textbox = Instantiate(textBox_mine, panel_textview.transform);
        }

        new_textbox.GetComponent<Fitting_Box>().setText(msg);

        message_record.Add(msg);
    }

    // 입력을 전송하고 대답을 받는 비동기 메서드
    private async void GetResponse()
    {
        if (inputField.text.Length < 10)
        {
            return;
        }
        // 버튼 Disable
        btn_Send.enabled = false;

        // 입력한 메세지를 가져옴
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;
        //list에 메세지 추가
        messages.Add(userMessage);

        // 유저 메세지 전송
        Make_Msg(userMessage.Content, true);

        //inputField 초기화
        inputField.text = "";

        // 전체 채팅을 openAI 서버에전송하여 다음 메시지(응답)를 가져오도록
        var chatResult = await api.Chat.CreateChatCompletionAsync(
            new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Temperature = 0.2,
                MaxTokens = 1000,
                Messages = messages,
            }
        );

        //응답 가져오기
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        //응답을 message리스트에 추가
        messages.Add(responseMessage);

        // 대답을 가져와서 적용
        Make_Msg(responseMessage.Content);

        // 전송버튼 활성화
        btn_Send.enabled = true;

    }
}
