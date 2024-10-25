using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerDatabase : ScriptableObject
{
    public List<PlayerStats> _players;
}
[Serializable]
public class PlayerStats
{
    public GameObject playerHolder;
    public int score;
    public float life;
    public float speed;
    public float armor;
}
