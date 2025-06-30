# 💈 Turnos Peluquería

Aplicación web para gestionar turnos en una peluquería. Desarrollada como proyecto académico utilizando ASP.NET Core MVC con Entity Framework Core y Bootstrap.

---

## 🎯 Descripción

Esta aplicación permite a los clientes reservar turnos con distintos peluqueros, seleccionando el horario y el servicio deseado. También pueden consultar los turnos que tienen agendados.

Está pensada como una solución simple y funcional para la gestión de turnos en un entorno de peluquería, con vistas diferenciadas para clientes y un diseño responsive.

---

## 🛠️ Tecnologías utilizadas

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First)
- Bootstrap 5
- Razor Views
- SQL Server LocalDB
- C#

---
## 🚀 Cómo ejecutar el proyecto

1. Cloná el repositorio:
   ```bash
   git clone https://github.com/fedechiesa/peluqueria-turnos.git
   ```

2. Abrí la solución `.sln` en **Visual Studio**.

3. Configurá la conexión a la base de datos en `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=TurnosPeluqueria;Trusted_Connection=True;"
   }
   ```

4. Aplicá las migraciones:
   ```bash
   Update-Database
   ```

5. Ejecutá el proyecto (`F5` o `Ctrl + F5`).

## 📸 Funcionalidades

### Cliente
- Registro e inicio de sesión
- Selección de peluquero con su imagen
- Visualización de horarios disponibles
- Reserva y cancelación de turnos
- Listado de "Mis Turnos"

### Administrador
- Visualización general de todos los turnos agendados
