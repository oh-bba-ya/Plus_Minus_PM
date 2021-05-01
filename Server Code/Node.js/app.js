var io = require('socket.io')({
	transports: ['websocket'],
});
io.attach(4000);
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
		console.log(roomManager.rooms[roomNum].playerCards);
		socket.emit('gameReady', {arrPlayer : roomManager.rooms[roomNum].playerCards});
	});

	socket.on('decision', function(data){
		var roomNum = roomManager.roomIndex[socket.id];
		var room = roomManager.rooms[roomNum];

		for(var i = 0; i < 3; i++){
			room.playerCards[data.index][i] = data.decision[i];
		}
		room.playerDecision[data.index] = true;
		console.log(data.index, "번 : ", data.decision[0], ", " , data.decision[1], ", ", data.decision[2]);

		var allDone = true;
		for(var i = 0; i < 5; i++){
			if(room.playerDecision[i] == false){
				allDone = false;
				break;
			}
		}

		if(allDone){
			console.log("모든 유저 선택 완료");
			console.log(roomManager.rooms[roomNum].playerCards)
			io.to(roomNum).emit('gameReady', {arrPlayer : roomManager.rooms[roomNum].playerCards});
			console.log("0번 차례");
			room.players[0].socket.emit('yourTurn', {turn : 0})
		}
	});

	socket.on('gameResult', function(data){
		var roomNum = roomManager.roomIndex[socket.id];
		var room = roomManager.rooms[roomNum];
		var result = data.playerResult;

		for (var i = 0; i < 5; i++){
			room.playerScore[i] = result[i];
			console.log(i, "번의 결과 : ", result[i]);
		}
	});

	socket.on('betting', function(data){
		var roomNum = roomManager.roomIndex[socket.id];
		var room = roomManager.rooms[roomNum];
		var index = data.index;
		var betMoney = data.betMoney;

		if(betMoney == -1){
			console.log(index, "번 다이");
			room.playerDieStatus[index] = true;
			socket.broadcast.to(roomNum).emit('die', {dieIndex : index})
		}
		else{
			console.log(index, "번이 ", betMoney, "원 배팅");
			room.playerBetStatus[index] = betMoney;
			room.totalMoney += data.betMoney;
			socket.broadcast.to(roomNum).emit('betting', data);
		}

		index = (index + 1) % 5;
		console.log(index);
		var gameDone = true;
		if(index == 0){
			var betNormal = Math.max.apply(null, room.playerBetStatus);
			console.log("가장 큰 배팅 금액 : ", betNormal);
			for(var i = 0; i < 5; i++){
				if(room.playerDieStatus[i] == false && betNormal != room.playerBetStatus[i]){
					gameDone = false;
				}
			}
			for(var i = 0; i < 5; i++){
				room.playerBetStatus[i] = 0;
			}
		}
		else{
			gameDone = false;
		}

		if(gameDone){
			console.log("페이즈 종료");
			for(var i = 0; i < 5; i++){
				room.players[i].socket.emit("paseEnd", {a : "a"});
				room.playerBetStatus[i] = 0;
			}
			while(room.playerDieStatus[index]){
				index+=1;
			}
			console.log(index, "번 차례");
			room.players[index].socket.emit("yourTurn", {turn : index});
		}
		else{
			while(room.playerDieStatus[index]){
				index+=1;
				console.log(index);
			}
			console.log(index, "번 차례");
			room.players[index].socket.emit("yourTurn", {turn : index});
		}
	});

	socket.on('result', (data)=>{
		if(matching){
			console.log("게임 끝");
			var roomNum = roomManager.roomIndex[socket.id];
			var room = roomManager.rooms[roomNum];

			var winnerIndex = -1;
			var liveArray = [];
			for (var i = 0; i < 5; i++)
			{
				if(room.playerDieStatus[i] == false){
					liveArray.push(room.playerScore[i]);
				}
				else{
					liveArray.push(0);
				}
			}
			var maxScore = Math.max.apply(null, liveArray);
			for(var i = 0; i < 5; i++){
				if(maxScore == liveArray[i]){
					winnerIndex = i;
				}
			}

			console.log("승리자는 ", winnerIndex, "번 입니다");

			if(room != undefined)
			{
				if(data.index == winnerIndex){
					room.players[data.index].socket.emit('gameEnd', {addMoney : room.totalMoney});
				}
				else{
					room.players[data.index].socket.emit('gameEnd', {addMoney : 0});
				}
			}

			if(room.playerNum == 0){
				roomManager.destroy(roomNum, lobbyManager);
			}
			else{
				room.playerNum -= 1;
			}
			matching = false;
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
