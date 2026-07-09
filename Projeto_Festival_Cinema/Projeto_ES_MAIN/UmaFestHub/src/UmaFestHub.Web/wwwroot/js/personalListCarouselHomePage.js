// Home page horizontal carousels for Personal Lists (_PersonalListCarousel: #watchlist-row, #favorites-row).
// hubScrollFavorites(viewportId, deltaPx): scrolls the viewport with smooth snap behaviour.
(function () {
  function hubScrollFavorites(viewportId, deltaPx) {
    var viewport = document.getElementById(viewportId);
    if (!viewport) return;

    viewport.scrollBy({ left: deltaPx, behavior: 'smooth' });
  }

  window.hubScrollFavorites = hubScrollFavorites;

  document.addEventListener("DOMContentLoaded", function () {
    if (!window.bootstrap || !window.bootstrap.Tooltip) return;
    var triggers = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    for (var i = 0; i < triggers.length; i++) {
      new window.bootstrap.Tooltip(triggers[i]);
    }
  });
})();
