namespace AireIndustrial.Domain.Entities;

public class LecturaAire : BaseEntity
{
    public Guid SensorId { get; set; }
    public double PM2_5 { get; set; }
    public double PM10 { get; set; }
    public double CO2 { get; set; }
    public DateTime FechaHora { get; set; }
    public SensorCalidadAire Sensor { get; set; } = null!;
}
