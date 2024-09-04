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
                        if (_denyRoutine == null && _fertilizeRoutine == null) _denyRoutine = StartCoroutine(DenyRoutine());
                        return;
                    }

                    _fertilizeRoutine ??= StartCoroutine(FertilizeRoutine());
                }
                else if (_denyRoutine == null) _denyRoutine = StartCoroutine(DenyRoutine());
                return;
            }

            if (Hotbar.SelectedTool == Hotbar.Tools.Shears)
                _harvestCoroutine ??= StartCoroutine(Harvest());
            else if (Hotbar.SelectedTool == Hotbar.Tools.Workers)
            {
                _startWorkersRoutine ??= StartCoroutine(StartWorkersRoutine());
            }
            else if (_denyRoutine == null && _startWorkersRoutine == null) _denyRoutine = StartCoroutine(DenyRoutine());
        }
        
        private Coroutine _harvestCoroutine;
        IEnumerator Harvest()
        {
            if (Health <= 0) yield break;
            Health -= 5;
            if (_shrinkRoutine != null) StopCoroutine(_shrinkRoutine);
            _shrinkRoutine = StartCoroutine(ShrinkRoutine());
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
