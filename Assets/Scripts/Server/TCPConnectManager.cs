using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TCPConnectManager : MonoBehaviour
{
    public static TCPConnectManager Instance = null;

    [Header("Chat")]
    [SerializeField] private TMP_Text MessageElement; // ä�� �޼���
    [SerializeField] private GameObject LobbyChattingList; // �κ� ä�� ����Ʈ
    [SerializeField] private TMP_InputField LobbyChat; // �κ� �Է� �޼���
    [SerializeField] private Button LobbyChatSendBtn; // ä�� �޼��� ���� ��ư

    [Header("Connect")]
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private StreamWriter writer;
    private User loginUserInfo;

    // ȣ��Ʈ
    private string hostname = "k10c209.p.ssafy.io"; // ���� ȣ��Ʈ
    private int port = 1370;

    private void Awake()
    {
        // �̱���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            // ���� ������ instance�� ����
            Instance.MessageElement = MessageElement;
            Instance.LobbyChattingList = LobbyChattingList;
            Instance.LobbyChat = LobbyChat;
            Instance.LobbyChatSendBtn = LobbyChatSendBtn;
        }


        // ������ ���̱�
        Instance.LobbyChatSendBtn.onClick.RemoveAllListeners();
        Instance.LobbyChatSendBtn.onClick.AddListener(() => Instance.MessageSendBtnClicked(LobbyChat));
    }

    void Start()
    {
        LobbyChat.characterLimit = 20;
    }

    void Update()
    {
        // �����Ͱ� ���� ���
        while (_networkStream != null && _networkStream.DataAvailable)
        {
            string response = ReadMessageFromServer();
            //DispatchResponse(response);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            MessageSendBtnClicked(LobbyChat);
        }
    }

    //#region ��û �й��ϱ�
    //private void DispatchResponse(string response)
    //{
    //    string type = getType(response);

    //    if (type == "0101")
    //    {
    //        showMessage(response, LobbyChattingList);
    //    }
    //}

    //private string getType(string response)
    //{
    //    string[] words = response.Split('\"');
    //    return words[3];
    //}
    //#endregion

    #region ���� ����
    public void ConnectToServer()
    {
        string message = "Hello Server";


        try
        {
            // TCP ������ ����
            _tcpClient = new TcpClient(hostname, port);
            _networkStream = _tcpClient.GetStream();
            writer = new StreamWriter(_networkStream);
            loginUserInfo = DataManager.Instance.loginUserInfo;

            string json = "{" +
                    $"\"userName\":\"{loginUserInfo.dataBody.nickname}\"," +
                    $"\"message\": \"{message}\"" +
                "}";

            SendMessageToServer(json);

            Debug.Log("ConnectToServer");
        }
        catch (Exception e)
        {
            // ���� �� ���� �߻� ��
            Debug.Log($"Failed to connect to the server: {e.Message}");
        }
    }

    // ������ �޼��� ������
    private void SendMessageToServer(string message)
    {
        if (_tcpClient == null) return;

        writer.WriteLine(message);
        writer.Flush(); // �޼��� �ﰢ ����
    }

    // �������� �޼��� �б�
    private string ReadMessageFromServer()
    {
        if (_tcpClient == null) return null;
        try
        {
            StringBuilder message = new StringBuilder();
            BinaryReader reader = new BinaryReader(_networkStream, Encoding.UTF8);
            
            while (_networkStream.DataAvailable)
            {
                char readChar = reader.ReadChar();
                if (readChar == '\n') break; // '\n' �� �����ڷ� ���
                
                message.Append(readChar);
            }
            return message.ToString();
        }
        catch(Exception e)
        {
            Debug.Log("���� �б� ���� : " + e.Message);
            return null;
        }
    }
    #endregion



    #region ä�� ����
    // �޼��� ���� ��ư Ŭ�� ��
    public void MessageSendBtnClicked(TMP_InputField inputField)
    {
        Debug.Log("�޽��� ����");

        string message = inputField.text;

        if (message == "")
        {
            return;
        }



        string json = "{" +
                $"\"userName\":\"{loginUserInfo.dataBody.nickname}\"," +
                $"\"message\": \"{message}\"" +
            "}";
        Debug.Log(json);
        inputField.text = "";
        SendMessageToServer(json);
        inputField.Select();
        inputField.ActivateInputField();
        showMessage(json, LobbyChattingList);
    }

    // �޼��� ������ ��
    public void showMessage(string message, GameObject ChatScrollView)
    {
        // ���� �θ� ������Ʈ
        Transform content = ChatScrollView.transform.Find("Viewport/Content");

        ChatMessage chatMessage = JsonUtility.FromJson<ChatMessage>(message);

        TMP_Text temp1 = Instantiate(MessageElement);

        temp1.text = $"{chatMessage.UserName} : {chatMessage.Message}";
        temp1.transform.SetParent(content, false);


        // 20�� �Ѿ�� ä�� ���������� �����
        if (content.childCount >= 20)
        {
            Destroy(content.GetChild(1).gameObject);
        }

        StartCoroutine(ScrollToBottom(ChatScrollView));
    }

    // ��ũ�� �� �Ʒ��� ������
    IEnumerator ScrollToBottom(GameObject ChatScrollView)
    {
        // ���� ������ ��ٸ�
        yield return null;

        Transform content = ChatScrollView.transform.Find("Viewport/Content");

        // LayOut Gropu�� ������ ��� ������Ʈ
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)content);

        // ��ũ�� �� �Ʒ��� ����
        ChatScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    #endregion

    #region ���� ����

    // ���� ��
    public void OnApplicationQuit()
    {
        clearChat();

        DataManager.Instance.gameDataClear();

        // tcp ���� ����
        if (_tcpClient != null)
        {
            DisconnectFromServer();
        }
        writer = null;
        loginUserInfo = null;
    }

    public void DisconnectFromServer()
    {
        // ���� ����
        _networkStream.Close();
        _tcpClient.Close();
        _networkStream = null;
        _tcpClient = null;
    }

    public void clearChat()
    {
        Transform content;

        if (LobbyChattingList != null)
        {
            content = LobbyChattingList.transform.Find("Viewport/Content");
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }
    }

    #endregion
}