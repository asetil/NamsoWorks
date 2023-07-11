package main

import (
	"fmt"
	"net/http"
	"time"
)

//GO ROUTINES
//Go da routine ler thread kavramı olarak kullanılıyor diyebiliriz. Asenkron çalışması (paralel)
//gereken kodlarınız varsa go routine bu noktada çözüm oluyor
//To start a new routine we use "go" keyword, as likewise async keyword in c#

//Concurrency : we can have multiple threads executing code, if one thread is blocking another one is picked up and worked on.
//Paralleism : multiple threads executed at the exact time. Required multiple CPU's

func testRoutines() {
	links := []string{
		"http://www.google.com",
		"https://www.osokuoglu.com.tr",
		"http://amazon.com.tr",
		"http://udemy.com.tr",
	}

	//this is classical serial running program
	for _, link := range links {
		checkLink(link)
	}

	//this is the solution for parallel programming
	for _, link := range links {
		//Here go keyword creates a new go routine namely a child routine inside main rotine
		go checkLink(link)
	}

	//CHANNELS
	//Main routine does not wait child routines to finish and when it finish execution of code just terminates execution and entire program will quit
	//If you run checkLinksParalel function above nothing will be printed to terminal
	//The solution for this situaion is work with channels
	// A channel is  like a medium in which all routines (main and childs) can communicate with each other
	//A channel can be created with any type (string, int, float etc.. communication language) but its type is tight.
	//So the data or message that we send through a channel must be always the same type

	cnl := make(chan string)
	for _, link := range links {
		go checkLinkWithChannel(link, cnl) //Sending channel value for comminunication
	}

	//chanValue <- cnl              //Read channel and assign to value
	//fmt.Println(<-cnl) //Read data from channel and print to output

	for l := range cnl { //Run at every 6 hours
		go func(link string) { //Using lambda, literal function
			time.Sleep(time.Hour * 6)
			checkLinkWithChannel(link, cnl)
		}(l) //executing ananymous function
	}
}

func checkLink(link string) {
	_, err := http.Get(link)
	if err != nil {
		fmt.Println(link, "is failed!")
		return
	}
	fmt.Println(link, "is up!")
}

func checkLinkWithChannel(link string, cnl chan string) {
	_, err := http.Get(link)
	if err != nil {
		fmt.Println(link, "is failed!")
		cnl <- link //Send value to channel by child routine
		return
	}

	fmt.Println(link, "is up!")
	cnl <- link //Send value to channel by child routine
}
