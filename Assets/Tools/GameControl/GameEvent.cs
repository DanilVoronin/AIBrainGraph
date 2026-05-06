namespace Tools.GameControl
{
    /// <summary>
    /// Игровое событие
    /// </summary>
    public struct GameEvent
    {
        public TypeGameEvent TypeGameEvent;
        static GameEvent e;

        public static void Trigger(TypeGameEvent typeGameEvent)
        {
            e.TypeGameEvent = typeGameEvent;

            MMEventManager.TriggerEvent(e);
        }
    }
    
    public enum TypeGameEvent
    {
        /// <summary>
        /// Загрузка данных игры
        /// </summary>
        LoadData,
        /// <summary>
        /// Сохранение данных игры
        /// </summary>
        SaveData            
    }
}
