const mongoose = require("mongoose");
const passportLocalMongoose = require("passport-local-mongoose");

var userSchema = mongoose.Schema(
{
    email : {type : String , required : true , unique : true}, 
    amigos: [{ type: mongoose.Schema.Types.ObjectId, ref: "User" }]//Array de ids de utilizadores
});

userSchema.plugin(passportLocalMongoose);//Adiciona uma autenticação local ao esquema

module.exports = mongoose.model("User",userSchema);