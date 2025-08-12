# 🚀 Backend API - La Cazuela Chapina

## 📋 Descripción General
API REST desarrollada en ASP.NET Core para el sistema de gestión de restaurante "La Cazuela Chapina". Proporciona funcionalidades completas para administración de productos, pedidos, inventario, usuarios y servicios de IA.

## 🏗️ Arquitectura del Sistema

### Patrón de Arquitectura
- **Arquitectura en Capas**: Controllers → Services → Repositories → Data
- **Patrón Repository**: Implementación genérica con repositorios específicos
- **Inyección de Dependencias**: Configurada en Program.cs
- **Middleware Personalizado**: Para manejo de errores, logging y rate limiting

### Tecnologías Principales
- **Framework**: ASP.NET Core 9.0
- **Base de Datos**: SQL Server con Entity Framework Core
- **Cache**: Redis para optimización de rendimiento
- **Autenticación**: JWT Bearer Token
- **IA**: Integración con OpenAI GPT-4
- **Voz**: Speech-to-Text y Text-to-Speech

## 🔐 Sistema de Autenticación y Autorización

### Flujo de Autenticación
1. **Registro de Usuario** (`POST /api/v1/user/register`)
   - Validación de contraseña (mínimo 12 caracteres)
   - Hash de contraseña con PasswordHasher
   - Asignación de rol por defecto: "Customer"

2. **Login** (`POST /api/v1/user/login`)
   - Verificación de credenciales
   - Generación de JWT Token
   - Generación de Refresh Token (válido por 7 días)
   - Almacenamiento en base de datos

3. **Autorización por Roles**
   - **Customer**: Acceso básico a productos y pedidos
   - **Admin**: Acceso completo a todas las funcionalidades

### Seguridad Implementada
- **Rate Limiting**: 50 requests por minuto por IP
- **CORS**: Configurado para orígenes específicos
- **Validación de Modelos**: Automática en todos los endpoints
- **Manejo de Errores**: Middleware personalizado para excepciones

## 🔄 Flujos de Negocio Principales

### 1. Flujo de Pedidos
```
Cliente → Selecciona Productos → Crea Pedido → Pago → Confirmación → Inventario
   ↓
1. Validación de stock
2. Cálculo de totales
3. Creación de pedido
4. Actualización de inventario
5. Notificación al cliente
```

### 2. Flujo de Inventario
```
Movimiento → Validación → Actualización → Registro → Cache
   ↓
1. Verificación de disponibilidad
2. Cálculo de nuevo stock
3. Registro de movimiento
4. Actualización de cache Redis
5. Notificación de stock bajo (si aplica)
```

### 3. Flujo de IA y Chat
```
Usuario → Consulta → Contexto BD → OpenAI → Respuesta → Cache
   ↓
1. Análisis de consulta del usuario
2. Búsqueda de contexto relevante en BD
3. Envío a OpenAI con contexto
4. Generación de respuesta
5. Almacenamiento en cache para futuras consultas
```

## 🌐 Endpoints de la API

### Base URL: `/api/v1`

#### Autenticación
- `POST /user/register` - Registro de usuario
- `POST /user/login` - Login de usuario
- `GET /user` - Obtener todos los usuarios (Admin)
- `GET /user/{id}` - Obtener usuario por ID
- `PUT /user/{id}` - Actualizar usuario
- `DELETE /user/{id}` - Eliminar usuario (Admin)

#### Productos
- `GET /product` - Listar productos
- `GET /product/{id}` - Obtener producto por ID
- `POST /product` - Crear producto (Admin)
- `PUT /product/{id}` - Actualizar producto (Admin)
- `DELETE /product/{id}` - Eliminar producto (Admin)

#### Pedidos
- `GET /order` - Listar pedidos
- `GET /order/{id}` - Obtener pedido por ID
- `POST /order` - Crear pedido
- `PUT /order/{id}` - Actualizar pedido
- `DELETE /order/{id}` - Cancelar pedido

#### Inventario
- `GET /inventory` - Listar inventario
- `GET /inventory/{id}` - Obtener item de inventario
- `POST /inventory` - Crear item de inventario (Admin)
- `PUT /inventory/{id}` - Actualizar inventario (Admin)
- `POST /inventory/movement` - Registrar movimiento

#### Combos
- `GET /combo` - Listar combos
- `GET /combo/{id}` - Obtener combo por ID
- `POST /combo` - Crear combo (Admin)
- `PUT /combo/{id}` - Actualizar combo (Admin)
- `DELETE /combo/{id}` - Eliminar combo (Admin)

#### Ventas
- `GET /sale` - Listar ventas
- `GET /sale/{id}` - Obtener venta por ID
- `POST /sale` - Crear venta
- `PUT /sale/{id}` - Actualizar venta

