using UnityEngine;

namespace Geekbrains
{
    public sealed class Bullet : Ammunition
    {
        private void OnCollisionEnter(Collision collision)
        {
            // дописать доп урон
            var tempObj = collision.gameObject.GetComponent<ISetDamage>();

            if (tempObj != null)
            {
                tempObj.SetDamage(new InfoCollision(_curDamage, collision.contacts[0], collision.transform,
                    Rigidbody.velocity));
            }

            DestroyAmmunition();
        }
    }
}
