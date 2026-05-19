namespace AireIndustrial.Application.DTOs;

public record RegisterLecturaDto(
    Guid SensorId,
    double PM2_5,
    double PM10,
    double CO2,
    DateTime FechaHora
);
