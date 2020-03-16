using System.Collections.Generic;
using UnityEngine;

namespace Geekbrains
{
	public abstract class Weapon : BaseObjectScene
	{
		private int _maxCountAmmunition = 40;
		private int _minCountAmmunition = 20;
		private int _countClip = 5;
		public Ammunition Ammunition;
		public Clip Clip;

		public AmmunitionType[] AmmunitionTypes = {AmmunitionType.Bullet};

		[SerializeField] protected Transform _barrel;
		[SerializeField] protected float _force = 999;
		[SerializeField] protected float _rechergeTime = 0.2f;
		private Queue<Clip> _clips = new Queue<Clip>();

		protected bool _isReady = true;


		protected virtual void Start()

		{
			for (var i = 0; i <= _countClip; i++)
			{
				AddClip(new Clip { CountAmmunition = Random.Range(_minCountAmmunition, _maxCountAmmunition) });
			}
			
			ReloadClip();
		}

		public abstract void Fire();

		protected void ReadyShoot()
		{
			_isReady = true;
		}

		protected void AddClip(Clip clip)
		{
			_clips.Enqueue(clip);
		}

		public void ReloadClip()
		{
			if (CountClip <= 0) return;
			Clip = _clips.Dequeue();
		}

		public int CountClip => _clips.Count;
	}
}