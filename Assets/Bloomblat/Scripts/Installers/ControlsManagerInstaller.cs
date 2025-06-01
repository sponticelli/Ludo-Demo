using Bloomblat.Input;
using Bloomblat.UI;
using Ludo.UnityInject;
using UnityEngine;

namespace Bloomblat.Installers
{
    /// <summary>
    /// Installer for configuring which controls manager implementation to use.
    /// Allows platform-specific or configuration-based selection of input handling.
    /// </summary>
    [CreateAssetMenu(fileName = "ControlsManagerInstaller", menuName = "Bloomblat/Installers/Controls Manager Installer")]
    public class ControlsManagerInstaller : ScriptableObjectInstaller
    {
        [Header("Controls Manager Configuration")]
        [SerializeField] private ControlsManagerType controlsManagerType = ControlsManagerType.Auto;
        
        [Header("Mobile Controls Settings")]
        [SerializeField] private MobileControlsManager mobileControlsPrefab;
        
        
        public override void InstallBindings(IContainer container)
        {
            var selectedType = GetControlsManagerType();
            
            switch (selectedType)
            {
                case ControlsManagerType.MobileTouch:
                    BindMobileControlsManager(container);
                    break;
                    
                case ControlsManagerType.NewInputSystem:
                    BindNewInputSystemManager(container);
                    break;
                default:
                    Debug.LogWarning($"[ControlsManagerInstaller] Unsupported ControlsManagerType: {selectedType}. Fallback to MobileTouch.");
                    // Fallback to mobile controls
                    BindMobileControlsManager(container);
                    break;
            }
        }
        
        private ControlsManagerType GetControlsManagerType()
        {
            if (controlsManagerType != ControlsManagerType.Auto)
            {
                return controlsManagerType;
            }
            
            // Auto-detect based on platform
            #if UNITY_ANDROID || UNITY_IOS
                return ControlsManagerType.MobileTouch;
            #elif UNITY_STANDALONE || UNITY_WEBGL
                return ControlsManagerType.NewInputSystem;
            #else
                return ControlsManagerType.MobileTouch; // Default fallback
            #endif
        }
        
        private void BindMobileControlsManager(IContainer container)
        {
            if (mobileControlsPrefab == null)
            {
                CreateDefaultMobileControlsPrefab();
            }

            // Bind as singleton - the prefab will be instantiated by the container
            container.Bind<IControlsManager>()
                .To<MobileControlsManager>()
                .AsSingleton();
        }
        
        private void BindNewInputSystemManager(IContainer container)
        {
            container.Bind<IControlsManager>()
                .To<NewInputSystemControlsManager>()
                .AsSingleton();
        }
        
        private void CreateDefaultMobileControlsPrefab()
        {
            // Create a basic mobile controls prefab at runtime if none is provided
            var controlsObj = new GameObject("Mobile Controls");
            mobileControlsPrefab = controlsObj.AddComponent<MobileControlsManager>();
            
            // Add Canvas component
            var canvas = controlsObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            // Add GraphicRaycaster
            controlsObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        
    }
    
    /// <summary>
    /// Enum defining the available controls manager types.
    /// </summary>
    public enum ControlsManagerType
    {
        /// <summary>
        /// Automatically select based on platform.
        /// </summary>
        Auto,
        
        /// <summary>
        /// Use mobile touch controls with virtual joystick and buttons.
        /// </summary>
        MobileTouch,
        
        /// <summary>
        /// Use Unity's new Input System for keyboard, gamepad, and touch.
        /// </summary>
        NewInputSystem
    }
}