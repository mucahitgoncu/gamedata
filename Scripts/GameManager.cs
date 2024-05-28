using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MailBlock
{
    public string mailName;
    public string mailText;
    public string subject;
    public bool mailControl;

    public MailBlock(string mailName, string subject, string mailText, bool mailControl)
    {
        this.mailName = mailName;
        this.subject = subject;
        this.mailText = mailText;
        this.mailControl = mailControl;
    }
}

public class GameManager : MonoBehaviour
{
    public GameObject shortMailPrefab;
    public Transform mailParent;
    public Vector3 startPosition;
    public Vector3 offset;

    public TextMeshProUGUI mailName;
    public TextMeshProUGUI mailText;
    public TextMeshProUGUI subject;
    public TextMeshProUGUI mailCount;
    private Button notPhishing;
    private Button phishing;
    public MailBlock[] mailBlocks;
    private int currentIndex = 0;
    public ScoreCount scoreCount;
    public mailLength mailLength;
    private int emailCount;
    private int numberOfSolvedMails;
    public RectTransform shortMailLength;

    private List<GameObject> mailInstances = new List<GameObject>();

    IEnumerator Start()
    {
        yield return StartCoroutine(LoadMailBlocksFromTextFile("file:///C:/Users/mucah/OneDrive/Masa%C3%BCst%C3%BC/Furkan%20ayta%C3%A7/Mail-Game/Phishy%20Mail%20Game/Assets/StreamingAssets/Input.txt"));
        emailCount = mailBlocks.Length;
        mailCount.text = mailBlocks.Length.ToString();
        if (emailCount > 0)
        {
            ChangeMail();
            mailLength.ChangeLength();
            CreateMailBlocks();
            shortMailPanelLengthSetting();
        }
    }

    IEnumerator LoadMailBlocksFromTextFile(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error downloading: " + www.error);
            }
            else
            {
                string[] lines = www.downloadHandler.text.Split('\n');
                if (lines.Length % 4 != 0)
                {
                    Debug.LogError("Invalid file format. The number of lines in the file must be a multiple of 4.");
                    yield break;
                }

                mailBlocks = new MailBlock[lines.Length / 4];

                for (int i = 0; i < lines.Length; i += 4)
                {
                    string mailName = lines[i];
                    string subject = lines[i + 1];
                    string mailText = lines[i + 2];
                    bool mailControl;
                    if (!bool.TryParse(lines[i + 3].Trim(), out mailControl))
                    {
                        Debug.LogError("Invalid boolean value in file at line: " + (i + 4));
                        yield break;
                    }
                    mailBlocks[i / 4] = new MailBlock(mailName, subject, mailText, mailControl);
                }
            }
        }
    }

    void CreateMailBlocks()
    {
        for (int i = 0; i < mailBlocks.Length; i++)
        {
            CreateMailBlock(i);
        }
    }

    void CreateMailBlock(int index)
    {
        GameObject mailInstance = Instantiate(shortMailPrefab, mailParent);
        mailInstance.transform.localPosition = startPosition + index * offset;

        TextMeshProUGUI[] texts = mailInstance.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            if (text.name == "MailTitle")
                text.text = mailBlocks[index].mailName;
            else if (text.name == "Subject")
                text.text = mailBlocks[index].subject;
            else if (text.name == "MailTextTitle")
                text.text = mailBlocks[index].mailText;
        }

        Button mailButton = mailInstance.GetComponent<Button>();
        if (mailButton == null)
        {
            mailButton = mailInstance.AddComponent<Button>();
        }

        mailButton.onClick.AddListener(() => OnMailClick(index));

        mailInstances.Add(mailInstance);
    }

    void OnMailClick(int index)
    {
        currentIndex = index;
        ChangeMail();
    }

    public void NotPhishingButton()
    {
        if (currentIndex < mailBlocks.Length)
        {
            if (mailBlocks[currentIndex].mailControl == true)
                scoreCount.AddScore();
            else
                scoreCount.RemoveScore();
            numberOfSolvedMails++;
            RemoveMail(currentIndex);
            if (mailBlocks.Length > 0)
            {
                currentIndex = currentIndex % mailBlocks.Length;
                ChangeMail();
            }
            else
            {
                ClearMail();
                RestartGame();
            }
        }
        else
        {
            Debug.LogWarning("Current index is out of range, can't proceed with NotPhishingButton.");
        }
    }

    public void PhishingButton()
    {
        if (currentIndex < mailBlocks.Length)
        {
            if (mailBlocks[currentIndex].mailControl == false)
                scoreCount.AddScore();
            else
                scoreCount.RemoveScore();
            numberOfSolvedMails++;
            RemoveMail(currentIndex);
            if (mailBlocks.Length > 0)
            {
                currentIndex = currentIndex % mailBlocks.Length;
                ChangeMail();
            }
            else
            {
                ClearMail();
                RestartGame();
            }
        }
        else
        {
            Debug.LogWarning("Current index is out of range, can't proceed with PhishingButton.");
        }
    }

    public void RemoveMail(int index)
    {
        if (index < mailBlocks.Length)
        {
            Destroy(mailInstances[index]);
            mailInstances.RemoveAt(index);
            mailBlocks = mailBlocks.Where((val, idx) => idx != index).ToArray();
            shortMailPanelLengthSetting();

            for (int i = 0; i < mailInstances.Count; i++)
            {
                mailInstances[i].transform.localPosition = startPosition + i * offset;

                // Update the button listener to the correct index
                int buttonIndex = i;
                Button mailButton = mailInstances[i].GetComponent<Button>();
                mailButton.onClick.RemoveAllListeners();
                mailButton.onClick.AddListener(() => OnMailClick(buttonIndex));
            }

            if (mailBlocks.Length == 0)
            {
                ClearMail();
                RestartGame();
            }
        }
        mailCount.text = mailBlocks.Length.ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeMail()
    {
        if (currentIndex < mailBlocks.Length)
        {
            mailName.text = mailBlocks[currentIndex].mailName;
            mailText.text = mailBlocks[currentIndex].mailText;
            subject.text = mailBlocks[currentIndex].subject;
        }
        else
        {
            Debug.LogWarning("Current index is out of range, can't change mail.");
        }
        mailLength.ChangeLength();
    }

    public void ClearMail()
    {
        mailName.text = "";
        mailText.text = "";
        subject.text = "";
    }

    public void shortMailPanelLengthSetting()
    {
        Vector2 sizeDelta = shortMailLength.sizeDelta;
        sizeDelta.y = (mailBlocks.Length * 135);
        shortMailLength.sizeDelta = sizeDelta;
    }
}
