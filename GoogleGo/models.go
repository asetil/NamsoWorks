package main

import (
	"encoding/json"
	"fmt"
	awareLog "log" //rename namespace

	"github.com/tidwall/gjson"
)

type (
	Student struct {
		Ad            string        `json:"ad,omitempty"`
		Soyad         string        `json:"soyad,omitempty"`
		Yas           int           `json:"age,omitempty"`
		OgrenimDurumu OgrenimDurumu `json:"ogdrm,omitempty"` //Enum
	}
)

func testModel() {
	st := Student{
		Ad:            "Osman",
		Soyad:         "Sokuoğlu",
		Yas:           35,
		OgrenimDurumu: DOCTORA,
	}

	fmt.Println(st)

	if st.OgrenimDurumu == DOCTORA {
		fmt.Println("Doktora öğrencisi")
	}

	data, err := json.Marshal(st) //JSON serialize example
	if err != nil {
		awareLog.Println("Log error:", err)
		return
	}

	//Get data by key from json data
	age := gjson.Get(string(data), "age").String()
	fmt.Println("Data:", string(data), "Age:", age)
}
