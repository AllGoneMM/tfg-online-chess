var chessBoard;
var signalRConnection;

document.addEventListener('DOMContentLoaded', async function () {

    document.getElementById("startGame").addEventListener("click", async function () {
        try {
            let serverJson = await signalRConnection.invoke("StartGame");
            chessBoard = Chessboard('chessBoard', {
                pieceTheme: 'assets/img/chesspieces/{piece}.png',
                draggable: true,
                position: 'start',
                onDrop: await onDrop
            });
        } catch (err) {
            alert("Error al iniciar una nueva partida: " + err.toString());
        }
    });


    signalRConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/offlinegame", signalR.HttpTransportType.WebSockets)
        .withAutomaticReconnect()
        .build();

    signalRConnection.start()
        .catch(error => alert("Error al abrir la conexión: " + error.toString()));
});

async function onDrop(source, target, piece, newPos, oldPos, orientation) {
    let move = source + target;
    return 'snapback'
    try {
        let chessGame = await signalRConnection.invoke("ProcessMove", move);

        chessGame = JSON.parse(chessGame);
        let fen = chessGame.Fen;
        let state = chessGame.State;
        let promotion = chessGame.Promotion;
        chessBoard.position(fen);
        if(state != 1) {
            const exampleModal = new bootstrap.Modal(document.getElementById('exampleModal'))
            exampleModal.show();
        }
    } catch (err) {
        chessBoard.position(oldPos);
    }
}
