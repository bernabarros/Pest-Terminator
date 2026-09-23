using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreScript : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    public UnityEvent enemyDeath;
    void Start()
    {
        enemyDeath.AddListener(IncrementScore);
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void IncrementScore()
    {
        score += 100;
        scoreText.text = score.ToString();
    }
}
