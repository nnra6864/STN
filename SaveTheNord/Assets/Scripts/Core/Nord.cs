using System.Collections;
using System.Collections.Generic;
using NnUtils.Scripts;
using Plants;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Color = UnityEngine.Color;

namespace Core
{
    public class Nord : MonoBehaviour
    {
        [System.Serializable]
        public struct StartingPlant
        {
            public GameObject Prefab;
            [Tooltip("Both numbers are inclusive")]
            public Vector2 SpawnNumberRange;
        }
        
        
        private readonly List<Transform> _tiles = new();
        private int _generatedTiles;
        public bool IsDestroyed;
        
        [Header("Components")]
        [SerializeField] private ParticleSystem _fog;
        [SerializeField] private VolumeProfile _volumeProfile;
        [SerializeField] private EndScreen _endScreen;
        [SerializeField] private GameObject _groundTileUI, _waterTileUI;
        [SerializeField] private List<StartingPlant> _startingPlants;
        
        [Header("Generation")]
        [SerializeField] private float _timeBetweenTiles = 0.0025f;
        [SerializeField] private float _tileTransitionTime = 1;
        [SerializeField] private Easings.Type _tileTransitionEasing = Easings.Type.CubicOut;
        [SerializeField] private float _maxStartDistance = 5;
        [SerializeField] private float _maxStartRotation = 360;

        private void Awake()
        {
            _volumeProfile.TryGet<LensDistortion>(out var lens);
            lens.intensity.value = 0;
            GetTiles();
            Chaotic();
        }

        private void GetTiles()
        {
            _tiles.Clear();
            foreach (Transform tile in transform)
            {
                _tiles.Add(tile);
                var tileScript = tile.GetComponent<Tile>();

                if (tileScript.TileType == Tile.TileTypes.Ground)
                {
                    Stats.GroundTiles.Add(tile.gameObject);
                    tileScript.TileUI = _groundTileUI;
                }
                else
                {
                    Stats.WaterTiles.Add(tile.gameObject);
                    Stats.WaterTileRenderers.Add(tile.GetComponent<Renderer>());
                    tileScript.TileUI = _waterTileUI;
                }

                tile.gameObject.SetActive(false);
            }
            _generatedTiles = 0;
        }

        #region Generation

        private void OnFinishedGenerating()
        {
            _fog.Play();
            GenerateStartingPlants();
        }

        private IEnumerator GenerateTile(Transform tile)
        {
            float lerpPosition = 0;
            var originalPos = tile.position;
            var originalRot = tile.rotation;
            var originalScale = tile.localScale;

            tile.position = new(
                originalPos.x + _maxStartDistance * Misc.RandomInvert,
                originalPos.y + _maxStartDistance * Misc.RandomInvert,
                originalPos.z + _maxStartDistance * Misc.RandomInvert
                );
            
            tile.rotation = Quaternion.Euler(
                originalRot.x + _maxStartRotation * Misc.RandomInvert,
                originalRot.y + _maxStartRotation * Misc.RandomInvert,
                originalRot.z + _maxStartRotation * Misc.RandomInvert
                );
            
            tile.localScale = Vector3.zero;
        
            var startPos = tile.position;
            var startRot = tile.rotation;
            var startScale = tile.localScale;

            tile.gameObject.SetActive(true);
            while (lerpPosition < 1)
            {
                var t = Misc.Tween(ref lerpPosition, _tileTransitionTime, easingType: _tileTransitionEasing);
                tile.position = Vector3.Lerp(startPos, originalPos, t);
                tile.rotation = Quaternion.Lerp(startRot, originalRot, t);
                tile.localScale = Vector3.Lerp(startScale, originalScale, t);
            
                yield return null;
            }
            _generatedTiles++;
        }

        private void Chaotic()
        {
            var shufflesLeft = _tiles.Count;
            while (shufflesLeft > 1)
            {
                shufflesLeft--;
                var k = Random.Range(0, shufflesLeft+1);
                (_tiles[shufflesLeft], _tiles[k]) = (_tiles[k], _tiles[shufflesLeft]);
            }
            StartCoroutine(GenerateChaotic());
        }

        private IEnumerator GenerateChaotic()
        {
            foreach (var tile in _tiles)
            {
                StartCoroutine(GenerateTile(tile));
                yield return new WaitForSeconds(_timeBetweenTiles);
            }

            while (_generatedTiles < _tiles.Count) yield return null;
        
            OnFinishedGenerating();
        }

        private void GenerateStartingPlants()
        {
            foreach (var plant in _startingPlants)
            {
                for (var i = 0; i < Random.Range(plant.SpawnNumberRange.x, plant.SpawnNumberRange.y + 1); i++)
                {
                    var plantScript = plant.Prefab.GetComponent<Plant>();
                    var tiles = plantScript.TileType == Tile.TileTypes.Ground
                        ? Stats.GroundTiles
                        : Stats.WaterTiles;
                    var rand = Random.Range(0, tiles.Count);
                    while (tiles[rand].GetComponent<Tile>().IsUsed) rand = Random.Range(0, tiles.Count);
                    Instantiate(plant.Prefab, tiles[rand].transform);
                    tiles[rand].GetComponent<Tile>().IsUsed = true;
                }
            }
        }
        #endregion

        public void ExplodeNord()
        {
            if (IsDestroyed) return;
            Stats.SelectedTile = null; 
            Stats.SelectedInfo = Stats.SelectedObject = null;
            IsDestroyed = true;
           StartCoroutine(ExplodeCoroutine());
        }

        private IEnumerator ExplodeCoroutine()
        {
            float effectLerpPos = 0;
            _volumeProfile.TryGet<LensDistortion>(out var lens);
            var fogMat = _fog.GetComponent<Renderer>().material;
            var startingFogAlpha = fogMat.color.a;
            
            SoundManager.Instance.StopSound("GameLoop");
            yield return new WaitForSeconds(0.75f);
            _fog.Stop();
            Stats.OnNordExploded?.Invoke();
            SoundManager.Instance.PlaySound("NordExplosion");
            while (effectLerpPos < 1)
            {
                var t = Misc.Tween(ref effectLerpPos, 0.5f, easingType: Easings.Type.SineIn);
                lens.intensity.value = Mathf.Lerp(0, 0.25f, Easings.Ease(effectLerpPos, Easings.Type.SineOut));
                fogMat.color = new Color(fogMat.color.r, fogMat.color.g, fogMat.color.b, Mathf.Lerp(startingFogAlpha, 0f, t));
                _fog.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }
            effectLerpPos = 0;

            GetComponent<ParticleSystem>().Play();
            foreach (var tile in _tiles)
            {
                foreach (Transform child in tile)
                {
                    child.GetComponent<Rigidbody>().AddExplosionForce(10000f, Vector3.zero, 20);
                }
                tile.GetComponent<Rigidbody>().AddExplosionForce(10000f, Vector3.zero, 20);
            }

            while (effectLerpPos < 1)
            {
                var t = Misc.Tween(ref effectLerpPos, 0.5f, easingType: Easings.Type.SineOut);
                lens.intensity.value = Mathf.Lerp(0.25f, -0.5f, t);
                yield return null;
            }
            
            effectLerpPos = 0;
            while (effectLerpPos < 1)
            {
                var t = Misc.Tween(ref effectLerpPos, 2, easingType: Easings.Type.SineOut);
                lens.intensity.value = Mathf.Lerp(-0.5f, 0f, t);
                yield return null;
            }
            
            _endScreen.Show();
            _endScreen.UpdateInfo(false);
        }
    }
}
