using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

namespace blu
{
    public enum TransitionType
    {
        NONE = 0,
        LRSweep,
        Fade,
        Slice
    }

    public enum LoadingBarType
    {
        NONE = 0,
        BottomRightRadial,
        BottomBar
    }

    public class SceneModule : Module
    {
        private Animator _transitionAnimator;
        private GameObject _transitionPrefab;
        private GameObject _progressBarPrefab;
        private ProgressBar _progressBar;

        [HideInInspector] public float loadProgess = 0f;
        [HideInInspector] public bool switching = false;

        public void SwitchScene(string in_scene, TransitionType in_transitionType = TransitionType.NONE, LoadingBarType in_loadingBarType = LoadingBarType.NONE, bool test = false)
        {
            if (switching)
                return;
            else
                switching = true;

            float delay;

            InitiailizeTransitionValues(out delay, in_transitionType, in_loadingBarType);

            StartCoroutine(LoadLevel(in_scene, delay, test));
        }

        private void InitiailizeTransitionValues(out float in_delay, TransitionType in_transitionType, LoadingBarType in_loadingBarType)
        {
            InitLoadingBar(in_loadingBarType);
            in_delay = InitTransitionType(in_transitionType);
        }

        private float InitTransitionType(TransitionType in_transitionType)
        {
            float in_delay;
            switch (in_transitionType)
            {
                case TransitionType.NONE:
                    _transitionPrefab = null;
                    _transitionAnimator = null;
                    in_delay = 0f;
                    break;

                case TransitionType.LRSweep:
                    _transitionPrefab = Instantiate(Resources.Load<GameObject>("prefabs/SceneTransition_ L_R Sweep")); // instantiates the LR Sweep prefab canvas
                    DontDestroyOnLoad(_transitionPrefab); // stops the scene loader from destroying it after switch
                    _transitionAnimator = _transitionPrefab.GetComponentInChildren<Animator>();
                    in_delay = 0.5f;
                    break;

                case TransitionType.Fade:
                    _transitionPrefab = Instantiate(Resources.Load<GameObject>("prefabs/SceneTransition_ Fade To Black")); // instantiates the LR Sweep prefab canvas
                    DontDestroyOnLoad(_transitionPrefab); // stops the scene loader from destroying it after switch
                    _transitionAnimator = _transitionPrefab.GetComponentInChildren<Animator>();
                    in_delay = 0.5f;
                    break;

                case TransitionType.Slice:
                    _transitionPrefab = Instantiate(Resources.Load<GameObject>("prefabs/SceneTransition_ Slice")); // instantiates the LR Sweep prefab canvas
                    DontDestroyOnLoad(_transitionPrefab); // stops the scene loader from destroying it after switch
                    _transitionAnimator = _transitionPrefab.GetComponentInChildren<Animator>();
                    in_delay = 0.5f;
                    break;

                default:
                    _transitionPrefab = null;
                    _transitionAnimator = null;
                    in_delay = 0f;
                    break;
            }

            return in_delay;
        }

        private void InitLoadingBar(LoadingBarType in_loadingBarType)
        {
            switch (in_loadingBarType)
            {
                case LoadingBarType.NONE:
                    _progressBarPrefab = null;
                    _progressBar = null;
                    break;

                case LoadingBarType.BottomRightRadial:
                    _progressBarPrefab = Instantiate(Resources.Load<GameObject>("prefabs/SceneProgress_ Radial"));
                    DontDestroyOnLoad(_progressBarPrefab);
                    _progressBar = _progressBarPrefab.GetComponentInChildren<ProgressBar>();
                    break;

                case LoadingBarType.BottomBar:
                    _progressBarPrefab = Instantiate(Resources.Load<GameObject>("prefabs/SceneProgress_ Line"));
                    DontDestroyOnLoad(_progressBarPrefab);
                    _progressBar = _progressBarPrefab.GetComponentInChildren<ProgressBar>();
                    break;

                default:
                    _progressBarPrefab = null;
                    _progressBar = null;
                    break;
            }
        }

        private IEnumerator LoadLevel(string in_scene, float in_delay = 0f, bool test = false)
        {
            yield return new WaitForSeconds(in_delay); // allow for animation to trigger

            AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(in_scene); // begin async scene swap after intro if any

            if (sceneLoad == null) // catch any invalid scene switch calls
            {
                Debug.Log("[App/SceneModule]: Unable to load scene: " + in_scene);
                Debug.Log("[App/SceneModule]: Wrong or missing index?");
                if (_transitionPrefab)
                    Destroy(_transitionPrefab); // clean up the transition object and animation
                _transitionAnimator = null;
                if (_progressBarPrefab)
                    Destroy(_progressBarPrefab);
                _progressBar = null;
                yield break;
            }

            if (!test)
            {
                while (!sceneLoad.isDone) // report the progress of the scene load
                {
                    loadProgess = Mathf.Clamp01(sceneLoad.progress / 0.9f); // 0f - 0.9f -> 0f - 1f
                    if (_progressBar)
                        _progressBar.UpdateProgressBar(loadProgess);
                    yield return null;
                }
            }
            else
            {
                float currentTime = 0f; // Loading bar test code
                while (currentTime < 3f) // report the progress of the scene load
                {
                    currentTime += Time.deltaTime;
                    if (_progressBar)
                        _progressBar.UpdateProgressBar(currentTime / 3f);
                    yield return null;
                }
            }

            if (_transitionAnimator)
                _transitionAnimator.SetTrigger("Outro"); // begin the outro animation

            if (_progressBarPrefab)
                GameObject.Destroy(_progressBarPrefab); // clean up the loading bar prefab and reference
            _progressBar = null;
            yield return new WaitForSeconds(in_delay); // allow for animation to trigger

            if (_transitionPrefab)
                GameObject.Destroy(_transitionPrefab); // clean up the transition object and animation
            _transitionAnimator = null;

            switching = false;

            yield break;
        }

        public void Quit()
        {
            Debug.Log("[App]: Quitting");
            UnityEngine.Application.Quit();
        }

        public override void Initialize()
        {
            Debug.Log("[App]: Initializing scene module");
        }
    }
}