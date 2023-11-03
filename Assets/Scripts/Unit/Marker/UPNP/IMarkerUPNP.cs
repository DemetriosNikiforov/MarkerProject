/// <summary>
/// Интерфейс полезной нагрузки Маркера
/// </summary>
public interface IMarkerUPNP
{
    #region Variables
    /// <summary>
    /// Вес полезной нагрузки
    /// </summary>
    public float Weight { get; protected set; }
    #endregion
    #region Custom methods
    /// <summary>
    /// Функция инициализации модуля
    /// </summary>
    public void Init();
    /// <summary>
    /// Включение модуля
    /// </summary>
    public void EnableUPNP();
    /// <summary>
    /// Отключение модуля
    /// </summary>
    public void DisavleUPNP();
    #endregion
}
