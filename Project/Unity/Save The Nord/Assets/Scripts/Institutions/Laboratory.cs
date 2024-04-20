using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Institutions
{
    public class Laboratory : MonoBehaviour
    {
        [SerializeField] private GameObject _crystal;
        [SerializeField] private float _crystalMoveTime;
        [SerializeField] private Vector3 _crystalRotationAmount, _minCrystalPosition, _maxCrystalPosition;
        [SerializeField] private GameObject _recipePrefab;
        [SerializeField] private GameObject _recipesScrollContent;
        [SerializeField] private List<Institution.CraftRecipe> _recipes;
        
        private void Awake()
        {
            foreach (var recipe in _recipes)
            {
                var recipeItem = Instantiate(_recipePrefab, _recipesScrollContent.transform);
                recipeItem.GetComponent<Recipe>().UpdateInfo(recipe);
            }
        }

        private void Start()
        {
            StartCoroutine(RotateCrystal());
            StartCoroutine(MoveCrystal());
        }

        IEnumerator RotateCrystal()
        {
            while (true)
            {
                _crystal.transform.Rotate(_crystalRotationAmount * Time.deltaTime);
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        IEnumerator MoveCrystal()
        {
            float lerpPosition = 0;
            while (true)
            {
                while (lerpPosition < 1)
                {
                    lerpPosition += Time.deltaTime / _crystalMoveTime;
                    lerpPosition = Mathf.Clamp01(lerpPosition);
                    var t = NnUtils.EaseInOut(lerpPosition);
                    _crystal.transform.localPosition = Vector3.Lerp(_minCrystalPosition, _maxCrystalPosition, t);
                    yield return null;
                }
                while (lerpPosition > 0)
                {
                    lerpPosition -= Time.deltaTime / _crystalMoveTime;
                    lerpPosition = Mathf.Clamp01(lerpPosition);
                    var t = NnUtils.EaseInOut(lerpPosition);
                    _crystal.transform.localPosition = Vector3.Lerp(_minCrystalPosition, _maxCrystalPosition, t);
                    yield return null;
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
