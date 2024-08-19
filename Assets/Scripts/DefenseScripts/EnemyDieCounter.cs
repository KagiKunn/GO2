using UnityEngine;

namespace DefenseScripts
{
    public class EnemyDieCounter : MonoBehaviour
    {
        public int enemyDieCount;

        public int EnemyDieCount()
        {
            enemyDieCount++;
            return enemyDieCount;
        }

    }
}