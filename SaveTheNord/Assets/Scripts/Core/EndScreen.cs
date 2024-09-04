using System.Collections;
using NnUtils.Scripts;
using UnityEngine;
using TMPro;

namespace Core
{
    public class EndScreen : MonoBehaviour
    {
        public GameObject Scaler;
        [SerializeField] TMP_Text
            _title, _retry, _playtime,
            _averagePollution, _highestPollution,
            _averageAirPollution, _highestAirPollution,
            _averageWaterPollution, _highestWaterPollution,
            _bushesPlanted, _bushesCut,
            _liliesPlanted, _liliesHarvested,
            _treesPlanted, _treesChopped;

        public void UpdateInfo(bool saved = false)
        {
            _title.text = saved ? "YOU HAVE SAVED THE NORD" : "YOU HAVE FAILED THE NORD";
            _title.color = saved ? new Color32(163, 190, 140, 255) : new Color32(191, 97, 106, 255);
            _retry.text = saved ? "Play Again" : "Retry";
            int seconds = ((int)Stats.TimerTime % 60);
            int minutes = ((int)Stats.TimerTime / 60);
            _playtime.text = $"Playtime: {minutes:00}:{seconds:00}";
            _averagePollution.text = $"Average Pollution: {(int)Stats.AveragePollution}%";
            _highestPollution.text = $"Highest Pollution: {(int)Stats.HighestPollution}%";
            _averageAirPollution.text = $"Average Air Pollution: {(int)Stats.AverageAirPollution}%";
            _highestAirPollution.text = $"Highest Air Pollution: {(int)Stats.HighestAirPollution}%";
            _averageWaterPollution.text = $"Average Water Pollution: {(int)Stats.AverageWaterPollution}%";
            _highestWaterPollution.text = $"Highest Water Pollution: {(int)Stats.HighestWaterPollution}%";
            _bushesPlanted.text = $"Bushes Planted: {Stats.BushesPlanted}";
            _bushesCut.text = $"Bushes Cut: {Stats.BushesDestroyed}";
            _liliesPlanted.text = $"Lilies Planted: {Stats.LiliesPlanted}";
            _liliesHarvested.text = $"Lilies Harvested: {Stats.LiliesDestroyed}";
            _treesPlanted.text = $"Trees Planted: {Stats.TreesPlanted}";
            _treesChopped.text = $"Trees Chopped: {Stats.TreesDestroyed}";
        }

        public void Show()
        {
            Scaler.SetActive(true);
            StartCoroutine(Transition());
        }

        IEnumerator Transition()
        {
            float lerpPosition = 0;
            Scaler.transform.localScale = Vector3.zero;
            
            while (lerpPosition < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPosition, 0.5f, easingType: Easings.Types.QuadInOut);
                Scaler.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
