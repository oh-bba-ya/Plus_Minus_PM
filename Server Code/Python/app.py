import os
from flask import Flask
from flask import request
from flask import session
from models import db
from models import PMUser
app = Flask(__name__)

@app.route('/register', methods=['GET','POST'])
def register():
    userid = request.form.get('userid')
    username = request.form.get('username')
    password = request.form.get('password')

    pmuser = PMUser()
    pmuser.password = password
    pmuser.userid = userid
    pmuser.username = username
    pmuser.money = 100000

    db.session.add(pmuser)
    db.session.commit()

    return "Success"

@app.route('/login', methods=['GET','POST'])
def login():
    userid = request.form.get('userid')
    password = request.form.get('password')

    pmuser = PMUser.query.filter_by(userid=userid).first()
    if pmuser != None and pmuser.password == password :
        return {"result" : "Success", "nickname" : pmuser.username, "money" : int(pmuser.money)}
    else :
        return {"result" : "Fail", "nickname" : "none", "money" : 0}

@app.route('/refresh', methods=['GET','POST'])
def refresh():
    userid = request.form.get('userid')
    money = request.form.get('money')

    pmuser = PMUser.query.filter_by(userid=userid).first()
    pmuser.money = int(money)

    db.session.commit()
    return "Success"


if __name__ == "__main__":
    basedir = os.path.abspath(os.path.dirname(__file__))
    dbfile = os.path.join(basedir, 'db.sqlite')
    app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///' + dbfile
    app.config['SQLALCHEMY_COMMIT_ON_TEARDOWN'] = True
    app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False

    db.init_app(app)
    db.app = app
    db.create_all()

    app.run(host='0.0.0.0', port=5000, debug=True)
