/// <summary>
/// ��������� �������� �������� �������
/// </summary>
public interface IMarkerUPNP
{
    #region Variables
    /// <summary>
    /// ��� �������� ��������
    /// </summary>
    public float Weight { get; protected set; }
    #endregion
    #region Custom methods
    /// <summary>
    /// ������� ������������� ������
    /// </summary>
    public void Init();
    /// <summary>
    /// ��������� ������
    /// </summary>
    public void EnableUPNP();
    /// <summary>
    /// ���������� ������
    /// </summary>
    public void DisavleUPNP();
    #endregion
}
