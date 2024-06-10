document.addEventListener("DOMContentLoaded", function () {
    chessBoard = Chessboard('chessBoard', {
        pieceTheme: 'assets/img/chesspieces/{piece}.png',
        position: 'start'
    });
    adjustContentHeight();
    chessBoard.resize();
});

window.addEventListener('resize', function () {
    adjustContentHeight();
    chessBoard.resize();
});

function adjustContentHeight() {
    var navbarHeight = document.getElementById('navbar').offsetHeight;
    let rowContent = document.getElementById('chessBoard');
    let chessBoardPlayersContainer = document.getElementById('chessBoardPlayersContainer');
    let computedStyle = window.getComputedStyle(chessBoardPlayersContainer);
    let marginTop = parseInt(computedStyle.marginTop);
    let marginBottom = parseInt(computedStyle.marginBottom);
    let differenceHeight = chessBoardPlayersContainer.offsetHeight - rowContent.offsetHeight;

    let maxHeight = "calc(100vh - " + navbarHeight + "px - " + marginTop + "px - " + marginBottom + "px - " + differenceHeight + "px)";
    let maxWidth = "calc(100vh - " + navbarHeight + "px - " + marginTop + "px - " + marginBottom + "px - " + differenceHeight + "px)";

    rowContent.style.maxHeight = maxHeight;
    rowContent.style.maxWidth = maxWidth;

    let div1 = document.getElementById('div1');
    let div2 = document.getElementById('div2');
    div1.style.width = rowContent.offsetWidth + "px";
    div2.style.width = rowContent.offsetWidth + "px";
}