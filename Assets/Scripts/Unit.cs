using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    private PlayerController playerID = null;
    private Rigidbody2D rb = null;
    private SpriteRenderer sprite = null;
    private GameManager gm = null;

    [Header("Selection")]
    [SerializeField] private GameObject select = null;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Color usedColor = Color.gray;

    [Header("Health")] 
    [SerializeField] private Image healthBar = null;
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth = 10f;
    
    [Header("Attack")] 
    [SerializeField] private float attack = 10;
    [SerializeField] private float range = 1;
    private bool hasAttacked = false;
    private Unit AttackTarget = null;
    
    [Header("Movement")] 
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 4f;
    private bool hasMoved = false;
    private int currentWaypoint = 0;
    private Path path = null;
    private Seeker seeker = null;
    

    private void Start()
    {
        this.seeker = GetComponent<Seeker>();
        this.rb = GetComponent<Rigidbody2D>();
        this.sprite = GetComponent<SpriteRenderer>();
        this.gm = GameManager.GetInstance();
        this.UpdateGraphs();
        if(this.hasMoved && this.hasAttacked)
            this.Rest();
        else this.Refresh();
    }

    public void SetPlayer(PlayerController playerController)
    {
        this.playerID = playerController;
    }

    public void SetColors(Color color, Color usedColor)
    {
        this.color = color;
        this.usedColor = usedColor;
    }
    
    public bool IsSelectable(PlayerController playerController)
    {
        return !this.hasMoved && this.IsPlayer(playerController);
    }

    public bool IsPlayer(PlayerController playerController)
    {
        return playerController == this.playerID;
    }

    public bool HasMoved()
    {
        return this.hasMoved;
    }

    public bool HasAttacked()
    {
        return this.hasAttacked;
    }

    public void Select()
    {
        this.select.SetActive(true);
        this.gameObject.layer = LayerMask.NameToLayer("SelectedUnit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<SpriteRenderer>().bounds));
    }

    public void Deselect()
    {
        this.select.SetActive(false);
        this.gameObject.layer = LayerMask.NameToLayer("Unit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<SpriteRenderer>().bounds));
    }

    public void Die()
    {
        this.Destroy();
    }

    public void Destroy()
    {
        GameManager.GetInstance().RemoveUnit(this.playerID, this);
        Destroy(this.gameObject);
    }

    public void MoveTo(Vector3 target)
    {
        seeker.StartPath(this.transform.position, target, OnFinishPathCalculation);
    }

    private void UpdateHealth()
    {
        this.healthBar.fillAmount = Mathf.Clamp01(this.currentHealth / this.maxHealth);
    }

    public void Damage(float dmg)
    {
        this.currentHealth = Mathf.Clamp(this.currentHealth - dmg, 0, this.maxHealth);
        this.UpdateHealth();
        if (this.currentHealth <= 0)
            this.Die();
    }
    
    public void Attack(Unit unit)
    {
        this.AttackTarget = unit;
        seeker.StartPath(this.transform.position, unit.transform.position, OnFinishAttackPathCalculation);
    }

    public void Refresh()
    {
        this.hasMoved = false;
        this.hasAttacked = false;
        this.sprite.color = this.color;
    }
    
    public void Rest()
    {
        this.hasMoved = true;
        this.hasAttacked = true;
        this.sprite.color = this.usedColor;
    }

    private void OnFinishPathCalculation(Path p)
    {
        if (p.error)
        {
            Debug.LogError(p.error.ToString());
            return;
        }

        Debug.Log(p.GetTotalLength());
        if (p.GetTotalLength() <= this.distance)
        {
            this.hasMoved = true;
            this.path = p;
            this.currentWaypoint = 0;
        }
    }
    
    private void OnFinishAttackPathCalculation(Path p)
    {
        if (p.error)
        {
            Debug.LogError(p.error.ToString());
            return;
        }

        Debug.Log(p.GetTotalLength());
        if (p.GetTotalLength() <= this.range)
        {
            this.AttackTarget.Damage(this.attack);
            this.hasAttacked = true;
            this.hasMoved = true;
            this.Deselect();
            this.Rest();
        }
    }
    
    private void FixedUpdate()
    {
        if (this.path == null)
        {
            return;
        }

        if (this.currentWaypoint >= this.path.vectorPath.Count)
        {
            this.UpdateGraphs();
            this.rb.velocity = Vector2.zero;
            this.path = null;
            return;
        }

        Vector2 direction = (path.vectorPath[this.currentWaypoint] - this.transform.position).normalized;
        this.rb.velocity = direction * this.speed;

        if (Vector2.Distance(this.transform.position , path.vectorPath[this.currentWaypoint]) < 0.1f)
        {
            this.currentWaypoint++;
        }
    }

    private void UpdateGraphs()
    {
        AstarPath.active.Scan();
    }
}
