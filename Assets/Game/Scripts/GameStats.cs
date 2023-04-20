using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodymary.Game
{
    public class GameStats : MonoBehaviour
    {
        public string levelName { get; set; }
        public float gameTime { get; set; }

        private GameManager manager => GameManager.GManager;

        void Awake()
        {
            levelName = SceneManager.GetActiveScene().name;
        }

        void LateUpdate()
        {
            if (manager.checkVictory)
                gameTime += Time.deltaTime;
        }
    }
}

