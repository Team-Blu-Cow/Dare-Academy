using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScriptableEntity : GridEntity
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private ScriptedActionQueue m_actionQueue;
}