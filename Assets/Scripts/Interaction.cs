using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventCommand {
    public enum ECommandKind {
        TALK, GIVE, TAKE, CHECK, WAIT, STOP, MOVE, CALL, EAT
    }
    public string desc;
    public ECommandKind kind;
    public GameObject target;
    public string text;
    public int id;
    public float number;
    public Vector3 position;
    public Interaction fork;
    public UnityEvent action;
}

public class Interaction : MonoBehaviour
{
    public bool moveToTarget;
    public EventCommand[] commands;

    public IEnumerator ExecuteCommandList(EventCommand[] commands) {
        Interactable.Busy = true;
        // TODO: Move towards target
        if (moveToTarget) {
            PlayerInteraction.Instance.Move.MoveTo(transform.position);
            while (PlayerInteraction.Instance.Move.IsMoving) yield return null;
        }
        yield return null;
        int index = 0;
        while (index < commands.Length) {
            yield return ExecuteCommand(commands[index]);
            if (!Interactable.Busy) yield break;
            index++;
        }
        Interactable.Busy = false;
    }

    private IEnumerator ExecuteCommand(EventCommand comm) {
        switch (comm.kind) {
            case EventCommand.ECommandKind.TALK:
                Debug.Log(comm.text);
                MessageBox.Instance.ShowText(comm.target, comm.text, comm.number);
                while (MessageBox.MessageOpen) yield return null;
                break;
            case EventCommand.ECommandKind.GIVE:
                GameState.Instance.AddItem(comm.id, 1);
                break;
            case EventCommand.ECommandKind.TAKE:
                GameState.Instance.AddItem(comm.id, -1);
                break;
            case EventCommand.ECommandKind.CHECK:
                bool b = GameState.Instance.GetItemAmount(comm.id) > 0;
                if (b) {
                    yield return StartCoroutine(ExecuteCommandList(comm.fork.commands));
                }
                break;
            case EventCommand.ECommandKind.WAIT:
                yield return new WaitForSeconds(comm.number);
                break;
            case EventCommand.ECommandKind.STOP:
                Interactable.Busy = false;
                break;
            case EventCommand.ECommandKind.MOVE:
                PlayerInteraction.Instance.Move.MoveTo(comm.position);
                while (PlayerInteraction.Instance.Move.IsMoving) yield return null;
                break;
            case EventCommand.ECommandKind.CALL:
                comm.action.Invoke();
                while (GameState.Instance.UIOpen) yield return null;
                if (GameState.CallResult) {
                    GameState.CallResult = false;
                    if (comm.fork != null) {
                        yield return StartCoroutine(ExecuteCommandList(comm.fork.commands));
                    }
                }
                break;
            case EventCommand.ECommandKind.EAT:
                GameState.Instance.Eat();
                break;
        }
        yield return null;
    }
}
