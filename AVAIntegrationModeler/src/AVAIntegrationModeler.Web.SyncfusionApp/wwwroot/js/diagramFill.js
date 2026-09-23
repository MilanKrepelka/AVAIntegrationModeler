// Nastaví výšku wrapperu diagramu na zbývající výšku viewportu.
// Vrátí výslednou výšku v px jako číslo.
window.setDiagramFillHeight = function (wrapperId) {
    var el = document.getElementById(wrapperId);
    if (!el) return 700;
    var top = el.getBoundingClientRect().top;
    var h = Math.max(Math.floor(window.innerHeight - top - 2), 200);
    el.style.height = h + 'px';
    return h;
};
