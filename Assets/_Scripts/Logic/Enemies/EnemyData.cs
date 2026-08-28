using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Rabbit Gun Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private EnemyType _type;
    [SerializeField] private string _enemyName;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private int _pointsAwarded;
    [SerializeField] private Color _enemyColour = Color.white; 

    // ----------------------------------------------

    // --- GETTERS ---
    public EnemyType Type => _type;
    public string EnemyName => _enemyName;
    public float MaxHealth => _maxHealth;
    public float MoveSpeed => _moveSpeed;
    public int PointsAwarded => _pointsAwarded;
    public Color EnemyColour => _enemyColour;
}
