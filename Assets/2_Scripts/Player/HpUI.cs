using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Player player;

    [Header("Hp UI")]
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Transform heartParent;

    [Header("Sprites")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private List<Image> hearts = new List<Image>();

    private void Start()
    {
        if (player == null)
            player = FindAnyObjectByType<Player>();

        CreateHearts();
        UpdateHearts();
    }

    private void Update()
    {
        UpdateHearts();
    }

    void CreateHearts()
    {
        foreach (Transform child in heartParent)
        {
            Destroy(child.gameObject);
        }

        hearts.Clear();

        int heartCount = Mathf.RoundToInt(player.hp);

        for (int i = 0; i < heartCount; i++)
        {
            Image heart = Instantiate(heartPrefab, heartParent);
            hearts.Add(heart);
        }
    }

    void UpdateHearts()
    {
        int needHeartCount = Mathf.RoundToInt(player.hp);

        if (hearts.Count != needHeartCount)
        {
            CreateHearts();
        }

        int currentHp = Mathf.RoundToInt(player.currenthp);

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHp)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}