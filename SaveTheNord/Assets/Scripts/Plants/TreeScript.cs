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
                        if (DeniedCoroutine == null && FertilizeCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
                        return;
                    }
                    FertilizeCoroutine ??= StartCoroutine(Fertilize());
                    return;
                }
                else if (DeniedCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
                return;
            }
            
            if (Hotbar.SelectedTool == Hotbar.Tools.Axe)
                _hitCoroutine ??= StartCoroutine(AxeHit());
            else if (Hotbar.SelectedTool == Hotbar.Tools.Workers)
            {
                StartWorkersCoroutine ??= StartCoroutine(StartWorkers());
            }
            else if (DeniedCoroutine == null && StartWorkersCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
        }
    
        private Coroutine _hitCoroutine;
        IEnumerator AxeHit()
        {
            if (Health <= 0) yield break;
            Health -= 5;
            if (PlantType == PlantTypes.Tree)
            {
                if (ShakeCoroutine != null) StopCoroutine(ShakeCoroutine);
                ShakeCoroutine = StartCoroutine(Shake());
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