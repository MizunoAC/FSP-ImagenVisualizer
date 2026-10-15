# FSP-Images

Galería ASP.NET Core MVC (.NET 8) que muestra las imágenes de una carpeta remota
con directory browsing activo. No modifica el sitio de origen.

## Configurar

En `appsettings.json`:

    "Images": {
      "BaseUrl": "https://YOUR-SITE.smarterasp.net/imagenes/",
      "PageSize": 24
    }

## Ejecutar

    dotnet restore
    dotnet run

## Publicar

    dotnet publish -c Release -o publish

Sube el contenido de `publish/` por FTP y selecciona .NET 8 en el panel de SmarterASP.NET.

## Estructura

- `Services/ImageService.cs`: lee el listado remoto (carpetas e imágenes).
- `Controllers/GalleryController.cs`: arma la página con paginación.
- `Views/Gallery/Index.cshtml`: la cuadrícula.
- `NuGet.Config`: usa solo nuget.org en este proyecto.
