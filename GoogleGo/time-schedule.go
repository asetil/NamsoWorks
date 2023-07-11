package main

import (
	"fmt"
	"time"
)

func testTimer() {
	time.Sleep(time.Second / 2) //sleep 0,5s

	time.AfterFunc(time.Second*10, func() {
		fmt.Println("Timer triggered..")
	})
}
