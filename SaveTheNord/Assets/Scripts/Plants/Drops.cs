using System.Collections;
using System.Collections.Generic;
using Core;
using NnUtils.Scripts;
using UnityEngine;

namespace Plants
{
    public class DropItem
    {
        public InventoryItem Item;
        public int Amount;

        public DropItem(InventoryItem item, int amount)
        {
            Item = item;
            Amount = amount;
        }
    }
    public class Drops : MonoBehaviour
    {
        [SerializeField] private GameObject _dropPrefab;
        //TODO: Convert to stack
        [HideInInspector] public List<DropItem> DroppedItems = new();

        private void Awake()
        {
            StartCoroutine(ShowDrops());
        }

        IEnumerator ShowDrops()
        {
            while (true)
            {
                if (DroppedItems.Count < 1)
                {
                    yield return null;
                    continue;
                }

                var droppedItem = DroppedItems[0];
                DroppedItems.RemoveAt(0);
                yield return StartCoroutine(ShowDropRoutine(droppedItem));
            }
        }

        private Coroutine _showDropCoroutine;
        IEnumerator ShowDropRoutine(DropItem dropItem)
        {
            var ui = Instantiate(_dropPrefab, transform);
            ui.GetComponent<Drop>().UpdateInfo(dropItem);
            var rt = ui.GetComponent<RectTransform>();
            float lerpPos = 1;
            
            while (lerpPos > 0)
            {
                var t = Misc.ReverseLerpPos(ref lerpPos, 0.5f, easingType: Easings.Type.CubicInOut);
                rt.anchoredPosition = Vector2.Lerp(Vector2.zero, Vector2.up * 100, t);
                yield return null;
            }
            
            yield return new WaitForSeconds(1);
            
            while (lerpPos < 1)
            {
                var t = Misc.UpdateLerpPos(ref lerpPos, 0.5f, easingType: Easings.Type.CubicInOut);
                rt.anchoredPosition = Vector2.Lerp(Vector2.zero, Vector2.up * 100, t);
                yield return null;
            }
            
            Destroy(ui);
        }
    }
}
