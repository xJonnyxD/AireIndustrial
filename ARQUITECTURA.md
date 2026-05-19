# Documento Técnico - AireIndustrial API

**Versión**: 1.0
**Fecha**: 18 de mayo de 2026
**Proyecto**: API Distribuida para Monitoreo de Calidad del Aire
**Autor**: Jonny Quintanilla
**CIF**: 2025010290

---

## 📐 1. Arquitectura de la solución

### 1.1 Patrón: Clean Architecture

La solución implementa **Clean Architecture** en 4 capas independientes:

```
┌─────────────────────────────────────┐
│  AireIndustrial (API/Presentation)  │
│         Controllers, Program         │
└────────────────┬────────────────────┘
                 │ depende de
┌────────────────▼────────────────────┐
│  AireIndustrial.Application         │
│    UseCases, DTOs, Interfaces       │
└────────────────┬────────────────────┘
                 │ depende de
┌────────────────▼────────────────────┐
│  AireIndustrial.Domain              │
│    Entities, Interfaces (contracts) │
└────────────────┬────────────────────┘
                 │ implementada por
┌────────────────▼────────────────────┐
│  AireIndustrial.Infrastructure      │
│  DbContext, Repositories, Services  │
└─────────────────────────────────────┘
```

### 1.2 Dependencias

| Capa | Referencia | Razón |
|------|-----------|-------|
| **API** | Application, Infrastructure | Orquesta la ejecución |
| **Application** | Domain | Usa interfaces y entidades |
| **Domain** | Ninguna | Independencia total |
| **Infrastructure** | Domain | Implementa interfaces de Domain |

---

## 🎯 2. Entidades y Modelos

### 2.1 Diagrama de entidades

```
SensorCalidadAire (1)
├─ Id: Guid
├─ Ubicacion: string
├─ TipoGas: string
├─ Estado: string
├─ CreatedAt: DateTime
├─ IsDeleted: bool
├─────────────────────────────────────
├─ Lecturas (1:N)
└─ Alertas (1:N)

LecturaAire (N)
├─ Id: Guid
├─ SensorId: Guid (FK)
├─ PM2_5: double
├─ PM10: double
├─ CO2: double
├─ FechaHora: DateTime
├─ CreatedAt: DateTime
├─ IsDeleted: bool
└─ Sensor (N:1)

AlertaAire (N)
├─ Id: Guid
├─ SensorId: Guid (FK)
├─ Nivel: string ("Leve", "Moderada", "Crítica", "Extrema")
├─ Mensaje: string
├─ FechaHora: DateTime
├─ CreatedAt: DateTime
├─ IsDeleted: bool
└─ Sensor (N:1)
```

### 2.2 BaseEntity (clase base)

Todas las entidades heredan de `BaseEntity`:

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }           // Identificador único
    public DateTime CreatedAt { get; set; }  // Fecha de creación
    public bool IsDeleted { get; set; } = false; // Soft delete
}
```

**Soft Delete**: Los registros no se eliminan, se marcan con `IsDeleted = true`. Las consultas aplican un `HasQueryFilter(!e.IsDeleted)` automáticamente.

---

## 🏗️ 3. Patrones y principios implementados

### 3.1 Repository Pattern

**Interfaz**: `IBaseRepository<TEntity>`

```csharp
public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(int page, int take);
    Task<TEntity?> FindFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, 
        params Expression<Func<TEntity, object>>[] includes);
    Task AddAsync(TEntity entity);
    Task<bool> DeleteAsync(Guid id);
}
```

**Implementación**: `BaseRepository<TEntity>` en Infrastructure
- Encapsula acceso a datos
- Reutilizable para todas las entidades
- Soporta paginación y includes dinámicos
- Maneja soft delete automáticamente

### 3.2 Unit of Work implícito

`ApplicationDbContext` actúa como Unit of Work:
- Una sola instancia por request
- SaveChangesAsync() garantiza consistencia transaccional
- Gestión automática de change tracking

### 3.3 Dependency Injection

**En Program.cs:**

```csharp
// Repositories
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ILecturaRepository, LecturaRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IJwtService, JwtService>();

