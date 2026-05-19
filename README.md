# AireIndustrial API

API RESTful para monitoreo de calidad del aire en plantas industriales, construida con .NET 9, MySQL y arquitectura limpia.

## 📋 Características

- ✅ Registro de lecturas de sensores (PM2.5, PM10, CO2)
- ✅ Generación automática de alertas según umbrales OMS
- ✅ Enriquecimiento de datos con información climática externa (Open-Meteo)
- ✅ Autenticación JWT con control de roles
- ✅ Arquitectura limpia (Domain, Application, Infrastructure, API)
- ✅ Documentación interactiva con Swagger/OpenAPI
- ✅ Validaciones de entrada y manejo de errores

## 🛠️ Requisitos

- **.NET 9.0** o superior
- **Docker** (para MySQL) o **MySQL Server 8.0+**
- **Git**

## 🚀 Instalación y ejecución

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/AireIndustrial.git
cd AireIndustrial
```

### 2. Levantar MySQL con Docker

```powershell
docker run --name mysql-aire -e MYSQL_ROOT_PASSWORD=tuga009 -p 3306:3306 -d mysql:8.0
```

Verifica que esté corriendo:
```powershell
docker ps
```

### 3. Restaurar paquetes NuGet

```bash
dotnet restore AireIndustrial.sln
```

### 4. Configurar conexión a BD (opcional)

Edita `AireIndustrial/appsettings.json` si tienes diferentes credenciales:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=aire_industrial;User=root;Password=tuga009;"
}
```

### 5. Ejecutar la aplicación

```bash
dotnet run --project AireIndustrial/AireIndustrial.csproj
```

O en Visual Studio: **F5**

La aplicación se iniciará en:
- **HTTPS**: https://localhost:53760
- **HTTP**: http://localhost:53761

### 6. Acceder a Swagger

Abre en el navegador:
```
https://localhost:53760/swagger
```

## 📡 Endpoints principales

### Sin autenticación

**POST** `/api/account/login` — Obtener token JWT
```json
{
  "email": "admin@aire.com",
  "password": "Admin123!"
}
```

**POST** `/api/account/register` — Registrar nuevo usuario
```json
{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan@example.com",
  "tel": "+34612345678",
  "password": "Pwd123!"
}
```

### Con autenticación (Bearer Token)

**POST** `/api/lectura` — Registrar lectura de sensor
```json
{
  "sensorId": "550e8400-e29b-41d4-a716-446655440000",
  "PM2_5": 45.5,
  "PM10": 78.2,
  "CO2": 850,
  "fechaHora": "2026-05-18T10:30:00"
}
```

**GET** `/api/lectura?fechaInicio=2026-05-18T00:00:00&fechaFin=2026-05-18T23:59:59&tipoContaminante=PM2_5` — Consultar lecturas filtradas

**GET** `/api/lectura/{id}/enriquecida` — Lectura con datos climáticos (temperatura, humedad)

**GET** `/api/sensor?page=1&take=10` — Listar sensores

**POST** `/api/sensor` — Crear sensor
```json
{
  "ubicacion": "Área de producción A",
  "tipoGas": "PM2.5, PM10, CO2",
  "estado": "Activo"
}
```

**GET** `/api/alerta?page=1&take=10` — Listar alertas

## 🔐 Seguridad

- Autenticación JWT con expiración de 7 días
- Endpoints sensibles protegidos con `[Authorize]`
- Validaciones de entrada (valores no negativos)
- Respuestas HTTP con mensajes descriptivos
- Documentación con esquema de seguridad en Swagger

## 📊 Umbrales de alertas OMS

| Nivel | Condición | Mensaje |
|-------|-----------|---------|
| **Leve** | PM2.5: 25-50 µg/m³ | "La calidad del aire es moderada, se recomienda reducir actividades al aire libre prolongadas." |
| **Moderada** | PM2.5: 51-100 o CO2 > 1000 ppm | "La calidad del aire es poco saludable para grupos sensibles (niños, adultos mayores, personas con enfermedades respiratorias)." |
| **Crítica** | PM2.5 > 150 o PM10 > 200 | "La calidad del aire es peligrosa. Se recomienda permanecer en interiores y usar mascarilla." |
| **Extrema** | CO2 > 5000 o PM2.5 > 250 | "Nivel de contaminación extremadamente alto. Riesgo severo para la salud." |

## 📁 Estructura del proyecto

```
AireIndustrial/
├── AireIndustrial/                    # API/Presentation
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
├── AireIndustrial.Application/        # Use Cases & DTOs
│   ├── UseCases/
│   └── DTOs/
├── AireIndustrial.Domain/             # Entities & Interfaces
│   ├── Entities/
│   └── Interfaces/
├── AireIndustrial.Infrastructure/     # Data & Services
│   ├── Persistence/
│   ├── Services/
│   ├── Identity/
│   └── Extensions/
├── AireIndustrial.sln
└── script.sql                         # Script de creación de BD
```

## 🧪 Pruebas manuales

1. **Registrar usuario**: POST `/api/account/register`
2. **Obtener token**: POST `/api/account/login`
3. **Crear sensor**: POST `/api/sensor` (con Authorization header)
4. **Registrar lectura**: POST `/api/lectura` (con Authorization header)
5. **Ver alertas generadas**: GET `/api/alerta` (con Authorization header)
6. **Consultar lectura enriquecida**: GET `/api/lectura/{id}/enriquecida`

## 📖 Documentación técnica

Consulta `ARQUITECTURA.md` para detalles sobre:
- Arquitectura de capas (Clean Architecture)
- Patrones de diseño implementados
- Seguridad y validaciones
- Casos de uso y flujos de negocio

## 🐛 Troubleshooting

### Error de conexión a BD
```
Verifique que MySQL esté corriendo en puerto 3306
docker ps | grep mysql-aire
```

### Puerto ya en uso
Cambie el puerto en `appsettings.json` o en las propiedades del proyecto.

### Token JWT inválido
Asegúrese de incluir el header:
```
Authorization: Bearer {token_aqui}
```

## 📝 Licencia

Este proyecto es de uso educativo.

## Autor

Jonny Quintanilla
CIF: 2025010290

Parcial 3 - API Distribuida con .NET

---

Última actualización: 18 de mayo de 2026
