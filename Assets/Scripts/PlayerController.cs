using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnableUnits = null;
    [SerializeField] private List<Transform> spawnPoints = null;
    [SerializeField] private List<Unit> units = new List<Unit>();
    [SerializeField] private List<Unit> enemyUnits = new List<Unit>();
    private bool isCurrentPlayer = false;
    private Actions actions = null;
    private Unit selected = null;

    private void Start()
    {
        this.spawnPoints.ForEach((Transform t) => {
            GameObject obj = Instantiate(this.GetRandomUnit(), t.position, Quaternion.identity);
            this.units.Add(obj.GetComponent<Unit>());
        });

        GameManager.GetInstance().GetOtherPlayerController(this).SetEnemyUnits(this.units);
        this.actions = new Actions();
        this.RegisterInput();
    }

    private void OnDestroy()
    {
        this.UnregisterInput();
    }

    private void RegisterInput()
    {
        this.actions.Player.Primary.performed += Primary;
        this.actions.Player.Enable();
    }
    
    private void UnregisterInput()
    {
        this.actions.Player.Primary.performed -= Primary;
        this.actions.Player.Disable();
    }

    private void Primary(InputAction.CallbackContext context)
    {
        if (!this.isCurrentPlayer) return;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 selectedPosition = new Vector3(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), 0);
        Unit unit = this.units.Find(x => x.isSelectable() && x.transform.position == selectedPosition);

        // selecting our unit
        if (unit != null)
        {
            this.SelectUnit(unit);
            return;
        }

        // nothing selected
        if (!this.selected)
            return;
        
        // move
        if (this.selected.canMoveTo(selectedPosition))
        {
            this.selected.MoveTo(selectedPosition);
            return;
        }
        
        // attack?
        if (this.selected.canAttackPosition(selectedPosition))
        {
            Unit enemy = this.enemyUnits.Find(x => x.transform.position == selectedPosition);
            if (enemy)
                this.selected.Attack(enemy);
        }

        this.selected.Wait();
        this.selected = null;

        if (!this.units.Find(x => x.isSelectable()))
            GameManager.GetInstance().GetOtherPlayerController(this).BeginTurn();
    }

    private void SelectUnit(Unit unit)
    {
        this.selected = unit;
        unit.ToggleSelect(true);
    }

    public List<Unit> GetUnits()
    {
        return this.units;
    }

    public void SetEnemyUnits(List<Unit> enemyUnits)
    {
        this.enemyUnits = enemyUnits;
    }

    private GameObject GetRandomUnit()
    {
        int length = this.spawnableUnits.Length;
        int roll = Random.Range(1, length) - 1;
        return this.spawnableUnits[roll];
    }

    public void BeginTurn()
    {
        this.isCurrentPlayer = true;
        this.units.ForEach((Unit u) =>
        {
            u.Refresh();
        });
    }

    public void EndTurn()
    {
        this.isCurrentPlayer = false;
    }
}
