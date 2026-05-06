using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioJuggler : MonoBehaviour
{
    public AudioSource[] sources; // Assign 3 sources in Inspector
    public List<AudioClip> playlist;
    public float fadeDuration = 5f;

    private int activeSourceIdx = 0;
    private int nextClipIdx = 0;
    private bool isTransitioning = false;

    void Start()
    {
        // Start the first track
        sources[activeSourceIdx].clip = playlist[nextClipIdx];
        sources[activeSourceIdx].Play();
        nextClipIdx++;
    }

    void Update()
    {
        AudioSource current = sources[activeSourceIdx];

        // Trigger crossfade when current song is near the end
        if (
            !isTransitioning
            && current.isPlaying
            && current.time >= (current.clip.length - fadeDuration)
        )
        {
            StartCoroutine(CrossfadeToNext());
        }
    }

    IEnumerator CrossfadeToNext()
    {
        isTransitioning = true;

        // Round-robin selection: 0 -> 1 -> 2 -> 0
        int nextSourceIdx = (activeSourceIdx + 1) % sources.Length;
        AudioSource outgoing = sources[activeSourceIdx];
        AudioSource incoming = sources[nextSourceIdx];

        // Prepare incoming source
        incoming.clip = playlist[nextClipIdx % playlist.Count];
        incoming.volume = 0;
        incoming.Play();

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            outgoing.volume = Mathf.Lerp(1, 0, t);
            incoming.volume = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        outgoing.Stop();
        activeSourceIdx = nextSourceIdx;
        nextClipIdx++;
        isTransitioning = false;
    }
}
