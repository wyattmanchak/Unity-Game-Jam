using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySound(AudioClip _sound, Transform spawnTransform, float volume, bool randomPitch)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = _sound;

        audioSource.volume = volume;

        if (randomPitch)
        {
            float Pitch = Random.Range(.85f, 1.15f);
            audioSource.pitch = Pitch;
        }

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSound(AudioClip[] _sound, Transform spawnTransform, float volume, bool randomPitch)
    {
        int rand = Random.Range(0, _sound.Length);

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = _sound[rand];

        audioSource.volume = volume;

        if (randomPitch)
        {
            float Pitch = Random.Range(.85f, 1.15f);
            audioSource.pitch = Pitch;
        }

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}
