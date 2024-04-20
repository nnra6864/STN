using System.Collections;
using UnityEngine;

namespace Core
{
    public class Tile : MonoBehaviour, IInteract
    {
        [HideInInspector] public GameObject TileUI;
        private ParticleSystem _waterPurifierParticles;
        public enum TileTypes
        {
            Ground,
            Water
        }
        
        public TileTypes TileType;
        public bool IsUsed;
        private Color32 _color;
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            TryGetComponent<ParticleSystem>(out _waterPurifierParticles);
            _color = _renderer.material.color;
        }

        public void MouseEnter()
        {
            if (Stats.SelectedTile != this)
                FadeToColor(Stats.TileHoverColor);
        }
        public void MouseLeave()
        {
            if (Stats.SelectedTile != this)
                FadeToColor(_color);
        }
        public void Click()
        {
            if (TileType == TileTypes.Water)
            {
                if (Hotbar.SelectedTool == Hotbar.Tools.WaterPurifier)
                {
                    if (Hotbar.WaterPurifierCount < 1)
                    {
                        _denyCoroutine ??= StartCoroutine(Deny());
                        return;
                    }
                    _applyWp ??= StartCoroutine(ApplyWaterPurifier());
                    return;
                }
            }
            if (IsUsed) { transform.GetChild(0).GetComponent<IInteract>().Click(); return; }
        }

        public void RightClick()
        {
            if (IsUsed) { transform.GetChild(0).GetComponent<IInteract>().RightClick(); return; }
            SelectTile();
        }

        private void SelectTile()
        {
            if (Stats.SelectedTile == this) return;
            SoundManager.Instance.PlaySound("Select");
            Stats.SelectedTile = this;
            Stats.SelectedInfo = TileUI;
            FadeToColor(Stats.TileSelectColor);
        }
        
        public void DeSelectTile()
        {
            SoundManager.Instance.PlaySound("Select");
            FadeToColor(_color);
            Stats.SelectedInfo = null;
        }

        private Coroutine _changeColorRoutine;
        void FadeToColor(Color32 targetColor)
        {
            if(_changeColorRoutine != null) StopCoroutine(_changeColorRoutine);
            _changeColorRoutine = StartCoroutine(ChangeColor(targetColor));
        }
        IEnumerator ChangeColor(Color32 targetColor)
        {
            float lerpPosition = 0;
            Color32 startingColor = _renderer.material.color;
            while (lerpPosition < 1)
            {
                lerpPosition += Time.deltaTime / Stats.ColorUpdateSpeed;
                lerpPosition = Mathf.Clamp01(lerpPosition);
                _renderer.material.color = Vector4.Lerp((Color)startingColor, (Color)targetColor, lerpPosition);
                yield return new WaitForEndOfFrame();
            }
        }

        Coroutine _applyWp;
        IEnumerator ApplyWaterPurifier()
        {
            SoundManager.Instance.PlaySound("PurifyWater");
            _waterPurifierParticles.Play();
            Stats.WaterPollutionLevel -= 5f;
            Hotbar.WaterPurifierCount--;
            yield return new WaitForSeconds(1f);
            _applyWp = null;
        }

        private Coroutine _denyCoroutine;
        IEnumerator Deny()
        {
            SoundManager.Instance.PlaySound("Denied");
            yield return new WaitForSeconds(0.25f);
            _denyCoroutine = null;
        }
    }
}
