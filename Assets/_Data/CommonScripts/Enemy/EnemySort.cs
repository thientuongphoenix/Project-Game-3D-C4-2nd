using System.Collections.Generic;
using UnityEngine;

public class EnemySort : MonoBehaviour
{
    List<Enemy> enemies = new List<Enemy>();
    EnemyManager enemyManager;

    private void Awake()
    {
        this.LoadComponents();
    }

    private void Reset()
    {
        this.LoadComponents();
    }

    private void Start()
    {
        this.Sorting();
        this.ShowEnemies();
    }

    protected virtual void LoadComponents()
    {
        if (this.enemyManager != null) return;
        this.enemyManager = GetComponent<EnemyManager>();
        //Debug.Log(transform.name + ("LoadComponents"));
    }

    protected virtual void Sorting()
    {
        this.enemies = this.enemyManager.Enemies;

        int n = this.enemies.Count;
        for(int i = 0; i < n - 1; i++)
        {
            int minIndex = i;
            for(int j = i + 1; j < n; j++)
            {
                if (this.enemies[j].GetWeight() < this.enemies[minIndex].GetWeight())
                {
                    minIndex = j;
                }
            }

            if(minIndex != i)
            {
                Enemy temp = this.enemies[i];
                this.enemies[i] = this.enemies[minIndex];
                this.enemies[minIndex] = temp;
            }
        }
    }

    protected virtual void ShowEnemies()
    {
        foreach(Enemy enemy in this.enemies)
        {
            Debug.Log(enemy.GetObjName() + " Enemy Weight: " + enemy.GetWeight());
        }
    }
}
