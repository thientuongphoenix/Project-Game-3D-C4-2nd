using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<Enemy> enemies = new List<Enemy>();
    public List<Enemy> Enemies => this.enemies;

    Enemy smallestEnemy;
    Enemy biggestEnemy;

    private void Awake()
    {
        this.LoadEnemies();
    }

    private void Start()
    {
        this.LoadSmallestEnemy();
        this.LoadBiggestEnemy();
    }

    protected virtual void LoadSmallestEnemy()
    {
        Enemy firstEnemy = enemies[0];
        float smallestWeight = firstEnemy.GetMaxWeight();
        foreach(Enemy enemy in this.enemies)
        {
            float enemyWeight = enemy.GetWeight();
            if(enemyWeight < smallestWeight)
            {
                smallestWeight = enemyWeight;
                this.smallestEnemy = enemy;
            }
            //Debug.Log(enemy.GetObjName() + " " + enemy.GetWeight());
        }
        //Debug.Log("Enemy nhẹ ký nhất: " +  smallestEnemy.GetObjName() + " " + smallestEnemy.GetWeight());
    }

    protected virtual void LoadBiggestEnemy()
    {
        Enemy firstEnemy = enemies[0];
        float biggestWeight = firstEnemy.GetMinWeight();
        foreach(Enemy enemy in this.enemies)
        {
            float enemyWeight = enemy.GetWeight();
            if (enemyWeight > biggestWeight)
            {
                biggestWeight = enemyWeight;
                this.biggestEnemy = enemy;
            }
            //Debug.Log(enemy.GetObjName() + " " + enemy.GetWeight());
        }
        //Debug.Log("Enemy nặng ký nhất: " +  biggestEnemy.GetObjName() + " " + biggestEnemy.GetWeight());
    }

    protected virtual void LoadEnemies()
    {
        foreach(Transform childObj in transform)
        {
            //Debug.Log(childObj.name);
            Enemy enemy = childObj.GetComponent<Enemy>();
            if (enemy == null) continue;
            this.enemies.Add(enemy);
        }
    }
}
