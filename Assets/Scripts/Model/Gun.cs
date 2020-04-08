using System;

namespace Geekbrains
{
	public sealed class Gun : Weapon
	{
        public override event Action OnAmmoEnd =delegate { };

        public override void Fire()
		{
			if (!_isReady) return;
			if (Clip.CountAmmunition <= 0)
			{
				OnAmmoEnd.Invoke();
			}
			else
			{
				var temAmmunition = Instantiate(Ammunition, _barrel.position, _barrel.rotation);//todo Pool object
				temAmmunition.AddForce(_barrel.forward * _force);
				Clip.CountAmmunition--;
				_isReady = false;
				Invoke(nameof(ReadyShoot), _rechergeTime);
			}
		}
	}
}