// Use Cases
builder.Services.AddScoped<LecturaUseCase>();
builder.Services.AddScoped<AuthUseCase>();
builder.Services.AddScoped<SensorUseCase>();
builder.Services.AddScoped<AlertaUseCase>();
```

### 3.4 Patrón Use Case

Cada caso de uso encapsula lógica de negocio:

- `LecturaUseCase`: Registrar lecturas, generar alertas, consultar filtradas
- `AuthUseCase`: Login, registro de usuarios
- `SensorUseCase`: CRUD de sensores
- `AlertaUseCase`: Consultar alertas

---

## 🔐 4. Seguridad

### 4.1 Autenticación JWT

**Flujo**:

```
Cliente
  │
  ├─ POST /api/account/login
  │    { "email": "admin@aire.com", "password": "Admin123!" }
  │
  └─→ JwtService.GenerateToken()
      ├─ SymmetricSecurityKey(Jwt:Key)
      ├─ Claims: NameIdentifier, Email, Role
      ├─ Expira en 7 días
      └─ Retorna token JWT
  
Cliente obtiene token y lo usa en Authorization header:
  Authorization: Bearer {token}
```

**Configuración (appsettings.json)**:

```json
"Jwt": {
  "Key": "AireIndustrialSecretKey2024XYZ12345678",  // Min 32 caracteres
  "Issuer": "AireIndustrial",
  "Audience": "AireIndustrialClients"
}
```

### 4.2 Endpoints protegidos

```csharp
[Authorize]  // Requiere token válido
[ApiController]
[Route("api/[controller]")]
public class LecturaController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RegisterLecturaDto dto)
    { ... }
}
```

**Roles futuros**: Sistema de roles implementado (Admin, Usuario)
- `[Authorize(Roles = "Admin")]` — solo administradores
- `[Authorize(Roles = "Usuario")]` — usuarios normales

### 4.3 Validaciones de entrada

| Validación | Dónde | Resultado |
|-----------|-------|-----------|
| PM2_5 >= 0 | `LecturaUseCase.RegisterLecturaAsync` | BadRequest |
| PM10 >= 0 | `LecturaUseCase.RegisterLecturaAsync` | BadRequest |
| CO2 >= 0 | `LecturaUseCase.RegisterLecturaAsync` | BadRequest |
| Email no vacío | `AccountController.Login` | BadRequest |
| Password no vacío | `AccountController.Login` | BadRequest |
| Ubicacion no vacía | `SensorController.Post` | BadRequest |

### 4.4 Manejo de errores

**Respuestas HTTP**:

```json
// 400 Bad Request - Validación fallida
{
  "mensaje": "Los valores de PM2.5, PM10 y CO2 deben ser mayores o iguales a 0."
}

// 401 Unauthorized - Token inválido o expirado
{
  "mensaje": "Credenciales inválidas."
}

// 404 Not Found
{
  "mensaje": "Lectura no encontrada."
}

