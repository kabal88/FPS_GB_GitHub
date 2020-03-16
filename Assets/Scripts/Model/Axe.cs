using UnityEngine;

namespace Geekbrains
{
	public sealed class Axe : Weapon
	{
        [SerializeField] private float _damage = 10.0f;

        protected override void Start()
        {

        }

        public override void Fire()
		{
			if (!_isReady) return;
			_isReady = false;
			Invoke(nameof(ReadyShoot), _rechergeTime);
		}

        private void OnCollisionEnter(Collision collision)
        {
            CustomDebug.Log($"collision woth {collision}");

            var tempObj = collision.gameObject.GetComponent<ISetDamage>();

            if (tempObj != null)
            {
                tempObj.SetDamage(new InfoCollision(_damage, collision.contacts[0], collision.transform));
            }
        }
    }
}
