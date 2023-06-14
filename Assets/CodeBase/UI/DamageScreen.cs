using System;

namespace CodeBase.UI
{
    public class DamageScreen : SplashScreen
    {
        public static Action OnPlayerDamaged;

        private void OnEnable()
        {
            OnPlayerDamaged += ShowScreen;
        }

        private void OnDisable()
        {
            OnPlayerDamaged -= ShowScreen;
        }
    }
}