#### Sucursales y Proveedores
- `GET /branch` - Listar sucursales
- `GET /supplier` - Listar proveedores
- `POST /branch` - Crear sucursal (Admin)
- `POST /supplier` - Crear proveedor (Admin)

#### Servicios de IA
- `POST /ai/chat` - Chat con IA
- `POST /voice/chat` - Chat por voz
- `GET /knowledge` - Obtener conocimiento de IA

## 🛠️ Servicios y Lógica de Negocio

### UserService
- **Registro**: Validación de email, contraseña y creación de usuario
- **Login**: Verificación de credenciales y generación de tokens
- **Gestión**: CRUD completo de usuarios

### ProductService
- **Validación**: Verificación de datos antes de persistir
- **Stock**: Control de inventario automático
- **Precios**: Cálculo de totales y descuentos

### OrderService
- **Creación**: Validación de stock y cálculo de totales
- **Estado**: Gestión de estados del pedido ( Solicitado, en progreso, confirmado)
- **Inventario**: Actualización automática de stock

### InventoryService
- **Movimientos**: Registro de entradas/salidas
- **Validación**: Verificación de disponibilidad
- **Alertas**: Notificación de stock bajo

### OpenAIChatService
- **Contexto**: Integración con base de datos para respuestas precisas
- **Prompting**: Sistema de prompts estructurados
- **Cache**: Almacenamiento de respuestas frecuentes

## 🔧 Middleware y Configuración

### Middleware Personalizado
1. **ErrorHandlingMiddleware**: Captura excepciones no manejadas
2. **RequestLoggingMiddleware**: Logging de todas las peticiones
3. **RateLimitingMiddleware**: Control de velocidad de requests

### Configuración de Base de Datos
- **Entity Framework**: Configurado con lazy loading
- **Migrations**: Sistema de versionado de esquema
- **Índices**: Optimización para consultas frecuentes
- **Constraints**: Restricciones de integridad referencial

### Configuración de Cache (Redis)
- **Almacenamiento**: Respuestas de API frecuentes
- **Invalidación**: Limpieza automática por patrones
- **TTL**: Tiempo de vida configurable por tipo de dato

## 📊 Optimizaciones de Rendimiento

### Estrategias Implementadas
1. **Cache Redis**: Para consultas frecuentes
2. **Lazy Loading**: Carga diferida de entidades relacionadas
3. **Índices de BD**: Para búsquedas optimizadas
4. **Rate Limiting**: Protección contra sobrecarga
5. **Async/Await**: Operaciones asíncronas en toda la aplicación

### Métricas de Rendimiento
- **Response Time**: < 200ms para operaciones simples
- **Throughput**: 50 requests/min por IP
- **Cache Hit Rate**: > 80% para datos frecuentes
- **Database Connections**: Pooling configurado

## 🚀 Despliegue y Configuración

### Variables de Entorno Requeridas
```bash
# Base de Datos
SQL_HOST=your_sql_host
SQL_PORT=1433
SQL_DATABASE=your_database
SQL_USER=your_user
SQL_PASSWORD=your_password

# Redis
REDIS_HOST=your_redis_host
REDIS_PORT=6379

# JWT
Jwt__Issuer=your_issuer
Jwt__Audience=your_audience
Jwt__Key=your_secret_key

# OpenAI
API_KEY_OPENAI=your_openai_key
```

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY bin/Release/net8.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Api.dll"]
```

### Health Checks
- **Endpoint**: `/health`
- **Verificaciones**: Base de datos, Redis, servicios externos
- **Métricas**: Tiempo de respuesta y estado de servicios

## 🔍 Monitoreo y Logging

### Sistema de Logging
- **Structured Logging**: Con Serilog
- **Niveles**: Debug, Info, Warning, Error, Fatal
- **Contexto**: Request ID, usuario, endpoint, duración

### Métricas de Monitoreo
- **Requests por minuto**
- **Tiempo de respuesta promedio**
- **Tasa de errores**
- **Uso de memoria y CPU**
- **Estado de servicios externos**

## 📚 Documentación Adicional

### Swagger/OpenAPI
- **Endpoint**: `/swagger` (solo en desarrollo)
- **Documentación**: Completa de todos los endpoints
- **Testing**: Interfaz para probar la API

### Postman
- **Colección**: https://documenter.getpostman.com/view/18340700/2sB3BGFokV

### Postman Collection
- **Colección**: Endpoints organizados por funcionalidad
- **Variables**: Configuración para diferentes entornos
- **Tests**: Validaciones automáticas de respuestas


*Este README documenta la versión 1.0 del backend de La Cazuela Chapina.*
