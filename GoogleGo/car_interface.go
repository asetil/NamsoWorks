package main

import (
	"fmt"
)

//Interfaces are not generic types in go
type Car interface {
	getModelInfo() string
}

type bmwCar struct{}
type citroenCar struct{}

func (bmw bmwCar) getModelInfo() string {
	return "Bmw 520d"
}

func (citroen citroenCar) getModelInfo() string {
	return "Citroen C4 Cactus"
}

//Interfaces are implicit namely no need to tell that our custom type should implements this interface
func printCarInfo(car Car) {
	fmt.Println(car.getModelInfo())
}

func testInterface() {
	var bmw = bmwCar{}
	var citroen = citroenCar{}

	printCarInfo(bmw)
	printCarInfo(citroen)
}
