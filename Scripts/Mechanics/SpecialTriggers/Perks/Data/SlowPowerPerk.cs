using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(SlowPowerPerk), menuName = "ScriptableObjects/Perks/" + nameof(SlowPowerPerk))]
    public class SlowPowerPerk : PerkSO
    {
        [Space]
        [SerializeField]
        private List<int> newBonuses = new List<int>(){8,10,12,15};
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var cells = _perksManager.SelectedLettersManager.GetAllSelectedLetterCells();

            var tasks = new List<UniTask>();
            
            await AnimateSuccess();
            _wasActivated = true;

            for (int i = 0; i < newBonuses.Count; i++)
            {
                int cellIndex = cells.Count - newBonuses.Count + i;
                tasks.Add(cells[cellIndex].ChangeDefaultCellBonus(newBonuses[i]));
                await UniTask.WaitForSeconds(0.1f);
            }

            await UniTask.WhenAll(tasks);
        }
    }
}