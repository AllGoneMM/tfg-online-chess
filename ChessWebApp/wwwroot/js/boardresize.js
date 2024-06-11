document.addEventListener("DOMContentLoaded", async function () {
    chessBoard = Chessboard('chessBoard', {
        pieceTheme: 'assets/img/chesspieces/{piece}.png',
        position: 'start'
    });
    adjustContentHeight();
    await chessBoard.resize();
});

window.addEventListener('resize', async function () {
    adjustContentHeight();
    await chessBoard.resize();
});

function adjustContentHeight() {
    var navbarHeight = document.getElementById('navbar').offsetHeight;
    let rowContent = document.getElementById('chessBoard');
    let chessBoardPlayersContainer = document.getElementById('chessBoardPlayersContainer');
    let computedStyle = window.getComputedStyle(chessBoardPlayersContainer);
    let marginTop = parseInt(computedStyle.marginTop);
    let marginBottom = parseInt(computedStyle.marginBottom);
    let differenceHeight = chessBoardPlayersContainer.offsetHeight - rowContent.offsetHeight;

    let maxHeight = "calc(100dvh - " + navbarHeight + "px - " + marginTop + "px - " + marginBottom + "px - " + differenceHeight + "px)";
    let maxWidth = "calc(100dvh - " + navbarHeight + "px - " + marginTop + "px - " + marginBottom + "px - " + differenceHeight + "px)";
    
    rowContent.style.maxHeight = maxHeight;
    rowContent.style.maxWidth = maxWidth;

    let div1 = document.getElementById('div1');
    let div2 = document.getElementById('div2');
    div1.style.width = rowContent.offsetWidth + "px";
    div2.style.width = rowContent.offsetWidth + "px";

    let mainContainer = document.getElementById('mainContainer');
    rowContent.style.height = mainContainer.offsetHeight - navbarHeight + "px";
}