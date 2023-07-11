package main

import "fmt"

type contact struct {
	email string
	phone string
}

type person struct {
	name string
	age  int
	info contact
}

//Go is a pass by value language
//it will create a copy and use this copy inside function
//So out of funct name will be still with old value
func (p person) updateName(name string) {
	p.name = name

}

//to overcome this a reference must be passed
//so here pointers come to screen
func (p *person) updateNameWithPointer(name string) { //*person => we are working with a pointer to person, we are passing reference of person namely
	(*p).name = name // *p => give me the value this memory adress is pointing at
}

func (p person) test() {
	p.updateName("isa")
	fmt.Println("updateName=>", p)

	pointerPerson := &p // &value => give me the memory address of this variable pointing at
	// ??? p.updateNameWithPointer("yessa") //this will also work since go handle person to pointer of Person at background for us 
	(pointerPerson).updateNameWithPointer("musa")
	fmt.Println("updateNameWithPointer=>", p)
}
