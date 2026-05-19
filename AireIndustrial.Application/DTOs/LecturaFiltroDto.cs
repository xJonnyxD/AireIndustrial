namespace AireIndustrial.Application.DTOs;

public record LecturaFiltroDto(
    DateTime FechaInicio,
    DateTime FechaFin,
    string? TipoContaminante
);
