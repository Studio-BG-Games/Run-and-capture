using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private Image levelImage;

        public Image LevelImage => levelImage;
    }
}
