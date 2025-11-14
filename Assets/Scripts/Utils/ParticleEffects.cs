using UnityEngine;

namespace KaiAlphabetAdventure.Utils
{
    /// <summary>
    /// Manages particle effects for visual feedback.
    /// Supports: REQ-3.3.1, game feel
    /// </summary>
    public class ParticleEffects : MonoBehaviour
    {
        public static ParticleEffects Instance { get; private set; }

        [Header("Particle Prefabs")]
        [SerializeField] private GameObject letterCollectParticles;
        [SerializeField] private GameObject wordCompleteParticles;
        [SerializeField] private GameObject npcCompleteParticles;
        [SerializeField] private GameObject letterDropParticles;

        [Header("Settings")]
        [SerializeField] private bool useObjectPooling = true;
        [SerializeField] private int poolSize = 10;

        private ObjectPool<ParticleSystem> particlePool;

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
            if (useObjectPooling)
            {
                InitializePool();
            }
        }

        private void InitializePool()
        {
            if (letterCollectParticles == null) return;

            particlePool = new ObjectPool<ParticleSystem>(
                createFunc: CreateParticleSystem,
                onGet: OnParticleGet,
                onRelease: OnParticleRelease,
                initialSize: poolSize
            );
        }

        private ParticleSystem CreateParticleSystem()
        {
            if (letterCollectParticles == null) return null;

            GameObject obj = Instantiate(letterCollectParticles, transform);
            obj.SetActive(false);
            return obj.GetComponent<ParticleSystem>();
        }

        private void OnParticleGet(ParticleSystem ps)
        {
            if (ps != null)
            {
                ps.gameObject.SetActive(true);
                ps.Clear();
            }
        }

        private void OnParticleRelease(ParticleSystem ps)
        {
            if (ps != null)
            {
                ps.Stop();
                ps.gameObject.SetActive(false);
            }
        }

        public void PlayLetterCollectEffect(Vector3 position)
        {
            PlayEffect(letterCollectParticles, position);
        }

        public void PlayWordCompleteEffect(Vector3 position)
        {
            PlayEffect(wordCompleteParticles, position);
        }

        public void PlayNPCCompleteEffect(Vector3 position)
        {
            PlayEffect(npcCompleteParticles, position);
        }

        public void PlayLetterDropEffect(Vector3 position)
        {
            PlayEffect(letterDropParticles, position);
        }

        private void PlayEffect(GameObject effectPrefab, Vector3 position)
        {
            if (effectPrefab == null) return;

            if (useObjectPooling && particlePool != null)
            {
                ParticleSystem ps = particlePool.Get();
                if (ps != null)
                {
                    ps.transform.position = position;
                    ps.Play();
                    StartCoroutine(ReturnToPoolAfterPlay(ps));
                }
            }
            else
            {
                GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
                Destroy(effect, 3f);
            }
        }

        private System.Collections.IEnumerator ReturnToPoolAfterPlay(ParticleSystem ps)
        {
            yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);
            particlePool?.Release(ps);
        }
    }
}
