using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI.Game
{
    public class LevelProgressComponent : MonoBehaviour
    {
        [SerializeField] private Text levelCounter = null;
        [SerializeField] private Text levelName = null;
        [SerializeField] private Image[] stars = null;
        [SerializeField] private Color starOn = Color.yellow;
        [SerializeField] private Color starOff = Color.black;

        public void SetData(PlayerDataEntry entry, int levelNumber)
        {
            levelCounter.text = "Level " + (levelNumber + 1);
            levelName.text = entry.LevelName;
            for (int i = 0; i < stars.Length; i++)
            {
                if (i + 1 <= entry.Stars)
                {
                    stars[i].color = starOn;
                }
                else
                {
                    stars[i].color = starOff;
                }
            }
        }
    }
}