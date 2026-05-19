namespace AireIndustrial.Domain.Entities;

public class AlertaAire : BaseEntity
{
    public Guid SensorId { get; set; }
    public string Nivel { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public SensorCalidadAire Sensor { get; set; } = null!;
}
