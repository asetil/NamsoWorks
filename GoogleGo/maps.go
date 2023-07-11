package main

import "fmt"

//A map is a collection of key & value pairs, likewise a hash or dictionary
//All keys are same type and al values must be the same type
// Ex => colors:= map[string]string{"red":"#f00", "blue":"#0f0"} -----> map of <string, string>
//Map is a reference type

func mapTest() {
	//colors:= make(map[string]string)
	colors := map[string]string{"red": "#f00", "blue": "#0f0"}
	colors["white"] = "#fff"
	colors["gray"] = "#eee"
	delete(colors, "gray")

	fmt.Println(colors)

	for key, value := range colors {
		fmt.Println(key + "=>" + value)
	}
}
