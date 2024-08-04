using System;
using UnityEngine;

namespace DefenseScripts
{
    public class BossBehaviour : MonoBehaviour
    {
        public event Action OnBossDisabledEvent;

        private void OnDisable()
        {
            CustomLogger.Log("BossBehaviour OnDisable 호출됨", "yellow");
            OnBossDisabledEvent?.Invoke();
        }
    }
}