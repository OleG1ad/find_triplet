using UnityEngine;
using UnityEngine.UI;

public class StartBut : MonoBehaviour
{
    private Button button;
    private GameManager manager;
    public int difficulty;

    private void Start()
    {
        button = GetComponent<Button>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        button.onClick.AddListener(SetDifficulty);
    }

    private void SetDifficulty()
    {
        manager.StartGame(difficulty);
    }
}
