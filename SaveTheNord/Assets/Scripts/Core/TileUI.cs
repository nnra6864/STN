using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class TileUI : MonoBehaviour
    {
        [System.Serializable]
        public struct Seed
        {
            public InventoryItem Item;
            public GameObject PlantPrefab;
            public string PlantSoundName;
        }

        [SerializeField] private Tile.TileTypes _tileType;
        [SerializeField] private GameObject _plantItemPrefab;
        [SerializeField] private GameObject _scrollContent;
        [SerializeField] private List<Seed> _seeds;
        private List<Action> _plantFunctions = new();

        private void Awake()
        {
            _plantFunctions.Clear();
            foreach (var seed in _seeds)
            {
                var item = Instantiate(_plantItemPrefab, _scrollContent.transform);
                var script = item.GetComponent<Plants.PlantItem>();
                script.UpdateInfo(seed);
                _plantFunctions.Add(script.PlantSeeds);
            }
        }

        private void Update()
        {
            GetInput();
        }

        void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Plant(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) Plant(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) Plant(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) Plant(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) Plant(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) Plant(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7)) Plant(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8)) Plant(7);
        }

        void Plant(int index)
        {
            if (_plantFunctions.Count <= index) return;
            _plantFunctions[index]();
        }

        public void DeSelectTile()
        {
            Stats.SelectedTile = null;
        }
    }
}
