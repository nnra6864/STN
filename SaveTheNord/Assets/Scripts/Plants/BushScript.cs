using System.Collections;
using Core;
using UnityEngine;

namespace Plants
{
    public class BushScript : Plant
    {
        [SerializeField] private float _harvestCooldown;
        
        private void OnClick()
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
                }
                else if (DeniedCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
                return;
            }

            if (Hotbar.SelectedTool == Hotbar.Tools.Shears)
                _harvestCoroutine ??= StartCoroutine(Harvest());
            else if (Hotbar.SelectedTool == Hotbar.Tools.Workers)
            {
                StartWorkersCoroutine ??= StartCoroutine(StartWorkers());
            }
            else if (DeniedCoroutine == null && StartWorkersCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
        }
        
        private Coroutine _harvestCoroutine;
        IEnumerator Harvest()
        {
            if (Health <= 0) yield break;
            Health -= 5;
            if (ShrinkCoroutine != null) StopCoroutine(ShrinkCoroutine);
            ShrinkCoroutine = StartCoroutine(Shrink());
            SoundManager.Instance.PlaySound("Shears");
            foreach (var particle in HitParticles)
            {
                particle.Play();
            }
            yield return new WaitForSeconds(_harvestCooldown);
            _harvestCoroutine = null;
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
