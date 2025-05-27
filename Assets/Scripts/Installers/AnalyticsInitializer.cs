using Ludo.Core.Analytics;
using Ludo.UnityInject;
using UnityEngine;

namespace Ludo.Demo.Installers
{
    public class AnalyticsInitializer : MonoBehaviour
    {
        [Inject] private IAnalyticsService _analyticsService;
        [Inject] private DebugProviderConfig _debugConfig;

        private void Awake()
        {
            // Create and register the debug provider
            var debugProvider = new DebugAnalyticsProvider();

            // Register providers
            _analyticsService.RegisterProvider(debugProvider, _debugConfig);

            // Initialize the service
            _analyticsService.Initialize();

            // Set default consent
            _analyticsService.SetConsent(true, null);
            
            Debug.Log("Analytics initialized!");
        }
    }
}