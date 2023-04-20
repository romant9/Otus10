using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Bloodymary.Game
{
    public class SpawnController : MonoBehaviour
    {
        private GameManager GManager => GameManager.GManager;
        private GameSettings GSettings => GameSettings.GSettings;

        public Transform CharactersGroup;
        public Transform EnemyPlaceGroup;
        private Queue<PlacePoint> EnemyPlacePoints = new Queue<PlacePoint>();

        public Transform PlayerPlaceGroup;

        public int waveNumber { get; private set; }
        public float timeInterval { get; set; }
        public int spawnCount { get; set; }

        private int characterCount;

        public bool isActive;
        public bool isLocal;

        private void Start()
        {
        }

        public void Initialize(int wave)
        {
            StartCoroutine(SpawnEnemy(wave));
        }

        public void ResetSpawn()
        {
            StopAllCoroutines();
            DeleteCharacters();
            GManager.AIOn = false;
        }

        private void DeleteCharacters()
        {
            var characters = GManager.Characters;

            if (characters.Count > 1)
            {
                foreach (var character in characters)
                {
                    if (character._characterType != GSettings.MyCharacterIs)
                    {
                        character.GetComponent<AiController>().isActive = false;
                        Destroy(character.gameObject);
                    }
                }
            }
        }
        private IEnumerator SpawnEnemy(int wave)
        {
            yield return new WaitUntil(() => isActive);
            waveNumber = wave;

            if (!isLocal)
            {
                spawnCount = GSettings.spawnCount;
                timeInterval = GSettings.spawnInterval;
            }
            else
            {
                if (spawnCount == 0) spawnCount = 1;
                if (timeInterval == 0) timeInterval = 1;
            }            
            
            characterCount = 0;

            isActive = false;

            DeleteCharacters();

            GManager.AIOn = true;
            GManager.PreInit();

            if (EnemyPlaceGroup && EnemyPlaceGroup.childCount > 0)
            {
                foreach (var place in EnemyPlaceGroup.GetComponentsInChildren<Transform>().Where(x => x.parent == EnemyPlaceGroup))
                {
                    EnemyPlacePoints.Enqueue(new PlacePoint() { _position = place.position, _place = place });
                }
            }

            var currentPlace = EnemyPlacePoints.First();

            while (characterCount < spawnCount)
            {
                if (!currentPlace._character)
                {
                    var enemy = Instantiate(EnemyType(GSettings.MyCharacterIs));
                    characterCount += 1;
                    enemy.name = enemy._characterType.ToString() + characterCount;
                    enemy.transform.position = currentPlace._position;
                    enemy.transform.parent = CharactersGroup;                  

                    currentPlace._character = enemy;
                    currentPlace = EnemyPlacePoints.Peek();
                }
                else
                {
                    if (Vector3.Distance(currentPlace._position, currentPlace._character.transform.position) > 1)
                    {
                        currentPlace._character = null;
                        yield return new WaitForSeconds(timeInterval);
                    }
                    else
                    {
                        var wrongPlace = EnemyPlacePoints.Dequeue();
                        currentPlace = EnemyPlacePoints.Peek();
                        EnemyPlacePoints.Enqueue(wrongPlace);
                    }

                }
                yield return null;

            }
        }


        public CharacterController EnemyType(GameSettings.CharacterType playerType)
        {
            if (playerType == GameSettings.CharacterType._Policeman) return GSettings._HooliganPrefab;
            else return GSettings._PolicemanPrefab;
        }
  
    }

    internal class PlacePoint
    {
        public Vector3 _position;
        public CharacterController _character;
        public Transform _place;
    }
}

