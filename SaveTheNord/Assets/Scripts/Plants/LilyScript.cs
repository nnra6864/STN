using Core;

namespace Plants
{
    public class LilyScript : Plant
    {
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

            if (Hotbar.SelectedTool == Hotbar.Tools.Hand)
                Harvest();
            else if (_denyRoutine == null) _denyRoutine = StartCoroutine(DenyRoutine());
        }
    
        void Harvest()
        {
            SoundManager.Instance.PlaySound("LilyHarvest");
            foreach (var particle in HitParticles)
            {
                particle.Play();
            }
            StartCoroutine(Die());
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
