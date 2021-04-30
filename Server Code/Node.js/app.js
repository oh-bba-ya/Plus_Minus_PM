var io = require('socket.io')({
	transports: ['websocket'],
});
io.attach(4000);
var userList = [];
console.log('server is on port 4000');

var lobbyManager = new (require('./LobbyManager.js'))(io);
var roomManager = new (require('./RoomManager.js'))(io);

io.on('connection', function(socket){
	var matching = false;
	var nickname;

	socket.on('join', function(data){
		nickname = data.nickname;
		console.log(nickname, "참가");
		socket.emit('join', data);
	});

	socket.on('pick', function(data){
		var nickname = data.nickname;
		console.log(nickname, "매칭 실시");
		matching = true;
		lobbyManager.push({socket, nickname});
		lobbyManager.dispatch(roomManager);
	});

	socket.on('gameReady', function(){
		var roomNum = roomManager.roomIndex[socket.id];

		io.to(roomNum).emit('gameReady', {arrPlayer : roomManager.rooms[roomNum].playerCards});
	});

	socket.on('decision', function(data){
		var roomNum = roomManager.roomIndex[socket.id];
		var room = roomManager.rooms[roomNum];

		for(var i = 0; i < 3; i++){
			room.playerCards[data.index][i] = data.decision[i];
		}
		room.playerDecision[data.index] = true;

		var allDone = true;
		for(var i = 0; i < 5; i++){
			if(room.playerDecision[i] == false){
				allDone = false;
				break;
			}
		}

		if(allDone){
			io.to(roomNum).emit('gameReady', {arrPlayer : roomManager.rooms[roomNum].playerCards});
		}
	});

	socket.on('betting', function(data){
		var roomNum = roomManager.roomIndex[socket.id];
		var room = roomManager.rooms[roomNum];
		var index = data.index;
		var betMoney = data.betMoney;

		room.totalMoney = data.totalMoney;
		if(betMoney == -1){
			socket.broadcast.to(roomNum).emit('die', {dieIndex : index})
		}
		else{
			room.playerBetStatus[index] = betMoney;
			socket.broadcast.to(roomNum).emit('betting', data);
		}

		index = (index + 1) % 5;
		var gameDone = true;
		if(index == 0){
			var betNormal = room.playerBetStatus[0];
			for(var i = 1; i < 5; i++){
				if(betNormal != room.playerBetStatus[i]){
					gameDone = false;
				}
			}
		}

		if(gameDone){
			for(var i = 0; i < 5; i++){
				room.players[i].socket.emit("paseEnd");
			}
		}
		else{
			room.players[index].socket.emit("yourTurn", {turn : index});
		}
	});

	socket.on('result', (data)=>{
			var roomNum = roomManager.roomIndex[socket.id];
			var room = roomManager.rooms[roomNum];
			if(room != undefined)
			{
				if(data.index == room.winnerIndex){
					room.players[data.index].socket.emit('gameEnd', {addMoney : room.totalMoney});
				}
				else{
					room.players[data.index].socket.emit('gameEnd', {addMoney : 0});
				}
			}

			if(roomNum){
				roomManager.rooms[roomNum].playerCnt--;
				roomManager.destroy(roomNum, lobbyManager);
			}
	});

	socket.on('disconnect', function(){
		var roomNum = roomManager.roomIndex[socket.id];

    if(roomNum){
			roomManager.rooms[roomNum].playerCnt--;
			roomManager.destroy(roomNum, lobbyManager);
    }

		if(matching){
			lobbyManager.kick(socket);
		}

		lobbyManager.dispatch(roomManager);
    console.log('user disconnected: ', socket.id);
  });
});
