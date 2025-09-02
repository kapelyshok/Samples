using System;
using System.Collections.Generic;
using System.Linq;
using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Infrastructure.Bootstrap;
using AtomicApps.Mechanics.Gameplay.Bonuses;
using AtomicApps.Mechanics.Gameplay.Dictionary;
using AtomicApps.Mechanics.Gameplay.FlyIcons;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using AtomicApps.Mechanics.Gameplay.Levels;
using AtomicApps.Mechanics.Gameplay.Score;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers.Boosters;
using AtomicApps.Pooling;
using UnityEngine;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public class PerksManager : TriggerRegister
    {
        [SerializeField]
        private List<PerkItemView> perkSlots = new List<PerkItemView>();
        [SerializeField]
        private RectTransform perksContainer;

        private List<PerkSO> _activePerks = new List<PerkSO>();
        private LevelStagesManager _levelStagesManager;
        public RectTransform PerksContainer => perksContainer;
        public List<PerkItemView> PerkSlots => perkSlots;
        public FlyIconsManager FlyIconsManager { get; private set; }
        public IObjectPool ObjectPool { get; private set; }
        public BonusesManager BonusesManager { get; private set; }
        public BoostersManager BoostersManager { get; private set; }
        public SelectedLettersManager SelectedLettersManager { get; private set; }
        public GamefieldLettersManager GamefieldLettersManager { get; private set; }
        public LettersBagManager LettersBagManager { get; private set; }
        public ScoreCalculationManager ScoreCalculationManager { get; private set; }
        public IWordsDictionaryService WordsDictionaryService  { get; private set; }

        [Inject]
        private void Construct(SelectedLettersManager selectedLettersManager, GamefieldLettersManager gamefieldLettersManager,
            LettersBagManager lettersBagManager, ScoreCalculationManager scoreCalculationManager, LevelStagesManager levelStagesManager,
            IWordsDictionaryService wordsDictionaryService, BoostersManager boostersManager, BonusesManager bonusesManager,
            IObjectPool objectPool, FlyIconsManager flyIconsManager)
        {
            FlyIconsManager = flyIconsManager;
            ObjectPool = objectPool;
            BonusesManager = bonusesManager;
            BoostersManager = boostersManager;
            WordsDictionaryService = wordsDictionaryService;
            _levelStagesManager = levelStagesManager;
            ScoreCalculationManager = scoreCalculationManager;
            LettersBagManager = lettersBagManager;
            GamefieldLettersManager = gamefieldLettersManager;
            SelectedLettersManager = selectedLettersManager;
        }

        public async void AddPerk(PerkSO perk)
        {
            foreach (var slot in perkSlots)
            {
                if (slot.IsLocked)
                {
                    slot.InitWithAnimation(perk);
                    perk.Init(this, slot);
                    RegisterTrigger(perk);
                    _activePerks.Add(perk);
                    return;
                }
            }
            
            Debug.LogError($"Adding perk {perk} failed! No free space! Obviously smth went wrong");
        }

        public PerkItemView TryGetViewForPerk(PerkSO perk)
        {
            foreach (var perkSlot in perkSlots)
            {
                if (perkSlot.Perk == perk)
                {
                    return perkSlot;
                }
            }

            return null;
        }

        private void OnDestroy()
        {
            foreach (var perk in _activePerks)
            {
                perk.Dispose();
            }
        }

        public List<PerkSO> GetActivePerks()
        {
            return _activePerks;
        }
    }
}
