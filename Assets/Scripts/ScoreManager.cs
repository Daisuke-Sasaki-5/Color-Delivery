using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int Score { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // èâä˙âª
    public void ResetScore()
    {

        Score = 0;
    }

    // ÉXÉRÉAâ¡éZ
    public void AddScore(int amont)
    {
        Score += amont;
    }
}
