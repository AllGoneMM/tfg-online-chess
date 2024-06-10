var chessBoard;
var signalRConnection;
document.addEventListener('DOMContentLoaded', function () {

    chessBoard = Chessboard('chessBoard', {
        pieceTheme: 'assets/img/chesspieces/{piece}.png',
        draggable: true,
        position: 'start',
        moveSpeed: 'slow',
        snapbackSpeed: 500,
        snapSpeed: 100,
        onDrop: onDrop
    });
    adjustContentHeight();
    chessBoard.resize();
    let chessBoardPlayersContainer = document.getElementById('chessBoardPlayersContainer');

    signalRConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/offlinegame", signalR.HttpTransportType.WebSockets)
        .withAutomaticReconnect()
        .build();

    signalRConnection.start()
        .catch(() => alert("Error al abrir la conexión"));
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
    let marginTop = parseInt(computedStyle.marginTop); // Parse the margin-top value to an integer
    let marginBottom = parseInt(computedStyle.marginBottom); // Parse the margin-bottom value to an integer
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

async function onDrop(source, target, piece, newPos, oldPos, orientation) {
    let move = source + target;

    // Check move legality via SignalR
    await signalRConnection.invoke("IsLegalMove", move).then(function (response) {
        if (response === true) {
            // If move is legal, update the game state
            chessBoard.move(source, target);
        }
        else {
            return 'snapback';
        }
    }).catch(function (err) {
        console.error(err.toString());
        // If there is an error, also revert the move
        chessBoard.position(oldPos);
    });


    // Return 'snapback' to prevent the piece from being placed in the new position temporarily
}


