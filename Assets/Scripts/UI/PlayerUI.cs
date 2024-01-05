using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    Slider healthBar;

    [SerializeField]
    Text scoreText;
    int score;

    // Start is called before the first frame update
    void Start()
    {
        DecisionMakingEntity player = FindObjectOfType<DecisionMakingEntity>();
        player.GetComponent<Health>().OnHealthChanged += ChangehealthUI;

        SimpleEnemy.OnEnemyDeath += ScoreIncrement;
    }

    void ChangehealthUI(int max, int curr)
    {
        healthBar.value = (float)curr / (float)max;
    }

    void ScoreIncrement()
    {
        score += 100;
        scoreText.text = score.ToString();
    }
}
