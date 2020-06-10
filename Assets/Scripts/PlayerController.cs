using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerID = 0;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Color usedColor = Color.white;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject[] spawnableUnits = null;
    private bool isMyTurn = false;
    
    public void SpawnUnits()
    {
        this.spawnPoints.ForEach((Transform t) => {
            GameObject obj = Instantiate(this.GetRandomUnit(), t.position, Quaternion.identity);
            Unit unit = obj.GetComponent<Unit>();
            unit.SetPlayer(this);
            unit.SetColors(this.color, this.usedColor);
            GameManager.GetInstance().AddUnit(this, unit);
        });
    }

    private GameObject GetRandomUnit()
    {
        int length = this.spawnableUnits.Length;
        int roll = Random.Range(1, length) - 1;
        return this.spawnableUnits[roll];
    }

    public void BeginTurn()
    {
        this.isMyTurn = true;
        GameManager.GetInstance().GetPlayerUnits(this).ForEach(u => u.Refresh());
    }

    public void EndTurn()
    {
        this.isMyTurn = false;
    }
}
