using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    
    private AudioSource audio = null;
    private Button button = null;
    [SerializeField] private AudioClip buttonSelectSFX = null;
    [SerializeField] private AudioClip buttonClickSFX = null;

    private void Start()
    {
        this.button = GetComponent<Button>();
        this.audio = this.GetComponent<AudioSource>();
        this.button.onClick.AddListener(OnClick);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        this.audio.PlayOneShot(this.buttonSelectSFX);
    }

    public void OnClick()
    {
        this.audio.Stop();
        this.audio.PlayOneShot(this.buttonClickSFX);
    }
}