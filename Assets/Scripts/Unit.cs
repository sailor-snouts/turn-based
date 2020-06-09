using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth = 10f;
    [SerializeField] private Image healthFill = null;
    [Header("Attack")]
    [SerializeField] private float attack = 1f;
    [SerializeField] private float accuracy = 0.8f;
    [SerializeField] private float range = 1f;
    [Header("Movement")]
    [SerializeField] private float movement = 5f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool used = false;
    [Header("Other")]
    [SerializeField] private GameObject selected = null;

    private bool isAlive = true;
    private SpriteRenderer sprite = null;

    private void Start()
    {
        this.UpdateGui();
        this.currentHealth = this.maxHealth;
        this.sprite = GetComponent<SpriteRenderer>();
    }

    public void Refresh()
    {
        this.used = false;
    }
    
    public bool isSelectable()
    {
        return !this.used;
    }

    public void Wait()
    {
        Debug.Log("waiting");
        this.used = true;
        this.sprite.color = new Color(0.5f,0.5f,0.5f,1);
    }
    
    public bool canMoveTo(Vector2 position)
    {
        if (this.used) return false;
        Vector2 distance = (Vector2) this.transform.position - position;
        return distance.sqrMagnitude <= Mathf.Pow(this.movement, 2);
    }

    public void MoveTo(Vector2 target)
    {
        this.used = true;
        StartCoroutine(Move());
        
        IEnumerator Move()
        {
            while ((Vector2) transform.position != target)
            {
                transform.position = Vector2.MoveTowards(transform.transform.position, target, this.speed * Time.deltaTime);
                yield return null;
            }
            
            this.ToggleSelect(false);
        }
    }

    public bool canAttackPosition(Vector2 position)
    {
        Vector2 distance = (Vector2) this.transform.position - position;
        return distance.sqrMagnitude <= Mathf.Pow(this.range, 2);
    }

    public void Attack(Unit unit)
    {
        if (Random.Range(0f, 100f) > this.accuracy * 100f)
        {
            this.Missed();
            return;
        }

        unit.Damage(this.attack);
    }

    private void Missed()
    {
        
    }

    public void Damage(float dmg)
    {
        this.currentHealth = Mathf.Clamp(this.currentHealth - dmg, 0f, this.maxHealth);
        this.UpdateGui();
        if (this.currentHealth <= 0)
            this.Die();
    }

    private void UpdateGui()
    {
        this.healthFill.fillAmount = Mathf.Clamp01(this.currentHealth / this.maxHealth);
    }

    private void Die()
    {
        this.isAlive = false;
        this.Destroy();
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void ToggleSelect(bool selected)
    {
        this.selected.SetActive(selected);
    }
}
