# Publicacion de base de datos, API y frontend

Esta solucion contiene:

- `Catalog.API`: API ASP.NET Core para productos.
- `Basket.API`: API ASP.NET Core para carrito.
- `BuildingBlocks`: CQRS, behaviors y excepciones compartidas.
- PostgreSQL para Catalog y Basket.
- Redis cache para Basket.
- Frontend Vue Vite en `frontend`.

## 1. Subir el proyecto a GitHub

El despliegue recomendado usa Render y Netlify conectados a un repositorio GitHub.

```powershell
cd D:\eshop-services--2.0\eshop-services
git init
git add .
git commit -m "Proyecto eShop services listo para deploy"
git branch -M main
git remote add origin https://github.com/TU_USUARIO/eshop-services.git
git push -u origin main
```

## 2. Publicar base de datos, APIs y Redis en Render

El archivo `render.yaml` en la raiz define:

- `eshop-catalog-db`: PostgreSQL para Catalog.
- `eshop-basket-db`: PostgreSQL para Basket.
- `eshop-redis-cache`: Render Key Value compatible con Redis.
- `eshop-catalog-api`: API Docker usando `src/Catalog.API/Dockerfile`.
- `eshop-basket-api`: API Docker usando `src/Basket.API/Dockerfile`.

Pasos:

1. Entrar a Render.
2. Seleccionar **New > Blueprint**.
3. Conectar GitHub.
4. Seleccionar el repositorio.
5. Confirmar que Render detecta `render.yaml`.
6. Crear el Blueprint.
7. Esperar a que terminen los deploys.

URLs a probar al terminar:

```text
https://eshop-catalog-api.onrender.com/health
https://eshop-basket-api.onrender.com/health
https://eshop-catalog-api.onrender.com/products/search?pageNumber=1&pageSize=10
```

Nota: Render Free Postgres expira despues de 30 dias. Para una entrega escolar sirve; para mayor duracion se puede cambiar a Neon Postgres y pegar manualmente los connection strings en Render.

## 3. Publicar frontend Vue Vite en Netlify

El archivo `netlify.toml` define:

```text
Base directory: frontend
Build command: npm run build
Publish directory: dist
```

Pasos:

1. Entrar a Netlify.
2. Seleccionar **Add new site > Import an existing project**.
3. Elegir GitHub y el repositorio.
4. Configurar:

```text
Base directory: frontend
Build command: npm run build
Publish directory: dist
```

5. Agregar variable de entorno:

```text
VITE_API_BASE_URL=https://eshop-catalog-api.onrender.com
```

6. Publicar.
7. Abrir la URL `.netlify.app` y probar el CRUD.

## 4. Requerimientos funcionales del PDF

- Buscar producto paginado por nombre:

```text
GET /products/search?name=zapato&pageNumber=1&pageSize=10
```

- Buscar por filtro/categoria:

```text
GET /products/category/{category}
```

- Insertar producto:

```text
POST /products
```

- Actualizar producto por nombre:

```text
PUT /products/{name}
```

- Eliminar producto por nombre:

```text
DELETE /products/{name}
```

- Basket:

```text
POST /basket
GET /basket/{userName}
DELETE /basket/{userName}
```

- Redis cache:

```text
Basket.API/Data/CachedBasketRepository.cs
```

## 5. Comandos de validacion local

```powershell
cd D:\eshop-services--2.0\eshop-services
dotnet build .\eshop-services.sln
docker compose config

cd .\frontend
npm install
npm run build
```
