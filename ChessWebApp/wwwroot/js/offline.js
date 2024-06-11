var chessBoard;
var chessGame;
var signalRConnection;
var waiting;

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
                    position: 'start',
                    onDrop: onDrop,
                    onSnapEnd: onSnapEnd,
                    onDragStart: onDragStart
                });
            })
            .catch((error) => {
                window.alert("Error al iniciar la partida " + error.toString());
            });
    });
});

async function onDrop(source, target, piece, newPos, oldPos, orientation) {
    await removeGreySquares();
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
async function onSnapEnd() {
    chessBoard.position(chessGame);
    waiting = true;
    await signalRConnection.invoke("GetStockfishMove")
        .then((response) => {
            chessBoard.position(response);
            waiting = false;
        })
        .catch((error) => {
            console.error("Error processing move:", error);
            return 'snapback';
        });
}
async function removeGreySquares() {
    $('#chessBoard .square-55d63').css('background', '')
}

async function onDragStart(source, piece, position, orientation) {
    if (waiting) {
        return false;
    }
    await signalRConnection.invoke("GetLegalMoves", source)
        .then(async (response) => {
            console.log("legal moves:" + response)
            response = JSON.parse(response);
            for (var i = 0; i < response.length; i++) {
                await greySquare(response[i]);
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