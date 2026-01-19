const API_URL = "https://localhost:7178/api/clientes";

let modalCliente, modalEliminar, toast;

document.addEventListener("DOMContentLoaded", () => {
  modalCliente = new bootstrap.Modal(document.getElementById("modalCliente"));
  modalEliminar = new bootstrap.Modal(document.getElementById("modalEliminar"));
  toast = new bootstrap.Toast(document.getElementById("toast"));

  cargarClientes();
  initEventListeners();
});

function initEventListeners() {
  document
    .getElementById("btnNuevoCliente")
    .addEventListener("click", abrirModalNuevo);
  document
    .getElementById("btnGuardarCliente")
    .addEventListener("click", guardarCliente);
  document
    .getElementById("btnConfirmarEliminar")
    .addEventListener("click", eliminarCliente);
  document
    .getElementById("inputBuscar")
    .addEventListener("input", debounce(buscarClientes, 300));
  document
    .getElementById("btnLimpiarBusqueda")
    .addEventListener("click", () => {
      document.getElementById("inputBuscar").value = "";
      cargarClientes();
    });
}

async function cargarClientes() {
  try {
    const response = await fetch(API_URL);
    if (!response.ok) throw new Error("Error al cargar clientes");
    const clientes = await response.json();
    renderizarTabla(clientes);
  } catch (error) {
    mostrarToast("Error", error.message, "danger");
    renderizarTabla([]);
  }
}

async function buscarClientes() {
  const termino = document.getElementById("inputBuscar").value.trim();
  if (!termino) {
    cargarClientes();
    return;
  }
  try {
    const response = await fetch(
      `${API_URL}/buscar?termino=${encodeURIComponent(termino)}`,
    );
    if (!response.ok) throw new Error("Error al buscar");
    const clientes = await response.json();
    renderizarTabla(clientes);
  } catch (error) {
    mostrarToast("Error", error.message, "danger");
  }
}

function renderizarTabla(clientes) {
  const tbody = document.getElementById("tablaClientes");
  if (clientes.length === 0) {
    tbody.innerHTML = `
            <tr>
                <td colspan="7" class="text-center py-4 text-muted">
                    <i class="bi bi-inbox fs-1 d-block mb-2"></i>
                    No hay clientes registrados
                </td>
            </tr>`;
    return;
  }
  tbody.innerHTML = clientes
    .map(
      (c) => `
        <tr>
            <td>${c.id}</td>
            <td>${c.nombres}</td>
            <td>${c.cedula}</td>
            <td>${c.direccion}</td>
            <td>${c.email}</td>
            <td>${c.telf}</td>
            <td class="text-center">
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" onclick="abrirModalEditar(${c.id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-outline-danger" onclick="abrirModalEliminar(${c.id})">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `,
    )
    .join("");
}

function abrirModalNuevo() {
  document.getElementById("formCliente").reset();
  document.getElementById("clienteId").value = "";
  document.getElementById("modalClienteTitulo").innerHTML =
    '<i class="bi bi-person-plus me-2"></i>Nuevo Cliente';
  modalCliente.show();
}

async function abrirModalEditar(id) {
  try {
    const response = await fetch(`${API_URL}/${id}`);
    if (!response.ok) throw new Error("Cliente no encontrado");
    const cliente = await response.json();

    document.getElementById("clienteId").value = cliente.id;
    document.getElementById("nombres").value = cliente.nombres;
    document.getElementById("cedula").value = cliente.cedula;
    document.getElementById("direccion").value = cliente.direccion;
    document.getElementById("email").value = cliente.email;
    document.getElementById("telf").value = cliente.telf;
    document.getElementById("modalClienteTitulo").innerHTML =
      '<i class="bi bi-pencil me-2"></i>Editar Cliente';
    modalCliente.show();
  } catch (error) {
    mostrarToast("Error", error.message, "danger");
  }
}

async function guardarCliente() {
  const form = document.getElementById("formCliente");
  if (!form.checkValidity()) {
    form.reportValidity();
    return;
  }

  const id = document.getElementById("clienteId").value;
  const cliente = {
    nombres: document.getElementById("nombres").value,
    cedula: document.getElementById("cedula").value,
    direccion: document.getElementById("direccion").value,
    email: document.getElementById("email").value,
    telf: document.getElementById("telf").value,
  };

  try {
    let response;
    if (id) {
      cliente.id = parseInt(id);
      response = await fetch(`${API_URL}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(cliente),
      });
    } else {
      response = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(cliente),
      });
    }

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || "Error al guardar");
    }

    modalCliente.hide();
    cargarClientes();
    mostrarToast(
      "Exito",
      id ? "Cliente actualizado" : "Cliente creado",
      "success",
    );
  } catch (error) {
    mostrarToast("Error", error.message, "danger");
  }
}

function abrirModalEliminar(id) {
  document.getElementById("eliminarClienteId").value = id;
  modalEliminar.show();
}

async function eliminarCliente() {
  const id = document.getElementById("eliminarClienteId").value;
  try {
    const response = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (!response.ok) throw new Error("Error al eliminar");

    modalEliminar.hide();
    cargarClientes();
    mostrarToast("Exito", "Cliente eliminado", "success");
  } catch (error) {
    mostrarToast("Error", error.message, "danger");
  }
}

function mostrarToast(titulo, mensaje, tipo) {
  const toastEl = document.getElementById("toast");
  const icon = document.getElementById("toastIcon");

  toastEl.className = `toast bg-${tipo} text-white`;
  icon.className = `bi bi-${tipo === "success" ? "check-circle" : "exclamation-circle"} me-2`;
  document.getElementById("toastTitulo").textContent = titulo;
  document.getElementById("toastMensaje").textContent = mensaje;
  toast.show();
}

function debounce(func, wait) {
  let timeout;
  return function (...args) {
    clearTimeout(timeout);
    timeout = setTimeout(() => func.apply(this, args), wait);
  };
}
