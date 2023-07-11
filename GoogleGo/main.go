/*
	Author : @ware
*/
package main

import (
	"fmt"

	"osokuoglu.com/Decker/utils"
)

//TO RUN PRESS CTRL + F5 for VS Code
//go run : compiles and runs the app. It does not produce an executable => go run main.go
//go build : just compiles the app. It produce an executable
//	=> go build file.go compiles single file and its depenencies
//	=> go build compile files in current directory
// => go build -o osman will produce an executable file called osman

func main() {
	testDeclarations()
	//testRoutines()
	return

	var (
		INFO_MESSAGE = utils.Packet{0xAA, 0x55, 0x00, 0x00, 0x71, 0x08, 0x00, 0x55, 0xAA}
	)

	message := "Osmancım"
	resp := INFO_MESSAGE
	resp.SetLength(int16(len(message)) + 3) // packet length
	resp[6] = byte(len(message))            // message length
	resp.Insert([]byte(message), 7)         // message
	fmt.Println("IBO 1:", resp, "[", string(resp), "]", int(0x2E))
	return

	testModel()
	testTimer()
	return

	testHttp()
	return

	var ibo []string = nil

	fmt.Println("IBO 1:", ibo)

	func() {
		fmt.Println("IBO 2:", ibo)
		for it, id := range ibo {
			fmt.Println("IBO :", it, id)
		}
	}()
	return

	//Map Test
	mapTest()

	fmt.Println("==========")
	var (
		xx = []string{"1", "2", "3", "4", "5", "6"}
		yy = []string{"A", "B", "C"}
	)
	xx = append(xx[:3], append(yy, xx[4:]...)...)
	fmt.Println(xx[2:5], "|", xx)

	var OPEN_LOT = utils.Packet{0xAA, 0x55, 0x0C, 0x00, 0xA2, 0x01, 0x32, 0x00, 0x00, 0x00, 0x00, 0x01, 0x55, 0xAA}
	var age = utils.IntToBytes(uint64(34), 2, false)
	fmt.Println("AGE : ", age)
	OPEN_LOT.Print()
	OPEN_LOT.Insert(age, 2) // item id
	OPEN_LOT.Print()

	var locationX = utils.FloatToBytes(7.86, 5, false)
	fmt.Println("LOCATION_X : ", locationX)
	OPEN_LOT.Insert(locationX, 69)
	OPEN_LOT.Print()

	OPEN_LOT.SetLength(0x24)
	length := 0x24
	n := length / 256
	fmt.Println("Osman", length, n)
	OPEN_LOT[3] = byte(n)
	OPEN_LOT[2] = byte(length - n*256)

	fmt.Println("Insert to not active field")
	OPEN_LOT.Insert([]byte{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 19) // item sockets
	OPEN_LOT.Print()
}

func testArea() {

	//Array & Slice
	var arr []string = []string{"A", "B", "C"}
	arr = append(arr, "D", "E")
	fmt.Println(arr)

	var fruits = []string{"Apple", "Banana", "Grape", "Orange"}
	fmt.Println(fruits[0:2]) //prints [Apple, Banana]

	//For Loop
	for i, card := range arr {
		fmt.Println(i, "=>", card)
	}

	//Custom Type Usage
	myDeck := newDeck()
	hand, remaining := deal(myDeck, 3)
	hand.printIt()
	fmt.Println("---------------")
	remaining.printIt()

	//Type Conversion
	fmt.Println("Type Convert => ", []byte("Hi there!"))
	fmt.Println(myDeck.toString())
	fmt.Println("----------------")

	//File IO
	fmt.Println("File IO Write=> ", myDeck.saveToFile("deck_save.json"))
	fmt.Println("File IO Read=> ", myDeck.readFromFile("deck_save.json"))
	fmt.Println("----------------")

	//Shuffle
	myDeck.shuffle()
	fmt.Println("Shuffle 1=> ", myDeck)
	myDeck.shuffle()
	fmt.Println("Shuffle 2=> ", myDeck)
	fmt.Println("----------------")

	//Struct example
	osman := person{name: "Osman", age: 34, info: contact{}}
	fmt.Println(osman)
	osman.age = 35
	osman.info = contact{email: "a@b.com", phone: "0542331"}
	fmt.Printf("person => %+v\n", osman)
	osman.test()
	fmt.Println("----------------")

	//Map Test
	mapTest()
}
