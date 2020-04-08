using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Geekbrains
{
	public sealed class Inventory : IInitialization
	{
		private List<Weapon> _weapons = new List<Weapon>();
		private int _selectIndexWeapon;

		public List<Weapon> Weapons => _weapons;

		public FlashLightModel FlashLight { get; private set; }

		public void Initialization()
		{
			_weapons = ServiceLocatorMonoBehaviour.GetService<CharacterController>().
				GetComponentsInChildren<Weapon>().ToList();

			foreach (var weapon in Weapons)
			{
				weapon.IsVisible = false;
			}

			FlashLight = Object.FindObjectOfType<FlashLightModel>();
			FlashLight.Switch(FlashLightActiveType.Off);
		}

		public Weapon SelectWeapon(int weaponNumber)
		{
			if (weaponNumber < 0 || weaponNumber >= Weapons.Count) return null;

			var tempWeapon = Weapons[weaponNumber];
			return tempWeapon;
		}

		public Weapon SelectWeapon(MouseScrollWheel scrollWheel)
		{
			if (scrollWheel == MouseScrollWheel.Up)
			{
				if (_selectIndexWeapon < Weapons.Count - 1)
				{
					_selectIndexWeapon++;
				}
				else
				{
					_selectIndexWeapon = -1;
				}
				return SelectWeapon(_selectIndexWeapon);
			}

			if (_selectIndexWeapon <= 0)
			{
				_selectIndexWeapon = Weapons.Count;
			}
			else
			{
				_selectIndexWeapon--;
			}
			return SelectWeapon(_selectIndexWeapon);
		}

		public void AddWeapon(Weapon weapon)
		{
		}

		public void RemoveWeapon(Weapon weapon)
		{
		}
	}
}