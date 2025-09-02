using System.Collections.Generic;
using AtomicApps.Mechanics.Gameplay.LettersBag;
using AtomicApps.Mechanics.Gameplay.Levels.Boss;
using AtomicApps.Mechanics.Gameplay.Score;
using AtomicApps.Mechanics.Gameplay.SpecialTriggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using Zenject;

namespace AtomicApps.Mechanics.Gameplay.SpecialTriggers
{
    public abstract class ABaseSpecialTile : MonoBehaviour, ITriggerListener
    {
        [SerializeField]
        private BaseSpecialTileSO specialTileSo;
        [SerializeField]
        private float animationSpeed = 1f;
        [field:SerializeField]
        public TriggerWave TriggerWave { get; set; }
        [field:SerializeField]
        public TriggerPlace TriggerPlace { get; set; }
        
        [Space]
        [SerializeField]
        private ParticleSystem particleSystem;
        [SerializeField]
        private Image glowImage;
        [SerializeField]
        private Image blickImage;

        protected BossStageManager _bossStageManager;
        private IVibrationsService _vibrationsService;

        [Inject]
        private void ConstructBase(BossStageManager bossStageManager, IVibrationsService vibrationsService)
        {
            _vibrationsService = vibrationsService;
            _bossStageManager = bossStageManager;
        }
        
        public ITriggerInitiator ConnectedInitiator { get; set; }
        public Tile Tile => specialTileSo.Tile;
        public Color TextColor  => specialTileSo.TextColor;
        
        public abstract UniTask ProcessTile(ITriggerInitiator initiator);

        public virtual async UniTask CheckTrigger(TriggerWave triggerWave, ITriggerInitiator initiator)
        {
            var isLockedByBossStage = _bossStageManager.CurrentBossStage != null &&
                                      _bossStageManager.CurrentBossStage is FlatFieldBossStageSO;
            
            if (triggerWave == TriggerWave && !isLockedByBossStage)
            {
                await ProcessTile(initiator);
            }
        }

        protected async UniTask DisableGlow()
        {
            await glowImage.DOFade(0f, .2f).SetEase(Ease.InOutQuad).AsyncWaitForCompletion();
            glowImage.gameObject.SetActive(false);
        }
        
        [Button]
        protected virtual async UniTask AnimateTileSuccess()
        {
            _vibrationsService.PlayVibration(HapticPatterns.PresetType.LightImpact);

            //glowImage.gameObject.SetActive(false);
            var imageColor = glowImage.color;
            imageColor.a = 1f;
            glowImage.color = imageColor;
            var color = blickImage.color;
            color.a = 0f;
            blickImage.color = color;
            Vector3 originalScale = transform.localScale;
            float halfDuration = 0.3f / animationSpeed;
            float bounceDuration = 0.7f / animationSpeed;
            float scaleMultiplier = 0.8f;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(blickImage.DOFade(1f, halfDuration).SetEase(Ease.InOutQuad));
            
            sequence.Join(transform
                .DOScale(originalScale * scaleMultiplier, halfDuration)
                .SetEase(Ease.InOutQuad));
            
            sequence.InsertCallback(0.5f / animationSpeed, () =>
            {
                particleSystem.Play();
            });
            
            sequence.InsertCallback(halfDuration, () =>
            {
                blickImage.DOFade(0f, halfDuration / 2).SetEase(Ease.InOutQuad);
                glowImage.gameObject.SetActive(true);
            });

            sequence.Append(transform
                .DOScale(originalScale, bounceDuration)
                .SetEase(Ease.OutElastic));

            await sequence.AsyncWaitForCompletion();
        }        
        
        protected virtual async UniTask AnimateTileFail()
        {
            await transform.DOShakePosition(.4f / animationSpeed,Vector3.one * 8f).AsyncWaitForCompletion();
        }

        public virtual void Activate(TileEntryView tileEntryView)
        {
            gameObject.SetActive(true);        
        }
        
        public virtual void Disable(TileEntryView tileEntryView)
        {
            if(!gameObject.activeSelf) return;
            
            gameObject.SetActive(false);        
        }
    }
}
