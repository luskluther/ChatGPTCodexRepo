document.addEventListener('DOMContentLoaded', function () {
    var tile = document.getElementById('toggle-tile');
    tile.addEventListener('click', function () {
        tile.classList.toggle('active');
        tile.textContent = tile.classList.contains('active') ? 'Toggled!' : 'Click me to toggle!';
    });
});
