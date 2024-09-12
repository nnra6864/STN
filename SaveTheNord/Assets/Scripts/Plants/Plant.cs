using System.Collections;
using System.Collections.Generic;
using Core;
using NnUtils.Scripts;
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
    
    public class Plant : NnBehaviour, IInteract
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
        protected Vector3 _startingPosition;
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
            _startingPosition = transform.position;
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
            _reducePollutionRoutine = StartCoroutine(ReducePollutionRoutine());
        }

        protected Coroutine _reducePollutionRoutine;
        protected IEnumerator ReducePollutionRoutine()
        {
            while (Health > 0)
            {
                Stats.AirPollutionLevel -= ((AirPollutionReduction / StartingHealth) * Health) * Time.deltaTime;
                Stats.WaterPollutionLevel -= ((WaterPollutionReduction / StartingHealth) * Health)  * Time.deltaTime;
                yield return null;
            }
        }

        protected Coroutine _engageWorkersRoutine;
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
                    if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
                    _shakeRoutine = StartCoroutine(ShakeRoutine());
                }
                else if (PlantType == PlantTypes.Bush)
                {
                    if (_shrinkRoutine != null) StopCoroutine(_shrinkRoutine);
                    _shrinkRoutine = StartCoroutine(ShrinkRoutine());
                }
                yield return new WaitForSeconds(PlantType == PlantTypes.Tree ? 0.6f : 0.4f);
            }
        }

        protected IEnumerator Die()
        {
            IsDestroyed = true;
            float lerpPosition = 0;
            
            foreach (var particle in DeathParticles)
                particle.Play();
            
            while (lerpPosition < 1)
            {
                var t = Misc.Tween(ref lerpPosition, 0.5f, easingType: Easings.Type.ExpoOut);
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return new WaitForEndOfFrame();
            }

            foreach (var drop in _plantDrops)
            {
                var amount = Random.Range(drop.DropAmountRange.x, drop.DropAmountRange.y + 1);
                if (amount == 0) continue;
                drop.Drop.Amount += amount;

                var existingItem = UI.Drops.DroppedItems.Find(x => x.Item == drop.Drop);
                if (existingItem != null) existingItem.Amount += amount;
                else UI.Drops.DroppedItems.Add(new DropItem(drop.Drop, amount));
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

        protected Coroutine _denyRoutine;
        protected IEnumerator DenyRoutine()
        {
            SoundManager.Instance.PlaySound("Denied");
            yield return new WaitForSeconds(0.25f);
            _denyRoutine = null;
        }
        
        protected Coroutine _fertilizeRoutine;
        protected IEnumerator FertilizeRoutine()
        {
            SoundManager.Instance.PlaySound("Fertilize");
            FertilizeParticles.Play();
            IsFertilized = true;
            GrowthTime *= 0.5f;
            Hotbar.FertilizerCount--;
            yield return new WaitForSeconds(0.25f);
            _fertilizeRoutine = null;
        }

        protected Coroutine _startWorkersRoutine;
        protected IEnumerator StartWorkersRoutine()
        {
            if (Hotbar.WorkersCount < 1)
            {
                StartNullRoutine(ref _denyRoutine, DenyRoutine());
                yield break;
            }
            
            SoundManager.Instance.PlaySound("EngageWorkers");
            if (_engageWorkersRoutine == null) _engageWorkersRoutine = StartCoroutine(EngageWorkers());
            else if (_denyRoutine == null) _denyRoutine = StartCoroutine(DenyRoutine());
            
            yield return new WaitForSeconds(0.25f);
            _startWorkersRoutine = null;
        }
        
        public void ToggleInfo()
        {
            if (_info.activeSelf)
                Stats.SelectedInfo = null;
            else
                Stats.SelectedInfo = _info;
            SoundManager.Instance.PlaySound("Select");
        }

        protected Coroutine _shakeRoutine;
        protected IEnumerator ShakeRoutine()
        {
            float lerpPosition = 0;
            var startingPosition = transform.position;
            var targetPosition = new Vector3(_startingPosition.x + Random.Range(-0.05f, 0.05f), _startingPosition.y, _startingPosition.z + Random.Range(-0.05f, 0.05f));
            while (lerpPosition < 1)
            {
                var t = Misc.Tween(ref lerpPosition, 0.05f, easingType: Easings.Type.SineOut);
                transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
                yield return null;
            }
            while (lerpPosition > 0)
            {
                var t = Misc.ReverseTween(ref lerpPosition, 0.05f, easingType: Easings.Type.SineIn);
                transform.position = Vector3.Lerp(_startingPosition, targetPosition, t);
                yield return null;
            }
            _shakeRoutine = null;
        }

        protected Coroutine _shrinkRoutine;
        protected IEnumerator ShrinkRoutine()
        {
            float lerpPosition = 0;
            var startingScale = transform.localScale;
            var rand = Random.Range(0.9f, 1.1f);
            var targetScale = new Vector3(rand, rand, rand);
            while (lerpPosition < 1)
            {
                var t = Misc.Tween(ref lerpPosition, 0.1f, easingType: Easings.Type.SineIn);
                transform.localScale = Vector3.Lerp(startingScale, targetScale, t);
                yield return null;
            }
            
            while (lerpPosition > 0)
            {
                var t = Misc.ReverseTween(ref lerpPosition, 0.1f, easingType: Easings.Type.SineOut);
                transform.localScale = Vector3.Lerp(Vector3.one, targetScale, t);
                yield return null;
            }
            _shakeRoutine = null;
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