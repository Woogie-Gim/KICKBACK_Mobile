using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UnityEngine.UI ���ӽ����̽� �߰�
using TMPro; // TMPro ���ӽ����̽� �߰�

public class LapTimeController : MonoBehaviour
{
    [SerializeField] private LapController controller;
    public TextMeshProUGUI timerText; // Ÿ�̸� �ؽ�Ʈ
    public TextMeshProUGUI resultTimerText; // ���â�� Ÿ�̸� �ؽ�Ʈ ������ ���� �ʵ�
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (!controller.isFinish) // ��Ⱑ ������ �ʾ��� ���� Ÿ�̸� ������Ʈ
        {
            float time = Time.time - startTime;

            int minutes = GetMinute(time);
            int seconds = GetSecond(time);
            string milliseconds = GetMilliseconds(time);

            timerText.text = string.Format("{0:00}:{1:00}:{2}", minutes, seconds, milliseconds);
        }
    }

    public int GetMinute(float time)
    {
        return (int)((time / 60) % 60);
    }

    public int GetSecond(float time)
    {
        return (int)(time % 60);
    }

    public string GetMilliseconds(float time)
    {
        string milliseconds = string.Format("{0:00}", (time % 1) * 100);
        return milliseconds;
    }
}
