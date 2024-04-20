using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Plants
{
    public class PlantNumberItem
    {
        public int Amount;
        public readonly int Target;
        public delegate void AmountChanged();
        public AmountChanged OnAmountChanged;

        public PlantNumberItem(int amount, int target)
        {
            Amount = amount;
            Target = target;
            OnAmountChanged = null;
        }
    }
    
    public class Plant : MonoBehaviour, IInteract
    {
        public delegate void OnValueChanged();

        public enum PlantTypes
        {
            Bush,
            Tree,
            Lily
        }
        public enum PlantNames
        {
            NoakBush,
            NoakLily,
            NoakTree,
            NirchBush,
            NirchLily,
            NirchTree,
            AnaciaBush,
            AnaciaLily,
            AnaciaTree,
            NordBush,
            NordLily,
            NordTree
        }
        [System.Serializable]
        public struct PlantDrop
        {
            public InventoryItem Drop;
            [Tooltip("Amount range of dropped items, both are inclusive")]
            public Vector2Int DropAmountRange;
        }

        [SerializeField] private GameObject _info;
        public PlantTypes PlantType;
        [SerializeField] private PlantNames _plantName;
        [Tooltip("Determines on which type of tiles the plant can be planted on.")]
        public Tile.TileTypes TileType;

        private float _maturity;
        public float Maturity
        {
            get => _maturity;
            set
            {
                _maturity = value;
                OnMaturityChanged?.Invoke();
            }
        }
        public OnValueChanged OnMaturityChanged;
        
        [HideInInspector] public float StartingHealth;
        protected Vector3 StartingPosition;
        private bool _isFertilized;
        public bool IsFertilized
        {
            get => _isFertilized;
            set
            {
                _isFertilized = value;
                OnIsFertilizedChanged?.Invoke();
            }
        }
        public OnValueChanged OnIsFertilizedChanged;
        
        [HideInInspector] public bool IsDestroyed;
        [SerializeField] public float GrowthTime;
        [SerializeField] private float _healthValue;
        
        private float _airPollutionReduction, _waterPollutionReduction;
        public float AirPollutionReduction, WaterPollutionReduction;

        [SerializeField] protected ParticleSystem PlantParticles;
        [SerializeField] protected ParticleSystem FertilizeParticles;
        [SerializeField] protected List<ParticleSystem> HitParticles;
        [SerializeField] protected List<ParticleSystem> DeathParticles;
        [SerializeField] private List<PlantDrop> _plantDrops;

        public float Health
        {
            get => _healthValue;
            set
            {
                if (value <= 0)
                {
                    _healthValue = 0;
                    OnHealthChanged?.Invoke();
                    StartCoroutine(Die());
                    return;
                }
                _healthValue = value;
                OnHealthChanged?.Invoke();
            }
        }
        public OnValueChanged OnHealthChanged;
        
        protected void Awake()
        {
            StartingHealth = Health;
            StartingPosition = transform.position;
            StartCoroutine(LifeCycle());
        }

        private IEnumerator LifeCycle()
        {
            PlantParticles.Play();
            transform.Rotate(0, Random.Range(0, 360), 0);
            while (Maturity < 1)
            {
                Maturity += Time.deltaTime / GrowthTime;
                Maturity = Mathf.Clamp01(Maturity);
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Maturity);
                yield return new WaitForEndOfFrame();
            }
            Stats.PlantNumbers[_plantName].Amount++;
            Stats.PlantNumbers[_plantName].OnAmountChanged?.Invoke();
            ReducePollutionCoroutine = StartCoroutine(ReducePollution());
        }

        protected Coroutine ReducePollutionCoroutine;
        protected IEnumerator ReducePollution()
        {
            while (Health > 0)
            {
                Stats.AirPollutionLevel -= ((AirPollutionReduction / StartingHealth) * Health) * Time.deltaTime;
                Stats.WaterPollutionLevel -= ((WaterPollutionReduction / StartingHealth) * Health)  * Time.deltaTime;
                yield return null;
            }
        }

        protected Coroutine EngageWorkersCoroutine;
        protected IEnumerator EngageWorkers()
        {
            Hotbar.WorkersCount--;
            SoundManager.Instance.PlaySound("EngageWorkers");
            while (Health > 0)
            {
                Health -= 5;
                foreach (var particle in HitParticles)
                {
                    particle.Play();
                }
                SoundManager.Instance.PlaySound(PlantType == PlantTypes.Tree ? "WorkersAxeHit" : "WorkersShears");
                if (PlantType == PlantTypes.Tree)
                {
                    if (ShakeCoroutine != null) StopCoroutine(ShakeCoroutine);
                    ShakeCoroutine = StartCoroutine(Shake());
                }
                else if (PlantType == PlantTypes.Bush)
                {
                    if (ShrinkCoroutine != null) StopCoroutine(ShrinkCoroutine);
                    ShrinkCoroutine = StartCoroutine(Shrink());
                }
                yield return new WaitForSeconds(PlantType == PlantTypes.Tree ? 0.6f : 0.4f);
            }
        }

        protected IEnumerator Die()
        {
            IsDestroyed = true;
            float lerpPosition = 0;
            
            foreach (var particle in DeathParticles)
            {
                particle.Play();
            }
            
            while (lerpPosition < 1)
            {
                lerpPosition += Time.deltaTime / 0.5f;
                var t = NnUtils.EaseInBack(lerpPosition);
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return new WaitForEndOfFrame();
            }

            foreach (var drop in _plantDrops)
            {
                var amount = Random.Range(drop.DropAmountRange.x, drop.DropAmountRange.y + 1);
                if (amount == 0) continue;
                drop.Drop.Amount += amount;

                var existingItem = UI.Drops.DroppedItems.Find(x => x.Item == drop.Drop);
                if (existingItem != null)
                {
                    existingItem.Amount += amount;
                }
                else
                    UI.Drops.DroppedItems.Add(new DropItem(drop.Drop, amount));
            }

            SoundManager.Instance.PlaySound("PlantDestroyed");
            transform.parent.GetComponent<Tile>().IsUsed = false;
            Stats.PlantNumbers[_plantName].Amount--;
            Stats.PlantNumbers[_plantName].OnAmountChanged?.Invoke();
            switch (PlantType)
            {
                case PlantTypes.Bush: Stats.BushesDestroyed++; break;
                case PlantTypes.Tree: Stats.TreesDestroyed++; break;
                case PlantTypes.Lily: Stats.LiliesDestroyed++; break;
            }
            Destroy(gameObject);
        }

        protected Coroutine DeniedCoroutine;
        protected IEnumerator Deny()
        {
            SoundManager.Instance.PlaySound("Denied");
            yield return new WaitForSeconds(0.25f);
            DeniedCoroutine = null;
        }
        
        protected Coroutine FertilizeCoroutine;
        protected IEnumerator Fertilize()
        {
            SoundManager.Instance.PlaySound("Fertilize");
            FertilizeParticles.Play();
            IsFertilized = true;
            GrowthTime *= 0.5f;
            Hotbar.FertilizerCount--;
            yield return new WaitForSeconds(0.25f);
            FertilizeCoroutine = null;
        }

        protected Coroutine StartWorkersCoroutine;
        protected IEnumerator StartWorkers()
        {
            if (Hotbar.WorkersCount < 1)
            {
                if (DeniedCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
                yield break;
            }
            SoundManager.Instance.PlaySound("EngageWorkers");
            if (EngageWorkersCoroutine == null)
                EngageWorkersCoroutine = StartCoroutine(EngageWorkers());
            else if (DeniedCoroutine == null) DeniedCoroutine = StartCoroutine(Deny());
            yield return new WaitForSeconds(0.25f);
            StartWorkersCoroutine = null;
        }
        
        public void ToggleInfo()
        {
            if (_info.activeSelf)
                Stats.SelectedInfo = null;
            else
                Stats.SelectedInfo = _info;
            SoundManager.Instance.PlaySound("Select");
        }

        protected Coroutine ShakeCoroutine;
        protected IEnumerator Shake()
        {
            float lerpPosition = 0;
            var startingPosition = transform.position;
            var targetPosition = new Vector3(StartingPosition.x + Random.Range(-0.05f, 0.05f), StartingPosition.y, StartingPosition.z + Random.Range(-0.05f, 0.05f));
            while (lerpPosition < 1)
            {
                lerpPosition += Time.deltaTime / 0.05f;
                lerpPosition = Mathf.Clamp01(lerpPosition);
                var t = NnUtils.EaseOut(lerpPosition);
                transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
                yield return null;
            }
            while (lerpPosition > 0)
            {
                lerpPosition -= Time.deltaTime / 0.5f;
                lerpPosition = Mathf.Clamp01(lerpPosition);
                var t = NnUtils.EaseIn(lerpPosition);
                transform.position = Vector3.Lerp(StartingPosition, targetPosition, t);
                yield return null;
            }
            ShakeCoroutine = null;
        }

        protected Coroutine ShrinkCoroutine;
        protected IEnumerator Shrink()
        {
            float lerpPosition = 0;
            var startingScale = transform.localScale;
            var rand = Random.Range(0.9f, 1.1f);
            var targetScale = new Vector3(rand, rand, rand);
            while (lerpPosition < 1)
            {
                lerpPosition += Time.deltaTime / 0.1f;
                lerpPosition = Mathf.Clamp01(lerpPosition);
                var t = NnUtils.EaseIn(lerpPosition);
                transform.localScale = Vector3.Lerp(startingScale, targetScale, t);
                yield return null;
            }
            while (lerpPosition > 0)
            {
                lerpPosition -= Time.deltaTime / 0.1f;
                lerpPosition = Mathf.Clamp01(lerpPosition);
                var t = NnUtils.EaseOut(lerpPosition);
                transform.localScale = Vector3.Lerp(Vector3.one, targetScale, t);
                yield return null;
            }
            ShakeCoroutine = null;
        }
        
        public virtual void MouseEnter() { }
        public virtual void MouseLeave() { }
        public virtual void Click() { }

        public virtual void RightClick()
        {
            ToggleInfo();
        }
    }
}