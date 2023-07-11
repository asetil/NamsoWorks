package main

import (
	"fmt"
	"math"
)

func testDeclarations() {
	//Print to output
	fmt.Println("Hello world!")
	fmt.Println("Hello world!:", math.Ceil(34.45))

	//Variables
	const PI = 3.14
	var age int = 32 //A declared variable must be used otherwise we get compile error
	var bloodPressure float64 = 1.0023
	var card string = "Ace of Spades" //or
	card2 := "Ace of Spades"          //Declare and assign a variable with := operator
	fmt.Println(PI, card, card2, age, bloodPressure)

	var name string
	name = "Osman"
	fmt.Println(name)

	//Multiple variable declaretion
	car, price := "Audi", 3000
	fmt.Println(car, price)

	var i, j int = 5, 8
	j, i = i, j //swapping values
	fmt.Println(i, j)

	var (
		salary     float64
		department string
	)
	fmt.Println(salary, department)

	//Type Convcersion
	var a = 5
	var b = 4.2
	a = int(b)
	fmt.Println(a, b)

	//*** Go is a statically and strong typed language. different types cannot be assigned to each other.
	//*** In GO all varables are initialized!!!

	//Function call
	//_ is the blank identifier, if you will not use the variable returned from func you can use _
	fc, _ := getNewCard()
	fmt.Println(fc)

	// fmt.Printf() prints out to stdout according to a format specifier called verb.
	// It doesn't add a newline (\n)

	// VERBS:
	// %d -> decimal
	// %f -> float
	// %s -> string
	// %q -> double-quoted string
	// %v -> value (any)
	// %#v -> a Go-syntax representation of the value
	// %T -> value Type
	// %t -> bool (true or false)
	// %p -> pointer (address in base 16, with leading 0x)
	// %c -> char (rune) represented by the corresponding Unicode code point
}

func getNewCard() (string, error) {
	return "Five of Spades", nil
}
