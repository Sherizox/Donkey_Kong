using UnityEngine;
using UnityEngine.UI;

public class GameAudioManager : MonoBehaviour
{
    public AudioSource princessAudio;
    public AudioSource warriorAudio;
    public AudioSource bgAudio1;
    public AudioSource bgAudio2;
    public AudioSource gameOverAudio;
    public AudioSource gameWinAudio;

    public Button musicSwitchButton;

    private bool bgMusicSwitched = false;
    private bool warriorPlayed = false;
    private bool bgStarted = false;

    private void Start()
    {
        if (musicSwitchButton != null)
            musicSwitchButton.onClick.AddListener(SwitchBackgroundMusic);

        PlayPrincessAudio();
    }

    private void Update()
    {
        if (GameManager.Instance.Cam2.activeSelf && !warriorPlayed)
        {
            PlayWarriorAudio();
        }

        if (warriorPlayed && !warriorAudio.isPlaying && !bgStarted)
        {
            bgAudio1.Play();
            bgStarted = true;
        }
    }

    void PlayPrincessAudio()
    {
        StopAllAudio();
        princessAudio.Play();
    }

    void PlayWarriorAudio()
    {
        StopAllAudio();
        warriorAudio.Play();
        warriorPlayed = true;
    }

    public void SwitchBackgroundMusic()
    {
        if (bgAudio1.isPlaying || bgAudio2.isPlaying)
        {
            bgAudio1.Stop();
            bgAudio2.Stop();
        }

        if (!bgMusicSwitched)
        {
            bgAudio2.Play();
        }
        else
        {
            bgAudio1.Play();
        }

        bgMusicSwitched = !bgMusicSwitched;
    }

    public void PlayGameOverAudio()
    {
        StopAllAudio();
        gameOverAudio.Play();
    }

    public void PlayGameWinAudio()
    {
        StopAllAudio();
        gameWinAudio.Play();
    }

    void StopAllAudio()
    {
        princessAudio.Stop();
        warriorAudio.Stop();
        bgAudio1.Stop();
        bgAudio2.Stop();
        gameOverAudio.Stop();
        gameWinAudio.Stop();
    }
}