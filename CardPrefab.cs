using UnityEngine;

public class CardPrefab : MonoBehaviour
{
    [SerializeField] private GameObject backPrefab;
    [SerializeField] private GameManager controller;

    private int _id;
    public int Id { get { return _id; } }

    public void SetCard(int id, Sprite image)
    {
        _id = id;
        GetComponent<SpriteRenderer>().sprite = image;
    }

    public void OnMouseDown()
    {
        if (backPrefab.activeSelf && controller.CanReveal && controller.isGameActive && !controller.isFlash)
        {
            backPrefab.SetActive(false);
            controller.CardRevealed(this);
        }
    }

    public void Unreveal()
    {
        backPrefab.SetActive(true);
    }
}
