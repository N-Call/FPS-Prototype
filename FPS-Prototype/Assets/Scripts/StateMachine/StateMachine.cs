using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public BaseState currentState;

    public bool NotEmptyState()
    {
        return this.currentState != null;
    }

    void Start()
    {
        this.currentState = this.GetFirstState();
        if (this.NotEmptyState()) 
        {
            this.currentState.Enter();
        }
    }

    void Update()
    {
        if (this.NotEmptyState())
        {
            this.currentState.StateLogic();
        }
    }

    private void LateUpdate()
    {
        if (this.NotEmptyState()) 
        {
            this.currentState.Action();
        }
    }

    public void ChangeState(BaseState state)
    {
        this.currentState.Exit();
        this.currentState = state;
        this.currentState.Enter();
    }

    public BaseState GetCurrentState()
    {
        return currentState;
    }

    protected virtual BaseState GetFirstState()
    {
        return null;
    }

}
