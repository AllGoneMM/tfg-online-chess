var board2;
document.addEventListener('DOMContentLoaded',
    function() {
        board2 = Chessboard('board2', {
            pieceTheme: 'assets/img/chesspieces/{piece}.png',
            draggable: true,
            dropOffBoard: 'trash'
        });

        document.getElementById('startBtn').addEventListener('click', board2.start);
        document.getElementById('clearBtn').addEventListener('click', board2.clear);
        adjustContentHeight();
        board2.resize();
    });

window.onresize = function () {
    adjustContentHeight();
    board2.resize();
};

function adjustContentHeight() {
    let boardContainer = document.getElementById('boardContainer');
    let navbarHeight = document.getElementById('navbar').offsetHeight;
    let opponentDivHeight = document.getElementById('opponentDiv').offsetHeight;
    let userDivHeight = document.getElementById('userDiv').offsetHeight;
    let availableHeight = window.innerHeight - navbarHeight - opponentDivHeight - userDivHeight;
    let availableWidth = boardContainer.offsetWidth;

    let boardSize = Math.min(availableHeight, availableWidth);
    document.getElementById('board2').style.width = `${boardSize}px`;
}