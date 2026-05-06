namespace Tools.LayerMask
{
    public static class LayerMaskExtensions
    {
        // Расширение для удобства
        //Определяет содержит ли слой 
        public static bool Contains(this UnityEngine.LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}