// 200 OK - Éxito
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "sensorId": "...",
  "pm2_5": 45.5,
  ...
}
```

---

## 💼 5. Casos de uso principales

### 5.1 UC-01: Registrar lectura de sensor

**Actor**: Usuario autenticado  
**Precondición**: Token JWT válido, sensor existente  
**Flujo**:

1. Cliente envía: `POST /api/lectura`
   ```json
   {
     "sensorId": "550e8400-e29b-41d4-a716-446655440000",
     "PM2_5": 45.5,
     "PM10": 78.2,
     "CO2": 850,
     "fechaHora": "2026-05-18T10:30:00"
   }
   ```

2. `LecturaUseCase.RegisterLecturaAsync()`:
   - Valida valores >= 0
   - Crea entidad `LecturaAire`
   - Guarda en BD
   - Llama `EvaluarAlerta(lectura)`
   - Si hay alerta, crea `AlertaAire` automáticamente
   - Retorna lectura creada

3. **Resultado**: 200 OK con lectura + alerta (si aplica)

**Umbrales de alertas**:

| Condición | Nivel | Mensaje |
|-----------|-------|---------|
| PM2.5 ∈ [25, 50] | Leve | "La calidad del aire es moderada, se recomienda reducir actividades al aire libre prolongadas." |
| PM2.5 ∈ (51, 100] ∨ CO2 > 1000 | Moderada | "La calidad del aire es poco saludable para grupos sensibles (niños, adultos mayores, personas con enfermedades respiratorias)." |
| PM2.5 > 150 ∨ PM10 > 200 | Crítica | "La calidad del aire es peligrosa. Se recomienda permanecer en interiores y usar mascarilla." |
| CO2 > 5000 ∨ PM2.5 > 250 | Extrema | "Nivel de contaminación extremadamente alto. Riesgo severo para la salud." |

### 5.2 UC-02: Consultar lecturas filtradas

**Actor**: Usuario autenticado  
**Entrada**:
- `fechaInicio`: DateTime
- `fechaFin`: DateTime
- `tipoContaminante`: string (opcional: "PM2_5", "PM10", "CO2")

**Flujo**:

1. Cliente envía: `GET /api/lectura?fechaInicio=...&fechaFin=...&tipoContaminante=PM2_5`

2. `LecturaUseCase.GetLecturasFiltradas()`:
   - Filtra por rango de fechas: `FechaHora ∈ [inicio, fin]`
   - Si `tipoContaminante`:
     - "PM2_5": retorna solo si PM2_5 > 0
     - "PM10": retorna solo si PM10 > 0
     - "CO2": retorna solo si CO2 > 0
   - Retorna colección filtrada

3. **Resultado**: 200 OK con array de lecturas

### 5.3 UC-03: Obtener lectura enriquecida con datos climáticos

**Actor**: Usuario autenticado  
**Entrada**: ID de lectura (Guid)

**Flujo**:

1. Cliente envía: `GET /api/lectura/{id}/enriquecida`

2. `LecturaController.GetEnriquecida()`:
   - Obtiene lectura con sensor: `FindFirstOrDefaultAsync(l => l.Id == id, l => l.Sensor)`
   - Llama Open-Meteo API (gratuita):
     ```
     GET https://api.open-meteo.com/v1/forecast
     ?latitude=19.4326&longitude=-99.1332
     &current=temperature_2m,relative_humidity_2m
     ```
   - Retorna lectura + datos climáticos

3. **Respuesta**:
   ```json
   {
     "lectura": { "id": "...", "pm2_5": 45.5, ... },
     "clima": {
       "current": {
         "temperature_2m": 25.5,
         "relative_humidity_2m": 65
       }
     }
   }
   ```

**Nota**: Usa coordenadas fijas (CDMX). En producción, se obtendría del sensor.

### 5.4 UC-04: Autenticación y autorización

**Actor**: Cliente no autenticado

**Flujo Login**:

1. `POST /api/account/login`
   ```json
   {
     "email": "admin@aire.com",
     "password": "Admin123!"
   }
   ```

2. `AuthUseCase.Login()`:
   - Busca usuario por email
   - Valida contraseña con `UserManager.CheckPasswordAsync()`
   - Obtiene roles: `UserManager.GetRolesAsync()`
   - Llama `JwtService.GenerateToken(usuario, roles)`

3. **Respuesta**:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
   }
   ```

4. Cliente usa token en requests posteriores:
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

---

## 📚 6. Tecnologías utilizadas

| Tecnología | Versión | Propósito |
|-----------|---------|----------|
| .NET | 9.0 | Runtime y framework web |
| Entity Framework Core | 9.0.0 | ORM |
| Pomelo MySQL | 9.0.0 | Driver MySQL |
| ASP.NET Identity | 9.0.0 | Gestión de usuarios |
| JWT Bearer | 9.0.0 | Autenticación |
| Swashbuckle | 7.2.0 | Documentación Swagger |
| MySQL | 8.0 | Base de datos |

---

## 🗄️ 7. Base de datos

### 7.1 Tablas principales

```sql
CREATE TABLE SensoresCalidadAire (
    Id CHAR(36) PRIMARY KEY,
    Ubicacion VARCHAR(200) NOT NULL,
    TipoGas VARCHAR(100) NOT NULL,
    Estado VARCHAR(50) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    IsDeleted TINYINT(1) DEFAULT 0
);

CREATE TABLE LecturasAire (
    Id CHAR(36) PRIMARY KEY,
    SensorId CHAR(36) NOT NULL,
    PM2_5 DOUBLE NOT NULL,
    PM10 DOUBLE NOT NULL,
    CO2 DOUBLE NOT NULL,
    FechaHora DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    IsDeleted TINYINT(1) DEFAULT 0,
    CONSTRAINT FK_LecturasAire_SensoresCalidadAire
        FOREIGN KEY (SensorId) REFERENCES SensoresCalidadAire(Id) ON DELETE RESTRICT
);

CREATE TABLE AlertasAire (
    Id CHAR(36) PRIMARY KEY,
    SensorId CHAR(36) NOT NULL,
    Nivel VARCHAR(50) NOT NULL,
    Mensaje VARCHAR(500) NOT NULL,
    FechaHora DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    IsDeleted TINYINT(1) DEFAULT 0,
    CONSTRAINT FK_AlertasAire_SensoresCalidadAire
        FOREIGN KEY (SensorId) REFERENCES SensoresCalidadAire(Id) ON DELETE RESTRICT
);
```

