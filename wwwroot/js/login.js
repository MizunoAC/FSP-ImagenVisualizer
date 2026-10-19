const LOGIN_URL = "/Account/Login";
const HOME_URL = "/Gallery/Index"; 

function showError(message) {
    document.getElementById("login-error-message").textContent = message;
    document.getElementById("login-error").hidden = false;
}

function hideError() {
    document.getElementById("login-error").hidden = true;
}

async function Login() {
    const button = document.getElementById("login-button");
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;

    hideError();
    button.disabled = true;
    button.textContent = "Iniciando Sesion, Espere…";

    let redirecting = false;

    try {
        const response = await fetch(LOGIN_URL, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            },
            body: JSON.stringify({ username: username, password: password })
        });

        const isJson = (response.headers.get("content-type") || "").includes("application/json");
        const result = isJson ? await response.json() : null;

        if (!result) {
            showError("El servidor devolvió una respuesta inesperada.");
            return;
        }

        if (!response.ok || result.error === true) {
            showError(result.message || "No se pudo iniciar sesión.");
            return;
        }

        redirecting = true;
        window.location.href = HOME_URL;
    } catch (error) {
        console.error("Login error:", error);

        showError(error instanceof TypeError
            ? "No se pudo conectar con el servidor."
            : "Ocurrió un error inesperado.");
    } finally {
        if (!redirecting) {
            button.disabled = false;
            button.textContent = "Entrar";
        }
    }
}