using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LapController : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerScript script;
    [SerializeField] private LapTimeController timeController;

    [Header("CheckPoints")]
    public Vector3 respawnPointPosition; // ������ ��ġ
    public Quaternion respawnPointRotation; // ������ Rotation
    public GameObject startPoint; // ���� ����
    public TMP_Text warningMsg;

    [Header("Laps")]
    public int currentIndex; // �ε��� üŷ
    public int checkPointsCnt; // üũ ����Ʈ ����
    public int currentLap; // ���� ������

    [Header("Results")]
    public TMP_Text currentLapTxt;
    public TMP_Text nickName;
    public GameObject Result;
    public bool isFinish;

    void Start()
    {
        respawnPointPosition = startPoint.transform.position;
        respawnPointRotation = startPoint.transform.rotation;

        currentLap = 1;

        // 'Respawn' �±׸� ���� ��� ������Ʈ ã��
        GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        // üũ����Ʈ ���� ����
        checkPointsCnt = respawnPoints.Length;

        isFinish = false;
        Result.SetActive(false);
    }

    // üũ����Ʈ ������ �����ϴ� �޼���
    public void UpdateCheckPoint(int index, Vector3 checkpointPosition, Quaternion checkpointRotation)
    {
        if (currentIndex == (index - 1))
        {
            currentIndex++;
            respawnPointPosition = checkpointPosition;
            respawnPointRotation = checkpointRotation;
        }
        else if (currentIndex == -1 && index == 1)
        {
            currentIndex = 1;
            respawnPointPosition = checkpointPosition;
            respawnPointRotation = checkpointRotation;
        }
        else if (currentIndex == index)
        {
            currentIndex = index;
            respawnPointPosition = checkpointPosition;
            respawnPointRotation = checkpointRotation;
        }
        else if (currentIndex != (index - 1))
        {
            WrongCheck();
        }
    }

    public void UpdateLap()
    {
        if (SceneManager.GetActiveScene().name == "Cebu Track" || SceneManager.GetActiveScene().name == "Boryeong Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap <= 3)
            {
                currentLap++;

                if (currentLap == 1)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.white;
                    currentIndex = -1;
                }
                else if (currentLap == 2)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.yellow;
                    currentIndex = -1;
                }
                else if (currentLap == 3)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.red;
                    currentIndex = -1;
                }
                else if (currentLap == 4)
                {
                    currentLapTxt.text = "3";
                    currentLapTxt.color = Color.red;
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "Mexico Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap <= 2)
            {
                currentLap++;

                if (currentLap == 1)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.white;
                    currentIndex = -1;
                }
                else if (currentLap == 2)
                {
                    currentLapTxt.text = currentLap.ToString();
                    currentLapTxt.color = Color.red;
                    currentIndex = -1;
                }
                else if (currentLap == 3)
                {
                    currentLapTxt.text = "2";
                    currentLapTxt.color = Color.red;
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "Downhill Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap <= 1)
            {
                currentLap++;

                if (currentLap == 1)
                {
                    currentLapTxt.text = "1";
                    currentLapTxt.color = Color.red;
                }
            }
        }
    }

    private void WrongCheck()
    {
        StartCoroutine(Warning());
        StartCoroutine(ForcedRespawn());
    }

    private IEnumerator ForcedRespawn()
    {
        yield return new WaitForSeconds(2.0f);

        script.EnforceRespawn();
    }

    private IEnumerator Warning()
    {
        float blinkTime = 0.5f; // �����̴� �ֱ�
        float warningDuration = 2.0f; // ��� �޽����� ���ӵǴ� �ð�

        float endTime = Time.time + warningDuration;
        while (Time.time < endTime)
        {
            Color msgColor = warningMsg.color;
            msgColor.a = msgColor.a == 1.0f ? 0.0f : 1.0f; // ���� ��ȯ
            warningMsg.color = msgColor;
            yield return new WaitForSeconds(blinkTime);
        }

        Color finalColor = warningMsg.color;
        finalColor.a = 0; // ������ 0���� �����Ͽ� �޽����� ������ ����
        warningMsg.color = finalColor;
    }

    public IEnumerator Finish()
    {
        yield return new WaitForSeconds(0.2f);

        if (SceneManager.GetActiveScene().name == "Cebu Track" || SceneManager.GetActiveScene().name == "Boryeong Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap > 3)
            {
                // Player �±׸� ���� ��� ���� ������Ʈ�� Ž��
                var players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (player != this.gameObject)
                    {
                        // �ٸ� �÷��̾���� �浹�� ����
                        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
                    }
                }

                isFinish = true;

                if (timeController.resultTimerText != null)
                {
                    timeController.resultTimerText.text = timeController.timerText.text; // ���â Ÿ�̸ӿ� ���� Ÿ�̸� �ؽ�Ʈ�� ����
                }

                // Result�� CanvasGroup ������Ʈ�� ������ �ִٰ� ����
                CanvasGroup resultCanvasGroup = Result.GetComponent<CanvasGroup>();
                if (resultCanvasGroup != null)
                {
                    float duration = 1.0f; // ���̵� ���ϴ� �� �ɸ��� �ð�(��)
                    float elapsedTime = 0;

                    // CanvasGroup�� alpha ���� 0���� 1�� ������ ����
                    while (elapsedTime < duration)
                    {
                        elapsedTime += Time.deltaTime;
                        resultCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                        yield return null;
                    }
                    resultCanvasGroup.alpha = 1; // ���������� alpha ���� ������ 1�� �����Ͽ� Ȯ���� ���̰� ��
                }

                Result.SetActive(true);
            }
        }
        else if (SceneManager.GetActiveScene().name == "Mexico Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap > 2)
            {
                // Player �±׸� ���� ��� ���� ������Ʈ�� Ž��
                var players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (player != this.gameObject)
                    {
                        // �ٸ� �÷��̾���� �浹�� ����
                        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
                    }
                }

                isFinish = true;

                if (timeController.resultTimerText != null)
                {
                    timeController.resultTimerText.text = timeController.timerText.text; // ���â Ÿ�̸ӿ� ���� Ÿ�̸� �ؽ�Ʈ�� ����
                }

                // Result�� CanvasGroup ������Ʈ�� ������ �ִٰ� ����
                CanvasGroup resultCanvasGroup = Result.GetComponent<CanvasGroup>();
                if (resultCanvasGroup != null)
                {
                    float duration = 1.0f; // ���̵� ���ϴ� �� �ɸ��� �ð�(��)
                    float elapsedTime = 0;

                    // CanvasGroup�� alpha ���� 0���� 1�� ������ ����
                    while (elapsedTime < duration)
                    {
                        elapsedTime += Time.deltaTime;
                        resultCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                        yield return null;
                    }
                    resultCanvasGroup.alpha = 1; // ���������� alpha ���� ������ 1�� �����Ͽ� Ȯ���� ���̰� ��
                }

                Result.SetActive(true);
            }
        }
        else if (SceneManager.GetActiveScene().name == "Downhill Track")
        {
            if (currentIndex >= checkPointsCnt && currentLap > 1)
            {
                // Player �±׸� ���� ��� ���� ������Ʈ�� Ž��
                var players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (player != this.gameObject)
                    {
                        // �ٸ� �÷��̾���� �浹�� ����
                        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
                    }
                }

                isFinish = true;

                if (timeController.resultTimerText != null)
                {
                    timeController.resultTimerText.text = timeController.timerText.text; // ���â Ÿ�̸ӿ� ���� Ÿ�̸� �ؽ�Ʈ�� ����
                }

                // Result�� CanvasGroup ������Ʈ�� ������ �ִٰ� ����
                CanvasGroup resultCanvasGroup = Result.GetComponent<CanvasGroup>();
                if (resultCanvasGroup != null)
                {
                    float duration = 1.0f; // ���̵� ���ϴ� �� �ɸ��� �ð�(��)
                    float elapsedTime = 0;

                    // CanvasGroup�� alpha ���� 0���� 1�� ������ ����
                    while (elapsedTime < duration)
                    {
                        elapsedTime += Time.deltaTime;
                        resultCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                        yield return null;
                    }
                    resultCanvasGroup.alpha = 1; // ���������� alpha ���� ������ 1�� �����Ͽ� Ȯ���� ���̰� ��
                }

                Result.SetActive(true);
            }
        }
        nickName.text = DataManager.Instance.loginUserInfo.dataBody.nickname;
    }
}