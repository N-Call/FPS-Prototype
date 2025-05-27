using UnityEngine;

public class BaseState
{
    public string name;
    public StateMachine stateMachine;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void StateLogic() { }
    public virtual void Action() { }
    public virtual void Exit() { }
}
