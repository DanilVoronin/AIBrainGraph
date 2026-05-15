using System;
using UnityEngine;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Компоненты нод
    /// </summary>
    public abstract class AINodeComponent : AINode
    {
        protected AINodeComponent()
        {
        }

        /// <summary>
        /// Настройка нода
        /// </summary>
        /// <param name="brain">Мозги</param>
        /// <typeparam name="T">Тип компоненты</typeparam>
        public abstract void Setup(AIBrain brain, Type component);
    }
}