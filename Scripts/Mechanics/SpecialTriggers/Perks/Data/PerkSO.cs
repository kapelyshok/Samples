using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public abstract class PerkSO : ScriptableObject, ITriggerListener, ISelectableGameplayBonus
    {
        [field: SerializeField]
        public GameplayBonusType GameplayBonusType { get; set; }
        [field: SerializeField]
        public Rarity Rarity { get; set; }
        [field: SerializeField]
        public TriggerWave TriggerWave { get; set; }
        [field: SerializeField]
        public TriggerPlace TriggerPlace { get; set; }
        [field: SerializeField, SpritePreview] 
        public Sprite Icon { get; set; }
        [field: SerializeField]
        public string TextSmall { get; private set; }
        [field: SerializeField]
        public string TextDetailed { get; set; }
        
        [field: SerializeField]
        public bool IsOneTimeActivation { get; set; }
        
        protected PerksManager _perksManager;
        protected bool _wasActivated = false;
        private PerkItemView _perkItemView;

        public ITriggerInitiator ConnectedInitiator { get; set; }

        public void Init(PerksManager perksManager, PerkItemView perkItemView)
        {
            _perkItemView = perkItemView;
            _perksManager = perksManager;

            InternalInitialize();
        }

        protected virtual void InternalInitialize()
        {
            _wasActivated = false;
        }

        public virtual void Dispose()
        {
            
        }

        public async UniTask CheckTrigger(TriggerWave triggerWave, ITriggerInitiator initiator)
        {
            if (triggerWave == TriggerWave)
            {
                if (IsOneTimeActivation && !_wasActivated)
                {
                    await ProcessPerk(initiator);
                }
                
                if (!IsOneTimeActivation)
                {
                    await ProcessPerk(initiator);
                }
            }
        }

        protected abstract UniTask ProcessPerk(ITriggerInitiator initiator);

        protected virtual async UniTask AnimateSuccess()
        {
            await _perkItemView.AnimateSuccess();
        }

        protected virtual async UniTask AnimateFailure()
        {
            await _perkItemView.AnimateFailure();
        }
    }
}
