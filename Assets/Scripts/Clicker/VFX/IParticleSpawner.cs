using UnityEngine;

namespace Clicker.VFX
{
    public interface IParticleSpawner
    {
        public void Spawn(Vector2 position);
    }
}