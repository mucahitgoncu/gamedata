using UnityEngine;
using TMPro;

public class ScoreCount : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] bool sad;
    private  int score;
    void Start()
    {
        score = 0;
        scoreText = GetComponent<TextMeshProUGUI>();
    }
    public  void AddScore()
    {
        score = score + 3;
        scoreText.text = score.ToString();
    }
    public  void RemoveScore()
    {
        score = score - 3;
        scoreText.text = score.ToString();
    }
}
