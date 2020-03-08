using UnityEngine;

namespace Geekbrains
{
    public sealed class InputController : BaseController, IExecute
    {
        private KeyCode _activeFlashLight = KeyCode.F;
        private KeyCode _cancel = KeyCode.Escape;
        private KeyCode _reloadClip = KeyCode.R;
        private int _mouseButton = (int)MouseButton.LeftButton;

        public InputController()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
		
        public void Execute()
        {
            if (!IsActive) return;
            if (Input.GetKeyDown(_activeFlashLight))
            {
                ServiceLocator.Resolve<FlashLightController>().Switch(ServiceLocator.Resolve<Inventory>().FlashLight);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectWeapon(0);
            }
            else if (Input.GetKeyDown(_cancel))
            {
                ServiceLocator.Resolve<WeaponController>().Off();
                ServiceLocator.Resolve<FlashLightController>().Off();
            } 
            else if (Input.GetKeyDown(_reloadClip))
            {
                ServiceLocator.Resolve<WeaponController>().ReloadClip();
            }
            
            if (Input.GetMouseButton(_mouseButton))
            {
                if (ServiceLocator.Resolve<WeaponController>().IsActive)
                {
                    ServiceLocator.Resolve<WeaponController>().Fire();
                }
            }
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // todo manager
            {
                MouseScroll(MouseScrollWheel.Up);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                MouseScroll(MouseScrollWheel.Down);
            }
        }

        private void SelectWeapon(int value)
        {
            var tempWeapon = ServiceLocator.Resolve<Inventory>().SelectWeapon(value);
            SelectWeapon(tempWeapon);
        }

        private void MouseScroll(MouseScrollWheel value)
        {
            var tempWeapon = ServiceLocator.Resolve<Inventory>().SelectWeapon(value);
            SelectWeapon(tempWeapon);
        }

        private void SelectWeapon(Weapon weapon)
        {
            ServiceLocator.Resolve<WeaponController>().Off();
            if (weapon != null)
            {
                ServiceLocator.Resolve<WeaponController>().On(weapon);
            }
        }
    }
}
