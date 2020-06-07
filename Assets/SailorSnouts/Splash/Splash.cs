using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class Splash : MonoBehaviour
{
    [SerializeField] private GameObject anyKeyPrompt = null;
    private VideoPlayer video = null;
        
    void Start()
    {
        this.video = GetComponent<VideoPlayer>();
        this.video.loopPointReached += DisplayPrompt;
    }

    private void OnDestroy()
    {
        this.video.loopPointReached += DisplayPrompt; 
    }

    private void DisplayPrompt(VideoPlayer video)
    {
        this.anyKeyPrompt.SetActive(true);
    }
}
