using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class PopUpTrigger : MonoBehaviour
{
    public string m_head;
    public List<string> m_bodyControler;
    public List<string> m_bodyKeyboard;

    private void Start()
    {
        if (App.GetModule<LevelModule>().EventFlags.IsFlagsSet(GameEventFlags.Flags.shoot_unlocked))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject popup = Instantiate(Resources.Load<GameObject>("prefabs/UI prefabs/PopUp"));

        PopUpController popUpController = popup.GetComponentInChildren<PopUpController>();

        popUpController.m_head = m_head;
        popUpController.m_playerControlled = false;

        switch (App.GetModule<InputModule>().LastUsedDevice.displayName)
        {
            case "Keyboard":
                popUpController.m_body = m_bodyKeyboard;
                break;

            case "Mouse":
                popUpController.m_body = m_bodyKeyboard;
                break;

            case "Xbox Controller":
                popUpController.m_body = m_bodyControler;
                break;

            case "Wireless Controller":
                popUpController.m_body = m_bodyControler;
                break;
        }

        Destroy(gameObject);
    }
}