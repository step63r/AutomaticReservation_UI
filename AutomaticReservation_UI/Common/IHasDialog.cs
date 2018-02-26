namespace AutomaticReservation_UI.Common
{
    /// <summary>
    /// Dialogを持つViewModelのインタフェース
    /// </summary>
    public interface IHasDialog
    {
        DialogType DType { get; set; }
        void AcceptDialog();
        void CancelDialog();
    }
}
