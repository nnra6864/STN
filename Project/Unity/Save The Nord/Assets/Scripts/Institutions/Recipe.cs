using System;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Institutions
{
    public class Recipe : MonoBehaviour
    {
        private Institution.CraftRecipe _recipe;
        [SerializeField] private GameObject _ingredientPrefab, _ingredientScrollContent;
        [SerializeField] private TMP_Text _price, _airContamination, _waterContamination, _productName, _itemsLeft;
        [SerializeField] private Image _productImage;
        [SerializeField] private Button _craftButton;
        [HideInInspector] public bool CanCraft;
        [SerializeField] private string _craftSoundName;
        [Header("Craft Button Available Colors")]
        [SerializeField] private ColorBlock _availableColors;
        [Header("Craft Button Unavailable Colors")]
        [SerializeField] private ColorBlock _unavailableColors;
        
        public void UpdateInfo(Institution.CraftRecipe recipe)
        {
            _recipe = recipe;
            _price.text = $"${recipe.Price}";
            _airContamination.text = $"Air Contamination: {recipe.AirContamination}";
            _waterContamination.text = $"Water Contamination: {recipe.WaterContamination}";
            _productImage.sprite = recipe.Product.ItemSprite;
            _productName.text = recipe.Product.Name;
            UpdateAmount();
            _recipe.Product.OnAmountChanged += UpdateAmount;
            Stats.OnMoneyChanged += OnUpdatedStats;
            PopulateIngredients();
            OnUpdatedStats();
        }

        void UpdateAmount()
        {
            _itemsLeft.text = $"{_recipe.Product.Name} Left: {_recipe.Product.Amount}";
        }

        void OnUpdatedStats()
        {
            UpdateCanCraft();
            _craftButton.colors = CanCraft ? _availableColors : _unavailableColors;
        }

        void UpdateCanCraft()
        {
            if (Stats.Money < _recipe.Price)
            {
                CanCraft = false;
                return;
            }
            foreach (var ingredient in _recipe.Ingredients)
            {
                if (ingredient.Amount < 1)
                {
                    CanCraft = false;
                    return;
                }
            }
            CanCraft = true;
        }
        
        void PopulateIngredients()
        {
            foreach (var ingredient in _recipe.Ingredients)
            {
                var item = Instantiate(_ingredientPrefab, _ingredientScrollContent.transform);
                item.GetComponent<Ingredient>().UpdateInfo(ingredient);
                ingredient.OnAmountChanged += OnUpdatedStats;
            }
        }

        public void Craft()
        {
            if (!CanCraft)
            {
                SoundManager.Instance.PlaySound("Denied");
                return;
            }
            
            SoundManager.Instance.PlaySound(_craftSoundName);
            foreach (var ingredient in _recipe.Ingredients)
            {
                ingredient.Amount--;
            }
            
            Stats.Money -= _recipe.Price;
            Stats.AirPollutionLevel += _recipe.AirContamination;
            Stats.WaterPollutionLevel += _recipe.WaterContamination;
            _recipe.Product.Amount++;
        }
    }
}