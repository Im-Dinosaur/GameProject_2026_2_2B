const {add} = require("./Math.js");

let num = 42;
var name = "TOM";
let isStudent = true;

let color = ["red", "green", "blue"];
let person = {name : "Alice", age : 30};

console.log(add(num,num));

function greet(name)
{
    console.log("Hello" + name + + " ! ");
}

if(num > 30)
{
    console.log("Number is Greater than 30");
}
else
{
    console.log("Number is Less than 30");
}

for (var i = 0; i < 5; i++)
{
    console.log(i);
}

setTimeout(() => {
    console.log("Delayed Message 1");
}, 1000)

setTimeout(() => {
    console.log("Delayed Message 1");
}, 750)

setTimeout(() => {
    console.log("Delayed Message 1");
}, 2000)

setTimeout(() => {
    console.log("Delayed Message 1");
}, 500)