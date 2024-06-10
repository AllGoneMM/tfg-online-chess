var chessBoard;
var chessGame;
var signalRConnection;
document.addEventListener('DOMContentLoaded', async function () {
    // Start the connection with OfflineGameHub
    signalRConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/offlinegame", signalR.HttpTransportType.WebSockets)
        .withAutomaticReconnect()
        .build();
    signalRConnection.start()
        .catch(error => alert("Error al conectar con el servidor"));

    // Event listener for the start game button
    document.getElementById("startGame").addEventListener("click", async function () {

        let serverJson = await signalRConnection.invoke("StartGame")
            .then(() => {
                chessBoard = Chessboard('chessBoard', {
                    pieceTheme: 'assets/img/chesspieces/{piece}.png',
                    draggable: true,
                    moveSpeed: 'slow',
                    snapbackSpeed: 500,
                    snapSpeed: 100,
                    position: 'start',
                    onDrop: onDrop,
                    onSnapEnd: onSnapEnd,
                    onDragStart: onDragStart
                });
            })
            .catch((error) => {
                window.alert("Error al iniciar la partida "+error.toString());
            });
    });
});

async function onDrop(source, target, piece, newPos, oldPos, orientation) {
    removeGreySquares();
    let move = source + target;
    return await signalRConnection.invoke("ProcessMove", move)
        .then((response) => {
            response = JSON.parse(response);
            if (!response.Success) {
                return 'snapback';
            }
            chessGame = response.Fen;
        })
        .catch((error) => {
            console.error("Error processing move:", error);
            return 'snapback';
        });
}
function onSnapEnd() {
    chessBoard.position(chessGame);
    signalRConnection.invoke("GetStockfishMove")
        .then((response) => {
            chessBoard.position(response);
        })
        .catch((error) => {
            console.error("Error processing move:", error);
            return 'snapback';
        });
}
function removeGreySquares() {
    $('#chessBoard .square-55d63').css('background', '')
}

async function onDragStart(source, piece, position, orientation) {
    await signalRConnection.invoke("GetLegalMoves", source)
        .then((response) => {
            response = JSON.parse(response);
            for (var i = 0; i < response.length; i++) {
                greySquare(response[i]);
            }
        })
        .catch(() => {
            window.alert("Error al obtener movimientos");
            return 'snapback';
        });
}

function greySquare(square) {
    var $square = $('#chessBoard .square-' + square);

    var background = '#a9a9a9';
    if ($square.hasClass('black-3c85d')) {
        background = '#696969';
    }

    $square.css('background', background);
}