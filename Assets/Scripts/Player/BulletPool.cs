using UnityEngine;

namespace RyanGQ.RunOrDie.Player
{
    public class BulletPool : MonoBehaviour
    {
        public Bullet[] Bullets = new Bullet[30];
        private int _nextBullet = 0;

        private void Start()
        {
            GameObject bulletPrefab = Resources.Load<GameObject>("Bullet");
            for (int i = 0; i < 30; i++)
            {
                Bullets[i] = Instantiate(bulletPrefab, transform).GetComponent<Bullet>();
                Bullets[i].gameObject.SetActive(false);
            }
        }

        public void FireBullet(Vector3 position, Vector3 direction, string player)
        {
            Bullets[_nextBullet].gameObject.SetActive(true);
            Bullets[_nextBullet].transform.position = position;
            Bullets[_nextBullet].transform.forward = direction;
            Bullets[_nextBullet].Body.AddForce(direction * 50f, ForceMode.Impulse);
            Bullets[_nextBullet].Fire(player);
            _nextBullet = _nextBullet == Bullets.Length - 1 ? 0 : (_nextBullet + 1);
        }
    }
}
