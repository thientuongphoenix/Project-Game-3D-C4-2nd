using System.Collections.Generic;
using UnityEngine;

public class TowerbarUIManager : MonoBehaviour
{
    [System.Serializable]
    public class CooldownSlot
    {
        public TowerCode towerCode;
        public CooldownTowerUI cooldownUI;
    }

    public List<CooldownSlot> slots;

    public static TowerbarUIManager Instance { get; private set; }
    void Awake() { Instance = this; }

    public void StartCooldownFor(TowerCode code, float cooldownTime)
    {
        foreach (var slot in slots)
        {
            if (slot.towerCode == code && slot.cooldownUI != null)
            {
                slot.cooldownUI.StartCooldown(cooldownTime);
                break;
            }
        }
    }
}
