using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Win : MonoBehaviour
    {
        [SerializeField] private UI _ui;
        [SerializeField] private List<GameObject> _groundPlantPrefabs, _waterPlantPrefabs;
        [SerializeField] private EndScreen _endScreen;
        
        public void SavedTheNord()
        {
            StartCoroutine(_ui.HideUISaved());
            Stats.SelectedObject = Stats.SelectedInfo = null;
            Stats.SelectedInteract = null;
            StartCoroutine(WinEffect());
        }

        IEnumerator WinEffect()
        {
            SoundManager.Instance.StopSound("GameLoop");
            SoundManager.Instance.PlaySound("Plant");
            foreach (var tile in Stats.GroundTiles)
            {
                var ts = tile.GetComponent<Tile>();
                if (ts.IsUsed) continue;
                var plant = Instantiate(_groundPlantPrefabs[Random.Range(0, _groundPlantPrefabs.Count)], ts.transform);
                var ps = plant.GetComponent<Plants.Plant>();
                ps.IsFertilized = true;
                ps.GrowthTime *= 0.1f;
                yield return new WaitForSeconds(0.01f);
            }
            SoundManager.Instance.PlaySound("WaterPlant");
            foreach (var tile in Stats.WaterTiles)
            {
                var ts = tile.GetComponent<Tile>();
                if (ts.IsUsed) continue;
                var plant = Instantiate(_waterPlantPrefabs[Random.Range(0, _waterPlantPrefabs.Count)], ts.transform);
                var ps = plant.GetComponent<Plants.Plant>();
                ps.IsFertilized = true;
                ps.GrowthTime *= 0.1f;
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(1);
            SoundManager.Instance.PlaySound("Win");
            _endScreen.Show();
            _endScreen.UpdateInfo(true);
        }
    }
}
