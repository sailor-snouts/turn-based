using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    private PlayerController playerController = null;
    private Rigidbody2D rb = null;
    private Health health = null;
    
    [Header("Selection")]
    [SerializeField] private GameObject select = null;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Color usedColor = Color.gray;

    [Header("Attack")]
    [SerializeField] private float attack = 10;
    [SerializeField] private float acc = 0.8f;
    [SerializeField] private float crit = 0.1f;
    [SerializeField] private float range = 1;
    private bool hasAttacked = false;
    private Unit AttackTarget = null;
    private Element element;
    
    [Header("Movement")] 
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 4f;
    private bool hasMoved = false;
    private int currentWaypoint = 0;
    private Path path = null;
    private Seeker seeker = null;
    
    
    public PlayerController GetPlayer()
    {
        return this.playerController;
    }
    
    public void SetPlayer(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    
    public bool IsPlayer(PlayerController playerController)
    {
        return playerController == this.playerController;
    }

    public Element.ELEMENT GetElement()
    {
        return this.element.GetElement();
    }

    public Health GetHealth()
    {
        return this.health;
    }
    
    private void Start()
    {
        this.seeker = GetComponent<Seeker>();
        this.rb = GetComponent<Rigidbody2D>();
        this.element = GetComponent<Element>();
        this.health = GetComponent<Health>();
        this.health.death += this.Die;
        this.UpdateGraphs();
        if(this.hasMoved && this.hasAttacked)
            this.Rest();
        else this.Refresh();
    }

    public bool IsSelectable(PlayerController playerController)
    {
        return !this.hasMoved && this.IsPlayer(playerController);
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
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
    }

    public void Deselect()
    {
        this.select.SetActive(false);
        this.gameObject.layer = LayerMask.NameToLayer("Unit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
    }

    public void Die()
    {
        this.Destroy();
    }

    private void Destroy()
    {
        GameManager.GetInstance().RemoveUnit(this.playerController, this);
        this.health.death -= this.Die;
        Destroy(this.gameObject);
    }

    public void MoveTo(Vector3 target)
    {
        seeker.StartPath(this.transform.position, target, OnFinishPathCalculation);
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
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            sprite.color = this.color;
    }
    
    public void Rest()
    {
        this.hasMoved = true;
        this.hasAttacked = true;
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            sprite.color = this.usedColor;
    }

    private void OnFinishPathCalculation(Path p)
    {
        if (p.error)
        {
            Debug.LogError(p.error.ToString());
            return;
        }

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
        if (p.GetTotalLength() > this.range)
            return;
        
        bool hit = Random.Range(0, 100) / 100 <= this.acc;
        if (!hit)
            return;
        
        float dmg = this.attack;
        if (this.element.IsWeakAgainst(this.GetElement(), this.AttackTarget.GetElement()))
        {
            dmg *= 0.5f;
        }
        else
        {
            if (this.element.IsStrongAgainst(this.GetElement(), this.AttackTarget.GetElement()))
                dmg *= 1.5f;
            bool crit = Random.Range(0, 100) / 100 <= this.crit;
            if (crit)
                dmg *= 1.5f;
        }
        
        this.AttackTarget.GetHealth().Damage(dmg);
        this.hasAttacked = true;
        this.hasMoved = true;
        this.Deselect();
        this.Rest();
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
