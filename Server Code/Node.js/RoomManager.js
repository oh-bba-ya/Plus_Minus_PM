function RoomManager(io){
  var RmMg = this;
  var roomCount = 0;
  RmMg.rooms = [];
  RmMg.roomIndex = [];
  RmMg.create = function(players){
    var roomNum = roomCount++;
    var room = new Room(roomNum, players);
    console.log("Room ", roomNum, "생성");

    for(var p of players){
      p.socket.join(roomNum);
    }

    RmMg.rooms[roomNum] = room;

    for(var p of players){
      RmMg.roomIndex[p.socket.id] = roomNum;
    }

    var tmpArray = new Array(55);

    for (var i = 0; i < 55; i++){
      tmpArray[i] = i;
    }

    for(var i = 0; i < 15; i++){
      var rand1 = Math.floor(Math.random() * 54) + 1;
      var rand2 = Math.floor(Math.random() * 54) + 1;

      var tmpVar = tmpArray[rand1];
      tmpArray[rand1] = tmpArray[rand2];
      tmpArray[rand2] = tmpVar;
    }

    var cnt = 1;
    for (var i = 0; i < 5; i++){
      for (var j = 0; j < 3; j++){
        room.playerCards[i][j] = tmpArray[cnt++];
      }
    }
    room.playerNum = 5;

    console.log("Room Created :", roomNum);

    for (var i = 0; i < 5; i++){
      players[i].socket.emit("pick", {yourTurn : i});
    }
  };

  RmMg.destroy = function(roomNum, LbMg){
      var room = RmMg.rooms[roomNum];
      room.players.forEach(function(data){
        data.socket.emit('destroy', {a : "a"});
  			LbMg.kick(data.socket);
        delete RmMg.roomIndex[data.socket.id];
      });
      console.log("Room ", roomNum, "삭제");
      delete RmMg.rooms[roomNum];
  };
}

function Room(num, players)
{
  this.num = num;
  this.players = players;
  this.playerNum = 0;
  this.playerCards = new Array(5);
  for(var i = 0; i < 5; i++){
    this.playerCards[i] = new Array(3);
  }

  this.phase = 1;
  this.totalMoney = 0;
  this.playerScore = new Array(5);
  for(var i = 0; i < 5; i++){
    this.playerScore[i] = 0;
  }
  this.playerDecision = new Array(5);
  for(var i = 0; i < 5; i++){
    this.playerDecision[i] = false;
  }
  this.playerBetStatus = new Array(5);
  for(var i = 0; i < 5; i++){
    this.playerBetStatus[i] = 0;
  }
  this.playerDieStatus = new Array(5);
  for(var i = 0; i < 5; i++){
    this.playerDieStatus[i] = false;
  }
}

module.exports = RoomManager;
