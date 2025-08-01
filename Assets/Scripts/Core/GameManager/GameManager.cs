using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameMode CurrentGameMode = GameMode.EndlessRunner;
    private bool gameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CheckGameOver()
    {
        if (gameOver) return;

        switch (CurrentGameMode)
        {
            case GameMode.EndlessRunner:
                if (FindObjectOfType<CultistManager>().GetActiveCultists().Count == 0)
                {
                    TriggerGameOver();
                }
                break;
            case GameMode.StageBased:
                // You will add custom checks for stages later
                break;
        }
    }

    private void TriggerGameOver()
    {
        gameOver = true;
        Debug.Log("Game Over!");
        // Show Game Over UI, stop time, etc.
    }
}