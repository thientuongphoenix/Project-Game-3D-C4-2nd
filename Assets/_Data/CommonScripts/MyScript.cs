using UnityEngine;

public class MyScript : MonoBehaviour
{
    private void FixedUpdate()
    {
        //this.TestClass();
        this.TestIsDead();
    }

    void TestIsDead()
    {
        //Zombie zombie = new Zombie();
        //zombie.SetHp(0);
        //string logMessage = zombie.GetName() + " " + zombie.GetCurrentHp() + " " + zombie.IsDead();
        //Debug.Log(logMessage);
    }

    void TestClass()
    {
        Zombie zombie = new Zombie();
        Wolf wolf = new Wolf();
        Eagle eagle = new Eagle();
        Ghost ghost = new Ghost();

        zombie.Moving();
        wolf.Moving();
        eagle.Moving();
        ghost.Moving();
    }
}
