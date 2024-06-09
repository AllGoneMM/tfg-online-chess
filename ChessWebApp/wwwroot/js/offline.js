var board2;
window.onload = function () {
    adjustContentHeight();
    board2 = Chessboard('board2', {
        pieceTheme: 'assets/img/chesspieces/{piece}.png',
        draggable: true,
        dropOffBoard: 'trash',
    })

    $('#startBtn').on('click', board2.start)
    $('#clearBtn').on('click', board2.clear)
}

window.onresize = function () {
    adjustContentHeight();
    board2.resize();
}
function adjustContentHeight() {
    var navbarHeight = document.getElementById('navbar').offsetHeight;
    let rowContent = document.getElementById('boardContent');
    rowContent.style.cssText = "max-height: calc(100dvh - " + navbarHeight + "px) !important;";
}