namespace Brain.MeshAgent.Models
{
    /// <summary>
    /// Состояния агента
    /// </summary>
    public enum AgentState
    {
        Idle,      // Стоит на месте
        Moving,    // В пути
        Arrived    // Достиг цели
    }
}