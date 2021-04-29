from flask_sqlalchemy import SQLAlchemy

db = SQLAlchemy()

class PMUser(db.Model): 
    __tablename__ = 'pmUser'
    id = db.Column(db.Integer, primary_key = True)
    password = db.Column(db.String(12))
    userid = db.Column(db.String(8))
    username = db.Column(db.String(8))
    money = db.Column(db.Integer)