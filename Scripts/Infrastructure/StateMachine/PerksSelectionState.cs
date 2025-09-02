using AtomicApps.Infrastructure.StateMachine;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using AtomicApps.Infrastructure.Services.Popups.Interfaces;
using AtomicApps.Mechanics.Gameplay.Levels;
using AtomicApps.UI.Popups;

namespace AtomicApps.Infrastructure.Bootstrap
{
    public class PerksSelectionState : IPayloadedState<BonusesSelectionData>
    {
        private GameStateMachine _stateMachine;
        private IPopupService _popupService;
        private LevelStagesManager _levelStagesManager;

        public PerksSelectionState(GameStateMachine stateMachine, IPopupService popupService)
        {
            _popupService = popupService;
            _stateMachine = stateMachine;
        }

        public async void Enter(BonusesSelectionData payload)
        {
            _levelStagesManager = payload.LevelStagesManager;
            var popup = await _popupService.ShowPopupAsync<StageCompletedPopup>(PopupKeys.STAGE_COMPLETED_POPUP);
            popup.Initialize(payload);
        }

        private async void OnSceneLoaded()
        {
            
        }

        public void Exit()
        {
            _levelStagesManager.StartNewStage();
        }
    }
}