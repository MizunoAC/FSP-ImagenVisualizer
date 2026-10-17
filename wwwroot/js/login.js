
async function Login() {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const errorElement = document.getElementById("login-error");

    errorElement.hidden = true;
    errorElement.textContent = "";

    try {
        const response = await fetch("/Account/Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            },
            body: JSON.stringify({
                username: username,
                password: password
            })
        });

        const contentType = response.headers.get("content-type") || "";

        if (!contentType.includes("application/json")) {
            throw new Error("El servidor devolvió una respuesta inesperada.");
        }

        const result = await response.json();

        if (!response.ok || result.error === true) {
            errorElement.textContent =
                result.message || "Error al iniciar sesión.";

            errorElement.hidden = false;
            return;
        }

        window.location.href = "/Home/Index";

    } catch (error) {
        errorElement.textContent =
            error.message === "Failed to fetch"
                ? "No se pudo conectar con el servidor."
                : error.message || "Ocurrió un error inesperado.";

        errorElement.hidden = false;
        console.error("Error en el login:", error);
    }
}
