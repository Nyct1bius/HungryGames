using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    private enum State
    {
        Idle,
        Moving
    }
    private State state;

    public bool IsMoving = false;

    private void Awake()
    {
        state = State.Idle; 
    }

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) 
        {
            case State.Idle:
                break;

            case State.Moving:
                break;
        }
    }
}
