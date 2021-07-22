using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    private bool _killPlayer = false;
    private bool _destroySelf = false;
    private PlayerEntity _playerRef = null;
    private Camera _camRef = null;
    [SerializeField] private SpriteRenderer _foreground = null;
    [SerializeField] private SpriteRenderer _background = null;
    [SerializeField] private SortingGroup _foregroundSG = null;
    [SerializeField] private SortingGroup _backgroundSG = null;
    [SerializeField] private GameObject _prefab = null;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if (_destroySelf)
        {
            _playerRef.GetComponent<SortingGroup>().name = "Midground";
            Destroy(_prefab.gameObject);
        }
        else if (_killPlayer)
        {
            KillPlayerAndResetPosition();
        }
        else
        {
            InitialiseDeathScreen();
        }
    }

    private void KillPlayerAndResetPosition()
    {
        blu.App.GetModule<blu.InputModule>().PlayerController.Enable();
        if (!_playerRef.MoveToRespawnLocation())
        {
            Debug.LogWarning("Player failed to respawn, reloading scene");
            blu.App.GetModule<blu.SceneModule>().SwitchScene(SceneManager.GetActiveScene().name, blu.TransitionType.Fade);
        }

        _destroySelf = true;
    }

    private void InitialiseDeathScreen()
    {
        DontDestroyOnLoad(_prefab);
        _camRef = Camera.main;
        blu.App.GetModule<blu.InputModule>().PlayerController.Disable();
        _playerRef = PlayerEntity.Instance;
        _playerRef.GetComponent<SortingGroup>().sortingLayerName = "World Space UI";
        _prefab.transform.position = new Vector3(_camRef.transform.localPosition.x, _camRef.transform.localPosition.y, 0);
        int order = _playerRef.GetComponent<SortingGroup>().sortingOrder;
        _foreground.transform.localScale = _camRef.OrthographicBounds().size;
        _background.transform.localScale = _camRef.OrthographicBounds().size;
        _foregroundSG.sortingOrder = order + 1;
        _backgroundSG.sortingOrder = order - 1;
        _killPlayer = true;
    }
}