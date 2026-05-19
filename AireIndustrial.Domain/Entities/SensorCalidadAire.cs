namespace AireIndustrial.Domain.Entities;

public class SensorCalidadAire : BaseEntity
{
    public string Ubicacion { get; set; } = string.Empty;
    public string TipoGas { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public ICollection<LecturaAire> Lecturas { get; set; } = new List<LecturaAire>();
    public ICollection<AlertaAire> Alertas { get; set; } = new List<AlertaAire>();
}
