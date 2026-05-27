# API E-Commerce 🛒

Una API REST moderna construida con .NET 10.0 para gestionar un sistema de comercio electrónico con características completas de autenticación, autorización, versionado de API y mapeo de datos con Mapster.

## 🚀 Características

- ✅ **API RESTful** - Endpoints completamente funcionales
- ✅ **Autenticación JWT** - Seguridad con tokens JWT
- ✅ **Autenticación con Identity** - Gestión de usuarios integrada
- ✅ **Versionado de API** - Soporte para múltiples versiones de API (v1, v2)
- ✅ **Mapster** - Mapeo rápido y eficiente de DTOs
- ✅ **Entity Framework Core** - ORM moderno
- ✅ **SQL Server** - Base de datos relacional
- ✅ **Docker Compose** - Fácil despliegue con contenedores
- ✅ **Swagger/OpenAPI** - Documentación interactiva
- ✅ **CORS** - Control de origen cruzado

## 📋 Requisitos Previos

### Opción 1: Ejecución Local
- **.NET 10.0 SDK** - [Descargar](https://dotnet.microsoft.com/download/dotnet/10.0)
- **SQL Server 2022** - [Descargar](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Visual Studio Code** o **Visual Studio 2022** (opcional)

### Opción 2: Con Docker
- **Docker** - [Descargar](https://www.docker.com/products/docker-desktop)
- **Docker Compose** - Incluido con Docker Desktop

## 🔧 Instalación

### 1. Clonar el repositorio

```bash
git clone <URL-DEL-REPOSITORIO>
cd ApiEcommerce
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

## 🏃 Ejecución Local

### Opción 1: Sin Docker (requiere SQL Server instalado localmente)

#### Paso 1: Configurar la cadena de conexión

Editar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ConexionSql": "Server=tu-servidor;Database=ApiEcommerceNET10;User ID=SA;Password=tu-contraseña;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

#### Paso 2: Crear la base de datos y ejecutar migraciones

```bash
dotnet ef database update
```

#### Paso 3: Ejecutar la aplicación

```bash
dotnet run
```

La API estará disponible en: `http://localhost:5001`

### Opción 2: Con Docker Compose (Recomendado) ⭐

#### Paso 1: Levantar los contenedores

```bash
docker-compose up -d
```

Este comando:
- Descarga la imagen de SQL Server 2022
- Crea un contenedor llamado `sqlserver2022`
- Expone SQL Server en el puerto `1433`
- Crea un volumen para persistencia de datos

#### Paso 2: Esperar a que SQL Server esté listo (30-60 segundos)

```bash
# Verificar logs del contenedor
docker-compose logs -f sql
```

#### Paso 3: Ejecutar migraciones

```bash
dotnet ef database update
```

#### Paso 4: Ejecutar la aplicación

```bash
dotnet run
```

La API estará disponible en: `http://localhost:5001`

## 🛑 Detener los contenedores

```bash
# Detener sin eliminar (los volúmenes se conservan)
docker-compose down

# Detener y eliminar todo incluyendo volúmenes
docker-compose down -v

# Verificar que esté detenido
docker-compose ps
```

## 📊 Estructura del Proyecto

```
ApiEcommerce/
├── Controllers/              # Controladores de la API
│   ├── ProductsController.cs
│   ├── UsersController.cs
│   ├── V1/                   # API versión 1.0
│   │   └── CategoriesController.cs
│   └── V2/                   # API versión 2.0
│       └── CategoriesController.cs
├── Models/                   # Modelos de datos
│   ├── Product.cs
│   ├── Category.cs
│   ├── ApplicationUser.cs
│   └── Dtos/                 # Data Transfer Objects
│       ├── ProductDto.cs
│       ├── CategoryDto.cs
│       └── UserDto.cs
├── Repository/               # Patrón Repository
│   ├── ProductRepository.cs
│   ├── CategoryRepository.cs
│   ├── UserRepository.cs
│   └── IRepository/          # Interfaces
├── Mapping/                  # Configuración de Mapster
│   ├── ProductProfile.cs
│   ├── CategoryProfile.cs
│   └── UserProfile.cs
├── Data/                     # Contexto de BD
│   ├── ApplicationDbContext.cs
│   └── DataSeeder.cs
├── Migrations/               # Migraciones de EF Core
├── Constants/                # Constantes de la aplicación
├── wwwroot/                  # Recursos estáticos
├── Program.cs                # Configuración principal
├── docker-compose.yaml       # Configuración Docker
└── appsettings.json          # Configuración de la aplicación
```

## 🔌 Endpoints Principales

### Productos
- `GET /api/v{version}/products` - Obtener todos los productos
- `GET /api/v{version}/products/{id}` - Obtener producto por ID
- `GET /api/v{version}/products/Paged` - Obtener productos paginados
- `POST /api/v{version}/products` - Crear nuevo producto
- `PUT /api/v{version}/products/{id}` - Actualizar producto
- `DELETE /api/v{version}/products/{id}` - Eliminar producto

### Categorías
- `GET /api/v{version}/categories` - Obtener todas las categorías
- `GET /api/v{version}/categories/{id}` - Obtener categoría por ID
- `POST /api/v{version}/categories` - Crear nueva categoría
- `PUT /api/v{version}/categories/{id}` - Actualizar categoría
- `DELETE /api/v{version}/categories/{id}` - Eliminar categoría

### Usuarios
- `POST /api/v{version}/users/Register` - Registrar nuevo usuario
- `POST /api/v{version}/users/Login` - Iniciar sesión
- `GET /api/v{version}/users` - Obtener todos los usuarios (Admin)
- `GET /api/v{version}/users/{id}` - Obtener usuario por ID (Admin)

### Documentación
- `http://localhost:5001/swagger` - Swagger UI
- `http://localhost:5001/swagger/v1/swagger.json` - JSON de OpenAPI

## 🔐 Autenticación

La API utiliza **JWT (JSON Web Tokens)**. Para acceder a endpoints protegidos:

1. **Registrarse**: `POST /api/v{version}/users/Register`
   ```json
   {
     "userName": "usuario@example.com",
     "password": "Contraseña123!",
     "email": "usuario@example.com",
     "firstName": "Juan",
     "lastName": "Pérez"
   }
   ```

2. **Login**: `POST /api/v{version}/users/Login`
   ```json
   {
     "userName": "usuario@example.com",
     "password": "Contraseña123!"
   }
   ```

3. **Usar el Token**: Incluir en el header `Authorization: Bearer {token}`

## 🗄️ Base de Datos

### Credenciales por Defecto (Docker)
- **Usuario**: SA
- **Contraseña**: MyStrongPass123
- **Puerto**: 1433
- **Base de datos**: ApiEcommerceNET10

### Conectar desde SQL Server Management Studio
```
Server: localhost,1433
Authentication: SQL Server Authentication
User: SA
Password: MyStrongPass123
```

### Conectar desde Azure Data Studio
- **Server**: localhost,1433
- **Username**: SA
- **Password**: MyStrongPass123
- **Database**: ApiEcommerceNET10

## 📝 Migraciones

### Ver migraciones
```bash
dotnet ef migrations list
```

### Crear nueva migración
```bash
dotnet ef migrations add NombreDeLaMigracion
```

### Revertir última migración
```bash
dotnet ef database update NombreDeLaMigracionAnterior
```

### Ver últimas migraciones
```bash
dotnet ef migrations list -v
```

## 🧪 Desarrollo

### Agregar nueva dependencia
```bash
dotnet add package NombreDelPaquete --version 1.0.0
```

### Compilar el proyecto
```bash
dotnet build
```

### Ejecutar con configuración de debug
```bash
dotnet run --configuration Debug
```

### Ejecutar con configuración de release
```bash
dotnet run --configuration Release
```

## 📦 Dependencias Principales

```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.7" />
<PackageReference Include="Mapster" Version="10.0.0" />
<PackageReference Include="Asp.Versioning.Mvc" Version="10.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.8" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="10.1.7" />
<PackageReference Include="BCrypt.Net-Next" Version="4.2.0" />
```

## 🐛 Solución de Problemas

### El contenedor SQL Server no inicia
```bash
# Ver logs
docker-compose logs sql

# Reiniciar
docker-compose restart sql

# Reconstruir
docker-compose up -d --build
```

### Error de conexión a la base de datos
- Verificar que los contenedores estén corriendo: `docker-compose ps`
- Verificar que el puerto 1433 no esté en uso
- Reintentar conexión después de 60 segundos de iniciar Docker

### Error en migraciones
```bash
# Limpiar y restaurar
dotnet clean
dotnet restore

# Reintenta la migración
dotnet ef database update
```

### Puertos ocupados
```bash
# Cambiar puerto en docker-compose.yaml o properties/launchSettings.json
# Por defecto: 5001 (HTTP), 7001 (HTTPS), 1433 (SQL)
```

## 📱 Usando la API con Postman/Insomnia

1. Importar la colección desde `ApiEcommerce.http`
2. Ejecutar login para obtener el JWT
3. Copiar el token en el header `Authorization: Bearer {token}`
4. Ejecutar los endpoints deseados

## 🚀 Deployment

### En producción cambiar:
- `appsettings.Production.json` - Configuración de producción
- Cambiar `SecretKey` por una clave más segura
- Cambiar `ConexionSql` por servidor real
- Habilitar HTTPS obligatorio
- Desabilitar Swagger

## 📚 Recursos

- [Documentación de .NET](https://learn.microsoft.com/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Mapster](https://mapperly.riok.app/)
- [JWT Authentication](https://tools.ietf.org/html/rfc7519)
- [Docker Documentation](https://docs.docker.com/)

## 👨‍💻 Autor

Desarrollado como proyecto de E-Commerce con .NET 10.0 Por Allan Fallas

## 📄 Licencia

Este proyecto está bajo la licencia MIT.

## 📧 Soporte

Para reportar problemas o sugerencias, abrir un issue en el repositorio.

---

**Última actualización**: Mayo 2026
**Versión de .NET**: 10.0
**Versión de SQL Server**: 2022
