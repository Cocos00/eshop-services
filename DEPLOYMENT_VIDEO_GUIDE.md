# Guia de publicacion y video maximo 15 minutos

## Requisitos cubiertos

- Base de datos publicada en nube: Render Postgres con `CatalogDb` y `BasketDb`.
- API publicada en nube real: Render Web Services con Docker para `Catalog.API` y `Basket.API`.
- Frontend publicado en Netlify: Vue Vite desde `frontend`.
- Basket paso a paso: `GET`, `POST`, `DELETE` y Redis cache.
- Redis cache: Render Key Value conectado a `Basket.API`.

## Antes de grabar

1. Subir el proyecto a GitHub.
2. Confirmar que compila localmente:

```powershell
dotnet build D:\eshop-services--2.0\eshop-services\eshop-services.sln
cd D:\eshop-services--2.0\eshop-services\frontend
npm run build
```

3. Tener cuentas listas:
   - Render
   - Netlify
   - GitHub

## Publicar APIs y bases en Render

1. Entrar a Render.
2. Seleccionar **New > Blueprint**.
3. Conectar el repositorio de GitHub.
4. Render detecta `render.yaml` en la raiz.
5. Confirmar recursos:
   - `eshop-catalog-db`
   - `eshop-basket-db`
   - `eshop-redis-cache`
   - `eshop-catalog-api`
   - `eshop-basket-api`
6. Crear el Blueprint y esperar el deploy.
7. Probar:

```text
https://eshop-catalog-api.onrender.com/health
https://eshop-basket-api.onrender.com/health
https://eshop-catalog-api.onrender.com/products/search?pageNumber=1&pageSize=10
```

## Publicar frontend Vue Vite en Netlify

1. Entrar a Netlify.
2. Seleccionar **Add new site > Import an existing project**.
3. Conectar GitHub y seleccionar el repositorio.
4. Configurar:

```text
Base directory: frontend
Build command: npm run build
Publish directory: frontend/dist
```

5. Agregar variable de entorno:

```text
VITE_API_BASE_URL=https://eshop-catalog-api.onrender.com
```

6. Publicar el sitio.
7. Abrir la URL de Netlify y probar:
   - Buscar producto por nombre.
   - Insertar producto.
   - Actualizar producto por nombre.
   - Eliminar producto por nombre.

## Guion para video de maximo 15 minutos

### 0:00 - 1:00 Introduccion

"Este proyecto cumple los requerimientos: API en nube, base de datos publicada, frontend Vue Vite en Netlify, CRUD de productos por nombre, filtros, Basket y Redis cache."

### 1:00 - 3:00 Arquitectura

Mostrar Visual Studio:

- `Catalog.API`
- `Basket.API`
- `BuildingBlocks`
- `frontend`
- `docker-compose`
- `render.yaml`
- `netlify.toml`

Explicar:

- `Catalog.API` maneja productos.
- `Basket.API` maneja carritos.
- `Basket.API/Data/CachedBasketRepository.cs` usa Redis cache.
- PostgreSQL guarda datos con Marten.

### 3:00 - 6:00 Render

Mostrar Render:

- Blueprint creado desde `render.yaml`.
- Dos Postgres.
- Un Key Value Redis.
- Dos Web Services.

Abrir:

```text
/health
/products/search?pageNumber=1&pageSize=10
```

### 6:00 - 9:00 Netlify

Mostrar Netlify:

- Build command: `npm run build`.
- Publish directory: `dist`.
- Variable: `VITE_API_BASE_URL`.

Abrir el sitio publicado.

### 9:00 - 13:00 Pruebas funcionales

En el frontend:

1. Insertar producto.
2. Buscar producto por nombre.
3. Actualizar producto por nombre.
4. Eliminar producto por nombre.

Opcional en Postman/Thunder Client:

```text
POST /basket
GET /basket/{userName}
DELETE /basket/{userName}
```

### 13:00 - 15:00 Cierre

"Con esto queda publicada la base de datos, las APIs, el frontend en Vue Vite y Basket usando Redis cache. El proyecto se puede volver a desplegar automáticamente desde GitHub."

## Nota importante

Render Free Postgres expira despues de 30 dias. Para una entrega escolar sirve; para que dure mas, usar Neon Postgres y configurar manualmente `ConnectionStrings__Database` en cada API.
