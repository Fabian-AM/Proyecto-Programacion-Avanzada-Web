Grupo C.

1. Integrantes finales del grupo. A los que se les asignará la nota del proyecto
   
1.Felipe Mora Quesada
2.Sebastián Araya Gómez
3.Johnson Abarca Chaves
4.Fabián Araya Marchena

2. Enlace del repositorio si lo subió en GitHub o en algún otro.
https://github.com/Fabian-AM/Proyecto-Programacion-Avanzada-Web
   
2. Sistema de Gestión de Créditos

Aplicación web desarrollada en ASP.NET Core 8 MVC para la gestión de clientes, solicitudes de crédito, usuarios, roles y bitácora de movimientos.  
Forma parte del proyecto de Programación Avanzada Web e implementa una arquitectura por capas (Web, BLL, DAL) con buenas prácticas y separación de responsabilidades.

Características principales

- Autenticación y autorización con ASP.NET Core Identity.
- Gestión de **clientes** (CRUD completo).
- Gestión de **solicitudes de crédito**:
  - Registro, actualización y consulta de solicitudes.
  - Asociación de solicitudes a clientes.
- Gestión de **usuarios y roles** del sistema:
  - Roles predefinidos: Admin, Analista, Gestor, Servicio al Cliente, Cliente.
- Bitácora de movimientos:
  - Registro de acciones sobre solicitudes (usuario, acción, comentario, fecha/hora).
- Interfaz web usando:
  - Bootstrap 5
  - jQuery
  - DataTables
  - Font Awesome


Arquitectura del proyecto

El sistema está organizado en 3 capas:

1️ ProyectoProgramacion (Capa Web/UI)

- Framework: ASP.NET Core 8 MVC
- Contiene:
  - Controladores (Controllers)
    - HomeController
    - AccountController
    - ClienteController
    - SolicitudesController
    - UsersController
  - Vistas (Views) con Razor:
    - Módulo de Clientes
    - Módulo de Solicitudes
    - Módulo de Usuarios
    - Inicio y navegación principal
  - Archivos estáticos (wwwroot): CSS, JS, librerías externas.
  - Configuraciones de startup en Program.cs:
    - Registro de servicios (repositorios, servicios BLL, AutoMapper, Identity, DbContext).
    - Middleware de autenticación, autorización, manejo de errores, etc.

 2️ ProyectoProgramacionBLL (Business Logic Layer)

- Contiene la lógica de negocio:
  - Servicios (por ejemplo, ClienteServicio, SolicitudesServicio, AccountService).
  - Dtos y ViewModels para transferir datos entre capas.
  - Mapeos con **AutoMapper** para mapear entidades ↔ DTOs.
  - Inicialización/RoleInitializer.cs para crear los roles basados en Identity.

3️ ProyectoProgramacionDAL (Data Access Layer)

- Contiene el acceso a datos con Entity Framework Core:
  - Contexto/AppDbContext.cs:
    - Hereda de IdentityDbContext<ApplicationUser>.
    - Define las tablas:
      - Clientes
      - Solicitudes
      - Documentos
      - BitacoraMovimientos
   - Entidades:
    - ApplicationUser (extiende IdentityUser con datos adicionales como Identificación, Nombre, Apellido).
    - Cliente
    - Solicitud
    - Documento
    - BitacoraMovimiento
    - Repositorios:
    - Interfaces e implementaciones para CRUD (IClientesRepositorio, ISolicitudesRepositorio, etc.).


Tecnologías utilizadas

- .NET 8.0
- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core 8
  - Provider: SQLite
- AutoMapper
- Bootstrap 5
- Query
- DataTables
- Font Awesome



Requisitos previos

Antes de ejecutar el proyecto, asegurate de tener:

- .NET 8 SDK instalado  
    Podés verificar con:
  bash
  dotnet --version
