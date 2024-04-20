using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Institutions
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] private GameObject _gear;
        [SerializeField] private Vector3 _rotationAmount;
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
            StartCoroutine(RotateGear());
        }

        IEnumerator RotateGear()
        {
            while (true)
            {
                _gear.transform.Rotate(_rotationAmount * Time.deltaTime);
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}