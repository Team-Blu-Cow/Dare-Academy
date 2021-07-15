using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableEntityTrigger : MonoBehaviour
{
    private BoxCollider2D col2d;

    public void OnValidate()
    {
        col2d = GetComponent<BoxCollider2D>();
    }

    private bool hasPlayer = false;

    public bool HasPlayer => hasPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GridEntity ent = collision.gameObject.GetComponent<GridEntity>();
        if (ent.Flags.IsFlagsSet(GridEntityFlags.Flags.isPlayer))
        {
            hasPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GridEntity ent = collision.gameObject.GetComponent<GridEntity>();
        if (ent.Flags.IsFlagsSet(GridEntityFlags.Flags.isPlayer))
        {
            hasPlayer = false;
        }
    }
}