### 7.2 Índices de rendimiento

```sql
CREATE INDEX IX_LecturasAire_SensorId ON LecturasAire(SensorId);
CREATE INDEX IX_LecturasAire_FechaHora ON LecturasAire(FechaHora);
CREATE INDEX IX_AlertasAire_SensorId ON AlertasAire(SensorId);
CREATE INDEX IX_AlertasAire_FechaHora ON AlertasAire(FechaHora);
```

### 7.3 Query Filters globales

Cada DbSet aplica automáticamente:

```csharp
.HasQueryFilter(e => !e.IsDeleted)
```

Esto garantiza que las consultas excluyan registros eliminados sin necesidad de filtros manuales.

---

## 📊 8. Flujos principales

### Flujo de registro de lectura

```mermaid
Cliente
   │
   ├─→ POST /api/lectura
   │   {sensorId, PM2_5, PM10, CO2, fechaHora}
   │
   └─→ LecturaController.Post()
       │
       ├─→ LecturaUseCase.RegisterLecturaAsync()
       │   │
       │   ├─ Validar: PM2_5, PM10, CO2 >= 0
       │   ├─ Crear: LecturaAire
       │   ├─ Guardar en BD
       │   │
       │   ├─→ EvaluarAlerta(lectura)
       │   │   ├─ if CO2 > 5000 ∨ PM2_5 > 250 → Extrema
       │   │   ├─ else if PM2_5 > 150 ∨ PM10 > 200 → Crítica
       │   │   ├─ else if PM2_5 ∈ (51, 100] ∨ CO2 > 1000 → Moderada
       │   │   ├─ else if PM2_5 ∈ [25, 50] → Leve
       │   │   └─ return AlertaAire (si aplica)
       │   │
       │   └─ if alerta ≠ null → Guardar AlertaAire
       │
       └─→ Response: 200 OK {lectura, alerta}
```

---

## 🧪 9. Consideraciones de calidad de código

### 9.1 Principios SOLID

- **S**ingle Responsibility: Cada clase tiene una única responsabilidad
- **O**pen/Closed: Abierto a extensión (nuevos repositorios), cerrado a modificación
- **L**iskov Substitution: Repositorios intercambiables
- **I**nterface Segregation: Interfaces específicas (ISensorRepository, ILecturaRepository)
- **D**ependency Inversion: Inyección de dependencias, inversión de control

### 9.2 Nombres en código

- **Español**: Nombres de UI y datos (Sensor, Lectura, Alerta)
- **Inglés**: Sintaxis y métodos (.NET conventions)
- **Claridad**: Nombres descriptivos sin abreviaturas

### 9.3 Gestión de errores

- ✅ Validaciones en entradas
- ✅ Respuestas HTTP semánticas (200, 400, 401, 404)
- ✅ Mensajes de error descriptivos
- ✅ Sin throws anónimos
- ✅ Manejo de excepciones en capas superiores

### 9.4 Sin código autogenerado

- ✅ Modelos escritos manualmente
- ✅ Repositorios implementados a mano
- ✅ DTOs creados explícitamente
- ❌ Scaffolding prohibido
- ❌ Code generation no utilizado

---

## 📝 10. Conclusiones

La arquitectura implementada proporciona:

1. **Mantenibilidad**: Código limpio, estructurado en capas
2. **Escalabilidad**: Fácil agregar nuevas entidades y casos de uso
3. **Testabilidad**: Dependencias inyectadas, interfaces bien definidas
4. **Seguridad**: Autenticación JWT, validaciones de entrada
5. **Documentación**: Swagger/OpenAPI, comentarios en código
6. **Rendimiento**: Índices en BD, paginación, lazy loading

---

**Documento preparado el 18 de mayo de 2026**  
**Desarrollado por**: Parcial 3 - .NET 9  
**Versión API**: 1.0
