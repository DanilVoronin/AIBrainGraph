using UnityEngine;

namespace Brain
{
    /// <summary>
    /// Пемечает объект как цель для AI
    /// </summary>
    public interface ITarget
    {
        public GameObject GameObject { get; }
        public Vector3 PointTarget { get; }
    }
}
