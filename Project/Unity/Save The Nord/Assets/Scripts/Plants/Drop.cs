using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Plants
{
    public class Drop : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _title;

        public void UpdateInfo(DropItem drop)
        {
            _image.sprite = drop.Item.ItemSprite;
            _title.text = $"{drop.Item.Name} x{drop.Amount}";
        }
    }
}
