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
                        if (DeniedCoroutine == null && FertilizeCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
                        return;
                    }
                    FertilizeCoroutine ??= StartCoroutine(Fertilize());
                }
                else if (DeniedCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
                return;
            }

            if (Hotbar.SelectedTool == Hotbar.Tools.Hand)
                Harvest();
            else if (DeniedCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
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
