using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    private enum State
    {
        idle,
        moving,
        dead
    }
    private State state;
    public bool IsMoving = false, isDead = false;
    public float Health = 100, TimeToRevive, TimeToMove;
    private float currentHealth, currentTimeToRevive;


    private void Awake()
    {
        state = State.idle; 
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = Health;
        currentTimeToRevive = TimeToRevive;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) 
        {
            case State.idle:
                if (currentHealth <= 0)
                {
                    Die();
                }
                break;

            case State.moving:
                if (currentHealth <= 0)
                {
                    Die();
                }
                break;

            case State.dead:
                if (currentTimeToRevive <= 0)
                {
                    currentHealth = Health;
                    currentTimeToRevive = TimeToRevive;
                    state = State.idle;
                }
                else
                {
                    currentTimeToRevive -= Time.deltaTime;
                }
                break;  
        }
    }

    private void Die()
    {
        isDead = true;
        state = State.dead;
    }
}
