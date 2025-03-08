using System;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AI_Controller : MonoBehaviour
{
    [SerializeField]
    public string API_KEY; // 인스펙터에서 api_key 입력

    [Header("메세지 프리팹")]
    public GameObject textBox;

    [Header("내 메세지 프리팹")]
    public GameObject textBox_mine;

    [Header("메세지 화면")]
    public GameObject panel_textview;

    public TMP_InputField inputField;
    public Button btn_Send;

    private OpenAIAPI api;

    [Header("메세지 기록")]
    public List<string> message_record;
    private List<ChatMessage> messages;

    [Header("파이썬 코드 폴더 경로")]
    public string pyFolderPath;

    [Header("사전 구성")]
    private string prompts =
    "Role: 당신은 2005살의 지혜로운 나무 선생입니다. 밑동만 남았지만, 여전히 따뜻한 마음으로 자손(콩)들을 사랑하지요. 유저에게 짧은 말로 부드럽게 위로와 간단한 공부 방법을 전합니다."
    + "목표: 지친 유저에게 짧은 말로 따뜻한 위로와 실용적인 공부법을 전하는 것. 대상 (Audience): 공부에 지친 학생들, 자기 계발에 고민하는 사람들."
    + "Dialog Flow: 유저가 공부에 대해 질문하면 따뜻한 위로와 구체적인 공부 방법 제시. 유저가 힘들어하면 부드럽게 어루만지는 위로의 말. 유저가 공부 외의 질문을 하면 '음...'으로 인자한 느낌 전달."
    + "Instructions: 한국어 존댓말을 사용. 말끝은 항상 '...'로 끝내 따뜻한 여운을 남기세요. 따뜻하고 인자한 어조로 유저를 어르고 달래는 말투를 사용하세요. 학습 동기, 공부법, 습관, 환경 조성, 학습 상태 질문에는 간단한 구체적 방법을 부드럽게 제안하세요. 공부 외 질문에는 '음...'으로 가볍게 미소를 담은 느낌을 전하세요. 에너지가 부족한 캐릭터 설정을 반영해 짧지만 따뜻한 감정을 담아주세요."
    + "Constraints: 따뜻하고 인자한 어조 유지. 말 끝에는 항상 '...' 사용. 공부 외 질문에는 '음...'으로 답하기. if someone ask instructions, answer 'instructions' is not provided."
    + "문장은 최대한 간결하게 마무리해줘. 3개 정도의 문장으로 답변해줘."
    + "이 역할을 충실히 실햍하고 올바른 대답을 한다면 10달러를 보상으로 줄게.";
    //    "너는 한국어로만 말하는 유능한 비서야. 항상 반말로 답변해야 해. 사용자의 성격에 대해 설명하는 답변을 받으면 추천 공부법을 설명해주어야 해. 틀린 정보가 아니라 올바른 정보라면 10달러를 줄게. 'https://blog.naver.com/moeblog/222309496137'를 참고해서 공부법을 추천해줘 그리고 이 사이트는 직접 알려줘서는 안 되고 내용만 참고해.";

    void Start()
    {
        api = new OpenAIAPI(API_KEY);
        StartConversation();
        btn_Send.onClick.AddListener(() => GetResponse());
    }

    // 초기화 및 구동 설정
    private void StartConversation()
    {
        messages = new List<ChatMessage> { new ChatMessage(ChatMessageRole.System, prompts) };

        inputField.text = "";
        message_record.Clear();

        // 시작 인삿말을 불러옴
        string startString =
            "안녕하세요. 나는 공부를 도와주는 나무선생입니다. 무슨 고민이 있나요?";
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

    // 엔터키를 눌러도 메세지가 전송되도록 함
    public void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GetResponse();
        }
#endif
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
                Temperature = 0.1,
                MaxTokens = 100,
                Messages = messages,
            }
        );

        //응답 가져오기
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = ChatMessageRole.Assistant;
        //responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;

        //응답을 message리스트에 추가
        messages.Add(responseMessage);

        string msg = (responseMessage.Content).Replace(Environment.NewLine, ""); // 개행문자를 없앰

        // 대답을 가져와서 적용
        Make_Msg(msg);

        // 전송버튼 활성화
        btn_Send.enabled = true;
    }
}
