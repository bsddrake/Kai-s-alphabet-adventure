using UnityEngine;
using System.Collections.Generic;
using KaiAlphabetAdventure.Utils;

namespace KaiAlphabetAdventure.Letters
{
    /// <summary>
    /// Spawns letters in the game world and manages letter object pooling.
    /// Supports: REQ-1.2.6, REQ-2.4.3
    /// </summary>
    public class LetterSpawner : MonoBehaviour
    {
        public static LetterSpawner Instance { get; private set; }

        [Header("Spawning Settings")]
        [SerializeField] private GameObject letterPrefab;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxActiveLetters = 15;

        [Header("Spawn Area")]
        [SerializeField] private Vector2 spawnAreaMin = new Vector2(-20f, 10f);
        [SerializeField] private Vector2 spawnAreaMax = new Vector2(20f, 15f);

        [Header("Letter Sprites")]
        [SerializeField] private Sprite[] letterSprites = new Sprite[26]; // A-Z

        private ObjectPool<Letter> letterPool;
        private List<Letter> activeLetters = new List<Letter>();
        private float spawnTimer = 0f;
        private bool isSpawning = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            InitializeLetterPool();
        }

        private void Update()
        {
            if (!isSpawning) return;

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                SpawnRandomLetter();
            }
        }

        private void InitializeLetterPool()
        {
            if (letterPrefab == null)
            {
                Debug.LogError("Letter prefab not assigned to LetterSpawner!");
                return;
            }

            letterPool = new ObjectPool<Letter>(
                createFunc: CreateLetter,
                onGet: OnLetterGet,
                onRelease: OnLetterRelease,
                initialSize: 20
            );
        }

        private Letter CreateLetter()
        {
            GameObject letterObj = Instantiate(letterPrefab, transform);
            letterObj.SetActive(false);
            return letterObj.GetComponent<Letter>();
        }

        private void OnLetterGet(Letter letter)
        {
            letter.gameObject.SetActive(true);
            letter.ResetLetter();
        }

        private void OnLetterRelease(Letter letter)
        {
            letter.gameObject.SetActive(false);
        }

        public void StartSpawning()
        {
            isSpawning = true;
            spawnTimer = 0f;
        }

        public void StopSpawning()
        {
            isSpawning = false;
        }

        public void SpawnRandomLetter()
        {
            if (activeLetters.Count >= maxActiveLetters)
            {
                return; // Too many letters already
            }

            // Random letter A-Z
            char randomLetter = (char)Random.Range('A', 'Z' + 1);
            Vector3 spawnPosition = GetRandomSpawnPosition();

            SpawnLetter(randomLetter, spawnPosition);
        }

        public Letter SpawnLetter(char letter, Vector3 position)
        {
            Letter letterObj = letterPool.Get();
            if (letterObj == null)
            {
                Debug.LogWarning("Failed to get letter from pool!");
                return null;
            }

            letterObj.SetLetter(letter);
            letterObj.transform.position = position;

            // Set sprite if available
            SetLetterSprite(letterObj, letter);

            activeLetters.Add(letterObj);

            return letterObj;
        }

        public void DropLetterAtPosition(char letter, Vector3 position, Vector2 dropDirection)
        {
            Letter letterObj = SpawnLetter(letter, position);
            if (letterObj != null)
            {
                letterObj.DropAtPosition(position, dropDirection);
            }
        }

        private void SetLetterSprite(Letter letter, char character)
        {
            int index = character - 'A';
            if (index >= 0 && index < letterSprites.Length && letterSprites[index] != null)
            {
                SpriteRenderer sr = letter.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = letterSprites[index];
                }
            }
        }

        public void ReturnLetterToPool(Letter letter)
        {
            activeLetters.Remove(letter);
            letterPool.Release(letter);
        }

        private Vector3 GetRandomSpawnPosition()
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            return new Vector3(x, y, 0f);
        }

        public void ClearAllLetters()
        {
            foreach (Letter letter in new List<Letter>(activeLetters))
            {
                ReturnLetterToPool(letter);
            }
            activeLetters.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((spawnAreaMin.x + spawnAreaMax.x) / 2f, (spawnAreaMin.y + spawnAreaMax.y) / 2f, 0f);
            Vector3 size = new Vector3(spawnAreaMax.x - spawnAreaMin.x, spawnAreaMax.y - spawnAreaMin.y, 0.1f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
