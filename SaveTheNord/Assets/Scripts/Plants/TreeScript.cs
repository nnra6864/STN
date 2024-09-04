using System.Collections;
using Core;
using UnityEngine;

namespace Plants
{
    public class TreeScript : Plant
    {
        [SerializeField] private float _axeHitCooldown;

        void OnClick()
        {
            if (IsDestroyed) return;
            if (Maturity < 1)
            {
                if (Hotbar.SelectedTool == Hotbar.Tools.Fertilizer)
                {
                    if (IsFertilized || Hotbar.FertilizerCount < 1)
                    {
                        if (_denyRoutine == null && _fertilizeRoutine == null) _denyRoutine = StartCoroutine(DenyRoutine());
                        return;
                    }
                    _fertilizeRoutine ??= StartCoroutine(FertilizeRoutine());
                    return;
                }
                else if (_denyRoutine == null) _denyRoutine = StartCoroutine(DenyRoutine());
                return;
            }
            
            if (Hotbar.SelectedTool == Hotbar.Tools.Axe)
                _hitCoroutine ??= StartCoroutine(AxeHit());
            else if (Hotbar.SelectedTool == Hotbar.Tools.Workers)
            {
                _startWorkersRoutine ??= StartCoroutine(StartWorkersRoutine());
            }
            else if (_denyRoutine == null && _startWorkersRoutine == null) _denyRoutine = StartCoroutine(DenyRoutine());
        }
    
        private Coroutine _hitCoroutine;
        IEnumerator AxeHit()
        {
            if (Health <= 0) yield break;
            Health -= 5;
            if (PlantType == PlantTypes.Tree)
            {
                if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
                _shakeRoutine = StartCoroutine(ShakeRoutine());
            }
            SoundManager.Instance.PlaySound("AxeHit");
            foreach (var particle in HitParticles)
            {
                particle.Play();
            }
            yield return new WaitForSeconds(_axeHitCooldown);
            _hitCoroutine = null;
        }

        public override void MouseEnter()
        {
        
        }
        public override void MouseLeave()
        {
        
        }
        public override void Click()
        {
            OnClick();
        }
    }
}