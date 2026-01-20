
function incluirScript(url, callback) {
  const script = document.createElement('script');
  script.src = url;
  script.onload = callback; // Ejecuta el callback cuando el script cargue
  document.body.appendChild(script);
}

incluirScript('_content/Microfrontends.Shared.UI/js/jquery.min.js', () => {
  console.log("bootstrap.bundle.min.js se ha cargado.");
  // Aquí puedes llamar funciones de admin.js si admin.js las exporta
  // o si las define globalmente (no recomendado).
});

// Para cargar admin.js y luego ejecutar algo:
incluirScript('_content/Microfrontends.Shared.UI/js/bootstrap.bundle.min.js', () => {
  console.log("bootstrap.bundle.min.js se ha cargado.");
  // Aquí puedes llamar funciones de admin.js si admin.js las exporta
  // o si las define globalmente (no recomendado).
});