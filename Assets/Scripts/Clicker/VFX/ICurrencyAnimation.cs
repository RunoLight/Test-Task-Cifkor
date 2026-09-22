using System;
using UnityEngine;

namespace Clicker.VFX
{
    public interface ICurrencyAnimation
    {
        public void Play(Vector2 startPosition, Action<ICurrencyAnimation> onComplete);
        public void Stop();
        
        public void Enable();
        public void Disable();
        public void Dispose();
    }
}