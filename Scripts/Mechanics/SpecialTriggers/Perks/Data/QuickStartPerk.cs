using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    [CreateAssetMenu(fileName = nameof(QuickStartPerk), menuName = "ScriptableObjects/Perks/" + nameof(QuickStartPerk))]
    public class QuickStartPerk : PerkSO
    {
        [Space]
        [SerializeField]
        private List<int> newBonuses = new List<int>(){1,1,1,1,3,4};
        
        protected override async UniTask ProcessPerk(ITriggerInitiator initiator)
        {
            var cells = _perksManager.SelectedLettersManager.GetAllSelectedLetterCells();

            var tasks = new List<UniTask>();
            
            await AnimateSuccess();
            _wasActivated = true;

            for (int i = 0; i < newBonuses.Count; i++)
            {
                tasks.Add(cells[i].ChangeDefaultCellBonus(newBonuses[i]));
                await UniTask.WaitForSeconds(0.1f);
            }

            await UniTask.WhenAll(tasks);
        }
    }
}