<script setup>
import { computed, onMounted, reactive, ref } from 'vue';

const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const search = reactive({
  name: '',
  pageNumber: 1,
  pageSize: 10
});

const form = reactive({
  name: '',
  description: '',
  categoryText: '',
  imageFiles: '',
  price: 0
});

const products = ref([]);
const totalCount = ref(0);
const loading = ref(false);
const saving = ref(false);
const message = ref('');
const error = ref('');

const totalPages = computed(() => Math.max(1, Math.ceil(totalCount.value / search.pageSize)));
const canGoBack = computed(() => search.pageNumber > 1);
const canGoForward = computed(() => search.pageNumber < totalPages.value);

function categoriesFromText(value) {
  return value
    .split(',')
    .map(category => category.trim())
    .filter(Boolean);
}

function setNotice(text) {
  message.value = text;
  error.value = '';
}

function setError(text) {
  error.value = text;
  message.value = '';
}

function resetForm() {
  form.name = '';
  form.description = '';
  form.categoryText = '';
  form.imageFiles = '';
  form.price = 0;
}

function editProduct(product) {
  form.name = product.name;
  form.description = product.description;
  form.categoryText = Array.isArray(product.category) ? product.category.join(', ') : '';
  form.imageFiles = product.imageFiles;
  form.price = product.price;
}

async function requestJson(path, options = {}) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers || {})
    },
    ...options
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP ${response.status}`);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

async function loadProducts() {
  loading.value = true;
  try {
    const params = new URLSearchParams({
      pageNumber: search.pageNumber,
      pageSize: search.pageSize
    });

    if (search.name.trim()) {
      params.set('name', search.name.trim());
    }

    const data = await requestJson(`/products/search?${params.toString()}`);
    products.value = data.products || [];
    totalCount.value = data.totalCount || 0;
    search.pageNumber = data.pageNumber || search.pageNumber;
    search.pageSize = data.pageSize || search.pageSize;
  } catch (err) {
    setError(`No se pudieron cargar productos: ${err.message}`);
  } finally {
    loading.value = false;
  }
}

async function submitProduct() {
  if (!form.name.trim()) {
    setError('El nombre del producto es obligatorio.');
    return;
  }

  saving.value = true;
  try {
    const payload = {
      name: form.name.trim(),
      description: form.description.trim(),
      category: categoriesFromText(form.categoryText),
      imageFiles: form.imageFiles.trim(),
      price: Number(form.price)
    };

    const existing = products.value.some(
      product => product.name.toLowerCase() === payload.name.toLowerCase()
    );

    if (existing) {
      await requestJson(`/products/${encodeURIComponent(payload.name)}`, {
        method: 'PUT',
        body: JSON.stringify({
          description: payload.description,
          category: payload.category,
          imageFiles: payload.imageFiles,
          price: payload.price
        })
      });
      setNotice('Producto actualizado correctamente.');
    } else {
      await requestJson('/products', {
        method: 'POST',
        body: JSON.stringify(payload)
      });
      setNotice('Producto insertado correctamente.');
    }

    resetForm();
    await loadProducts();
  } catch (err) {
    setError(`No se pudo guardar el producto: ${err.message}`);
  } finally {
    saving.value = false;
  }
}

async function deleteProduct(name) {
  saving.value = true;
  try {
    await requestJson(`/products/${encodeURIComponent(name)}`, { method: 'DELETE' });
    setNotice('Producto eliminado correctamente.');
    await loadProducts();
  } catch (err) {
    setError(`No se pudo eliminar el producto: ${err.message}`);
  } finally {
    saving.value = false;
  }
}

function changePage(delta) {
  search.pageNumber += delta;
  loadProducts();
}

function searchFromFirstPage() {
  search.pageNumber = 1;
  loadProducts();
}

onMounted(loadProducts);
</script>

<template>
  <main class="app-shell">
    <section class="toolbar">
      <div>
        <p class="eyebrow">eShop services</p>
        <h1>Catalogo de productos</h1>
      </div>
      <span class="api-chip">{{ API_BASE }}</span>
    </section>

    <section class="workspace">
      <form class="panel editor" @submit.prevent="submitProduct">
        <div class="panel-header">
          <h2>Producto</h2>
          <button class="secondary" type="button" @click="resetForm">Limpiar</button>
        </div>

        <label>
          Nombre
          <input v-model="form.name" autocomplete="off" placeholder="Nombre del producto" />
        </label>

        <label>
          Descripcion
          <textarea v-model="form.description" rows="4" placeholder="Descripcion breve"></textarea>
        </label>

        <label>
          Categorias
          <input v-model="form.categoryText" placeholder="Ropa, Calzado, Accesorios" />
        </label>

        <label>
          Imagen
          <input v-model="form.imageFiles" placeholder="URL o nombre de archivo" />
        </label>

        <label>
          Precio
          <input v-model.number="form.price" min="0" step="0.01" type="number" />
        </label>

        <button class="primary" :disabled="saving" type="submit">
          {{ saving ? 'Guardando...' : 'Insertar o actualizar' }}
        </button>
      </form>

      <section class="panel results">
        <div class="panel-header">
          <h2>Busqueda</h2>
          <span>{{ totalCount }} resultados</span>
        </div>

        <form class="search-row" @submit.prevent="searchFromFirstPage">
          <input v-model="search.name" placeholder="Buscar por nombre" />
          <select v-model.number="search.pageSize" @change="searchFromFirstPage">
            <option :value="5">5</option>
            <option :value="10">10</option>
            <option :value="20">20</option>
          </select>
          <button class="secondary" type="submit">Buscar</button>
        </form>

        <p v-if="message" class="notice success">{{ message }}</p>
        <p v-if="error" class="notice error">{{ error }}</p>

        <div class="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Categoria</th>
                <th>Precio</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="loading">
                <td class="empty-row" colspan="4">Cargando productos...</td>
              </tr>
              <tr v-else-if="products.length === 0">
                <td class="empty-row" colspan="4">No hay productos para mostrar.</td>
              </tr>
              <tr v-for="product in products" v-else :key="product.id">
                <td data-label="Nombre">
                  <strong>{{ product.name }}</strong>
                  <small>{{ product.description }}</small>
                </td>
                <td data-label="Categoria">{{ product.category?.join(', ') }}</td>
                <td data-label="Precio">${{ Number(product.price).toFixed(2) }}</td>
                <td class="actions" data-label="Acciones">
                  <button class="secondary" type="button" @click="editProduct(product)">Editar</button>
                  <button class="danger" type="button" @click="deleteProduct(product.name)">Eliminar</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="pagination">
          <button class="secondary" :disabled="!canGoBack" type="button" @click="changePage(-1)">
            Anterior
          </button>
          <span>Pagina {{ search.pageNumber }} de {{ totalPages }}</span>
          <button class="secondary" :disabled="!canGoForward" type="button" @click="changePage(1)">
            Siguiente
          </button>
        </div>
      </section>
    </section>
  </main>
</template>
