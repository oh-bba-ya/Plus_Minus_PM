function LobbyManager(io){
  var LbMg = this;
  LbMg.people = [];

  LbMg.push = function(data){
      LbMg.people.push(data)
      console.log("현재 매칭 인원 : ", LbMg.people.length);
  };

  LbMg.kick = function(socket){
    var kickIndex = -1;

    for(var i = 0; i < LbMg.people.length; i++){
      if(LbMg.people[i].socket == socket){
        kickIndex = i;
      }
    }

    if(kickIndex >= 0){
      LbMg.people.splice(i, 1);
    }
  };

  LbMg.dispatch = function(RmMg){
    if(LbMg.dispatching){
      return;
    }

    LbMg.dispatching = true;

    if(LbMg.people.length >= 5){
      RmMg.create(LbMg.people.splice(0, 5));
    }
    LbMg.dispatching = false;
  };
}

module.exports = LobbyManager;
