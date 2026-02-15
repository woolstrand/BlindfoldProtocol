using System;
using BlindfoldProtocol.Runtime;
using UnityEngine;

namespace BlindfoldProtocol.App
{
    public interface IGameScreenRenderer
    {
        void Show();
        void Hide();
    }

    public interface IMenuScreenRenderer
    {
        void Show(Action onStart);
        void Hide();
    }

    public sealed class AppEntryPoint : MonoBehaviour
    {
        private IGameScreenRenderer _gameScreenRenderer;
        private IMenuScreenRenderer _menuScreenRenderer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (FindFirstObjectByType<AppEntryPoint>() != null)
            {
                return;
            }

            var bootstrapObject = new GameObject("AppEntryPoint");
            DontDestroyOnLoad(bootstrapObject);
            bootstrapObject.AddComponent<AppEntryPoint>();
        }

        private void Awake()
        {
            _gameScreenRenderer = new DotsGameScreenRenderer();
            _menuScreenRenderer = new MenuScreenRenderer();
        }

        private void Start()
        {
            ShowMenuScreen();

            // Quick switch: uncomment this block to skip menu and open game immediately.
            //ShowGameScreen();
            //return;
        }

        private void ShowMenuScreen()
        {
            _gameScreenRenderer.Hide();
            _menuScreenRenderer.Show(ShowGameScreen);
        }

        private void ShowGameScreen()
        {
            _menuScreenRenderer.Hide();
            _gameScreenRenderer.Show();
        }
    }
}
